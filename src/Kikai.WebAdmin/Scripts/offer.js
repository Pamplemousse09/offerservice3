$(document).ready(function (e) {
    //On each ajaxStart Disable grid
    $(document).ajaxStart(function () {
        $("#grid").css({ "pointer-events": "none" });
    });

    //ajaxEnd Enable the grid
    $(document).ajaxComplete(function () {
        $("#grid").css({ "pointer-events": "all" });
        OfferResultCallBack();
    });

    //Takes the initial values of the field
    var StudyId = $("#StudyId").val();
    var OfferTitle = $("#OfferTitle").val();
    var OfferStatus = $("#OfferStatus option:selected").val();
    var OfferType = $("#OfferType option:selected").val();
    var PageSize = $("#PageSize").val();

    //Display the container of the found offers
    DisplaySearchResult(StudyId, OfferTitle, OfferStatus, OfferType, PageSize);
    //This call is made to enable the click Enter
    $("#search-form").keyup(function (e) {
        if (e.keyCode == 13) {
            StudyId = $("#StudyId").val().trim();
            OfferTitle = $("#OfferTitle").val().trim();
            OfferStatus = $("#OfferStatus option:selected").val();
            OfferType = $("#OfferType option:selected").val();

            DisplaySearchResult(StudyId, OfferTitle, OfferStatus, OfferType, PageSize);
        }
    });

    //When clicking the search button reload the offers container
    $("#SearchOffers").click(function (e) {
        StudyId = $("#StudyId").val().trim();
        OfferTitle = $("#OfferTitle").val().trim();
        OfferStatus = $("#OfferStatus option:selected").val();
        OfferType = $("#OfferType option:selected").val();

        DisplaySearchResult(StudyId, OfferTitle, OfferStatus, OfferType, PageSize);
    });

    //When changing the page size reload the offers page
    $("#PageSize").change(function (e) {
        PageSize = $("#PageSize").val();

        DisplaySearchResult(StudyId, OfferTitle, OfferStatus, OfferType, PageSize);
    });

    //When clicking the reset button reset all the fields to the initial state
    $("#reset-btn").click(function (e) {
        e.preventDefault();

        $("#StudyId").val("");
        $("#OfferTitle").val("");
        $("#OfferStatus option[value=1]").prop("selected", true);
        $("#OfferType option[value=0]").prop("selected", true);
        $("#PageSize option[value=50]").prop("selected", true);

        //unset all the search variables
        StudyId = $("#StudyId").val();
        OfferTitle = $("#OfferTitle").val();
        OfferStatus = $("#OfferStatus option:selected").val();
        OfferType = $("#OfferType option:selected").val();
        PageSize = $("#PageSize").val();

        DisplaySearchResult(StudyId, OfferTitle, OfferStatus, OfferType, PageSize);
    });

    //When clicking on the search button 
    $("#get-sample").submit(function (e) {
        e.preventDefault();
        $("#study-id").css("border-color", "#CCC");
        var validStudyId = $("#study-id").val().trim().match(/^\d+$/);
        //Check if the study Id is not empty and has valid value
        if (validStudyId) {
            showMessage("#loading-sample", msg_ProcessingRequest);
            SearchStudy($("#study-id").val());
        } else {
            //If the study id does not have a valid value display error
            showMessage("#loading-sample", error_FielCannotBeEmptyAndNumeric, 1);
            $("#study-id").css("border-color", "#F00");
        }
    });

    //When exiting the modal reload the page
    $('#myModal').on('hidden.bs.modal', function () {
        location.reload();
    })

});

//Display the offers container
function DisplaySearchResult(studyId, offerTitle, offerStatus, offerType, pageSize) {
    var object = {};
    object.StudyId = studyId;
    object.OfferTitle = offerTitle;
    object.OfferStatus = offerStatus;
    object.OfferType = offerType;
    object.PageSize = pageSize;

    LoadContainer(BuildUrl("/Offer/OfferResult"), "#OfferResult", object, true);
}

var ajaxRequest;

//searching the study
function SearchStudy(studyId) {
    ajaxRequest = $.ajax({
        type: "get",
        url: BuildUrl("/Sample/GetStudySamples"),
        data: { studyId: studyId },
        success: function (e) {
            if (e != "") {
                //if e returned not null value display the container
                $("#Found-Samples").html(e);
                $("#loading-sample").hide();
            } else {
                //if e returned null display error connection to SAM
                showMessage("#loading-sample", error_ConnectionSAMFailed, 1);
            }
        }, error: function (e) {
            //If error occured display unknown error
            showMessage("#loading-sample", error_Unknow, 1);
        }
    });
}

//This function is used to display the terms history
function SearchTerms(offerId) {
    var sampleId = $("#offer-element-sample-id-" + offerId).text();

    $("#terms-history-sample-id").html(sampleId);
    $.ajax({
        type: "get",
        url: BuildUrl("/Term/GetTermsHistory"),
        data: { OfferId: offerId },
        success: function (e) {
            if (e != null)
                $("#terms-history-container").html(e);
            else
                showMessage("#terms-history-container", error_Unknow, 1);
        }, error: function (e) {
            showMessage("#terms-history-container", error_Unknow, 1);
        }
    });
};