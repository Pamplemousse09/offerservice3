dump_attributes_fromXML()
{

cat <<EOF
--USE [OffersII];

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------
IF OBJECT_ID(N'[dbo].[FK_RespondentAttributes_AttributeId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespondentAttributes] DROP CONSTRAINT [FK_RespondentAttributes_AttributeId];
IF OBJECT_ID(N'[dbo].[FK_AttributeSettings_AttributeId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttributeSettings] DROP CONSTRAINT [FK_AttributeSettings_AttributeId];

-- --------------------------------------------------
-- Dropping and recreating the tables
-- --------------------------------------------------
IF OBJECT_ID(N'[dbo].[AttributeOptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttributeOptions];

IF OBJECT_ID(N'[dbo].[Attributes]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Attributes];
END


CREATE TABLE [dbo].[Attributes](
	[Id] [nvarchar](50) NOT NULL,
	[Name] [varchar](100) NULL,
	[ShortName] [varchar](100) NULL,
	[Label] [varchar](500) NULL,
    [Type] [varchar](100) NULL,
    PRIMARY KEY CLUSTERED 
    (
	  [Id] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

-- Adding trigger on Attributes table to automatically create AttributeSettings for the newly added attribute

IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trigger_InsertAttributeSetting_Attributes')
BEGIN
DROP TRIGGER [dbo].[trigger_InsertAttributeSetting_Attributes]
END

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE TRIGGER [dbo].[trigger_InsertAttributeSetting_Attributes] ON [dbo].[Attributes]
FOR INSERT

AS
DECLARE @AttributeId nvarchar(max)
SET @AttributeId = (SELECT Id FROM Inserted)
	
IF NOT EXISTS (SELECT * FROM AttributeSettings WHERE AttributeId = @AttributeId)
BEGIN
INSERT INTO AttributeSettings
        (AttributeId)
    SELECT
        Id
        FROM inserted
END

GO

CREATE TABLE [dbo].[AttributeOptions](
	[AttributeId] [nvarchar](50) NOT NULL,
	[Code] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
    CONSTRAINT [pk_AttributeCode] PRIMARY KEY CLUSTERED 
    (
	  [AttributeId] ASC,
	  [Code] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- --------------------------------------------------
-- Seed the Attributes and AttributeOptions tables
-- --------------------------------------------------
EOF

  cat $1 | 
	sed 's/""//g' | 
	sed 's/\"apps/apps/' | 
	sed 's/\"normal\"/normal/g' | 
  awk '{
    if ($1 == "<attribute"){
      dumped = 0; name=""; shortName=""; label="";
      ident = getV2($0,"ident");
      type = getV2($0,"type");
      stop = 0;
      while(stop == 0)
      {
        getline;
        if (index($0, "<name>") > 0)
        {
          name = getV($0, "name");
        }
        else if (index($0, "<shortName>") > 0)
        {
          shortName = getV($0, "shortName");
        }
        else if (index($0, "<label>") > 0)
        {
          label = getV($0, "label");
        }
        else if (index($0, "<values ") > 0 || (index($0, "<values>") > 0))
        {
          if (name != "")
          {
            # Dump the attribute first before the codes
            printf("--\n");
            dump_attribute(ident, name, shortName, label);
            dumped = 1;
          }
          getline;
          while(index($0, "</values>") <= 0)
          {
            if ($1 ==  "<value")
            {
              code = getV2($0,"code");
              desc = getCodeDescription($0);
              dump_options(ident, code, desc);
            }
            getline;
          }
        }
        else if (index($0, "</attribute>") > 0)
        {
          if (dumped == 0)
          {
            # the attribute had no values i.e. birthdate, so dump it no
            dump_attribute(ident, name, shortName, label);
            dumped = 1;
          }
          stop = 1;
        }
      }
    }
  }
  function getV(s,tag)
  {
    a = sprintf("<%s>", tag);
    b = sprintf("</%s>", tag);
    start = index(s, a) + length(a);
    end = index(s, b);
    s3 = substr(s, start, (end-start));
    return s3;
  }
  function getCodeDescription(s)
  {
    start = index(s, ">") + 1;
    s2 = substr(s, start, 1000);
    end = index(s2, "</");
    s3 = substr(s2, 1, end-1);
    return s3;
  }
  function getV2(s,l)
  {
    tag = sprintf("%s=", l);
    start = index(s, tag) + length(tag) + 1;
    s2 = substr(s, start, 1000);
    end = index(s2, "\"") - 1;
    s3 = substr(s2, 0, end);
    return s3;
  }
  function dump_options(id, code, desc)
  {
    printf("insert into [dbo].[AttributeOptions](AttributeId, Code, Description) Values(\"%s\", \"%s\", \"%s\");\n", id, code, desc);
  }
  function dump_attribute(id, name, shortName, label)
  {
    printf("insert into [dbo].[Attributes](Id, Name, ShortName, Label, Type) Values(\"%s\", \"%s\", \"%s\", \"%s\", \"%s\");\n",
      id, name, shortName, label, type);
  }
  END{
    printf("GO\n");
  }'  | sed "s/'//g" | tr -s "\"" "\'"

cat <<EOF

-- --------------------------------------------------
-- Define FOREIGN KEY constraints
-- --------------------------------------------------
ALTER TABLE [dbo].[AttributeOptions]  WITH CHECK ADD  CONSTRAINT [FK_AttributeOptions_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES [dbo].[Attributes] ([Id]) ON UPDATE CASCADE
ALTER TABLE [dbo].[AttributeOptions] CHECK CONSTRAINT [FK_AttributeOptions_AttributeId]
GO
ALTER TABLE [dbo].[RespondentAttributes] WITH CHECK ADD CONSTRAINT [FK_RespondentAttributes_AttributeId] FOREIGN KEY([Ident]) REFERENCES [dbo].[Attributes] ([Id]) ON UPDATE CASCADE
ALTER TABLE [dbo].[RespondentAttributes] CHECK CONSTRAINT [FK_RespondentAttributes_AttributeId]
GO
ALTER TABLE [dbo].[AttributeSettings] WITH CHECK ADD CONSTRAINT [FK_AttributeSettings_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES [dbo].[Attributes] ([Id]) ON UPDATE CASCADE
ALTER TABLE [dbo].[AttributeSettings] CHECK CONSTRAINT [FK_AttributeSettings_AttributeId]
GO
EOF
}

input=""
if [ $# -gt 0 -a -f "$1" ]; then
  input=$1
  dump_attributes_fromXML $input
else
  echo "Usage: $0 filename"
fi
