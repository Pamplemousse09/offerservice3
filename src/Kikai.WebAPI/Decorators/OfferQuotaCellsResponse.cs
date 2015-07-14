using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Kikai.Logging.DTO;
using Kikai.WebApi.DTO;
using System.Collections.Generic;
using Kikai.Common.DTO;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "")]
    public class OfferQuotaCellsResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [XmlElement("Data", Type = typeof(QuotaExpressionsObjectResponse))]
        [DataMember(Order = 2)]
        public QuotaExpressionsObjectResponse Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;


        public OfferQuotaCellsResponse() { }

        public OfferQuotaCellsResponse(string RequestId, bool status = false)
        {
            Status = status;
            this.Meta = new MetaDataObject(RequestId);
        }

    }

    public class QuotaExpressionsObjectResponse : IXmlSerializable
    {
        public object QuotaCells { get; set; }

        [DataMember(Order = 1)]
        public List<ErrorObject> Errors { get; set; }

        public QuotaExpressionsObjectResponse()
        {
            this.Errors = new List<ErrorObject>();
        }

        public QuotaExpressionsObjectResponse(string QuotaCells)
        {
            this.QuotaCells = QuotaCells;
        }

        /// <summary>
        /// Interface implementation not used here.
        /// </summary>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Interface implementation, which reads the content of the CDATA tag
        /// </summary>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.QuotaCells = reader.ReadElementString();
        }

        /// <summary>
        /// Interface implementation, which writes the CDATA tag to the xml
        /// </summary>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("QuotaCells");
            if (this.QuotaCells != null)
            {
                writer.WriteRaw((string)this.QuotaCells);
            }
            else
            {
                writer.WriteAttributeString("i", "nil", null, "true");
            }

            writer.WriteEndElement();
            writer.WriteStartElement("Errors");
            if (Errors != null && Errors.Count > 0)
            {
                foreach (var error in Errors)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Code", error.Code.ToString());
                    writer.WriteElementString("Message", error.Message);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteAttributeString("i", "nil", null, "true");
            }
            writer.WriteEndElement();
        }
    }

}
