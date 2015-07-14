 $(document).ready(function (e) {

    $(document).ajaxStart(function () {
        $("#grid").css({ "pointer-events": "none" });
    });
    $(document).ajaxComplete(function () {
        $("#grid").css({ "pointer-events": "all" });
        AttributeResultCallBack();
    });

    //these variables are put to save the search parameters in order not to change them while changing the records per page
    var AttributeId = $("#AttributeId").val().trim();
    var Published = $("#published option:selected").val();
    var PageSize = $("#PageSize option:selected").val();

    //Initialy don't pass the search parameters
    DisplaySearchResult(AttributeId, Published, PageSize);
    //alert('I am in JS :D');
    //On click search button
    $("#attr-form").submit(function (e) {
        e.preventDefault();

        //set the search variables
        AttributeId = $("#AttributeId").val().trim();
        Published = $("#published option:selected").val();
        
        //pass the search parameters to the display search result
        DisplaySearchResult(AttributeId, Published, PageSize);

    });

    //when changing the records per page only changes the pagesize parameter and takes the old search variables(attributeId, published)
    $("#PageSize").change(function (e) {
        PageSize = $("#PageSize").val();

        DisplaySearchResult(AttributeId, Published, PageSize);
    });

    //When clicking the reset button reset all the fields to the initial state
    $("#reset-btn").click(function (e) {
        e.preventDefault();

        $("#AttributeId").val("");
        $("#published option[value=2]").prop("selected", true);
        $("#PageSize option[value=100]").prop("selected", true);

        //unset all the search variables
        AttributeId = $("#AttributeId").val();
        Published = $("#published option:selected").val();
        PageSize = $("#PageSize option:selected").val();

        DisplaySearchResult(AttributeId, Published, PageSize);
    });
});

//This function loads the container with the proper search parameters
function DisplaySearchResult(attributeId, published, pageSize) {
    var object = {};
    object.AttributeId = attributeId;
    object.Published = published;
    object.PageSize = pageSize;
    LoadContainer(BuildUrl("/Attribute/AttributeResult"), "#AttributeResult", object, true);
}

//This function is used to update the given attribute to the desired state(Published/unpublished)
function PublishAction(id, imageUrl) {
    var status;
    var statusLabelElementId = "#attribute-element-status-label-" + id;
    var lastUpdatedByElementId = "#attribute-element-updated-by-" + id;

    //Display the ajax loader
    StartLoading(imageUrl, statusLabelElementId);

    //removes the color of the status
    $(statusLabelElementId).removeAttr("class");

    //gets the status based on the checked/unchecked status of the checkbox
    if ($("#attribute-element-status-" + id).is(":checked"))
        status = true;
    else
        status = false;

    $.ajax({
        type: "put",
        url: BuildUrl("/Attribute/PutAttribute"),
        data: { id: id, status: status },
        success: function (user) {
            //Changing the last updated by label
            if(user != ""){
                $(lastUpdatedByElementId).html(user);

                //Changing the status label
                if (!status) {
                    $(statusLabelElementId).html("Unpublished");
                    $(statusLabelElementId).attr("class", "red");
                } else {
                    $(statusLabelElementId).html("Published");
                    $(statusLabelElementId).attr("class", "green");
                }
            }
            //If the user returned is null than print the error 
            else {
                showMessage(statusLabelElementId, error_Unknow, 1);
            }
        },
        error: function (e) {
            //in case there was an internal server error
            showMessage(statusLabelElementId, error_Unknow, 1);
        }
    });
}