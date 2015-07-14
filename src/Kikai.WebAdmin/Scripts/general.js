//These variables are used in the UI in order to notify the user about certain events
var ajaxLoaderGif = "<img src='"+BuildUrl("/Content/img/ajax-loader.gif")+"' width='15' />";
var msg_ProcessingRequest = ajaxLoaderGif + " Processing request, please wait...";
var msg_SampleAlreadyExists = "This sample cannot be added as it already exists in the offer service.";
var msg_LeaveWithoutSaving = "Edited information is not saved."
var msg_OfferActive = "Active";
var msg_OfferInactive = "Inactive";
var msg_OfferSuspended = "Suspended";
var msg_OfferPending = "Pending";
var msg_OfferSuspended_NotMainstreamEnabled = " - Not found as mainstream enabled";
var msg_OfferSuspended_NoQuotaCells = " - Quota cells could not be retrieved";
var msg_OfferRetryingActivation = ajaxLoaderGif + " Activating the offer...";
var msg_ChangesSaved = "Changes have been successfully saved.";
//These variables are used in the UI in order to show error messages to the user about certain events
var error_FielCannotBeEmptyAndNumeric = "This field should contain unsigned integer values.";
var error_ConnectionSAMFailed = "Connection to SAM failed. Please try again later.";
var error_Unknow = "An error occured while processing your request.";
var error_SampleConnectionSAMFailed = "This sample could not be added due to connection problem. Please try again later.";
var error_FailedToLoadSection = "The section failed to load. Please try again later or contact support.";
var error_InvalidCPI = "Unable to set the CPI. Please enter a valid CPI (any numeric value between 1 and 9999).";
var error_FieldCannotBeEmpty = "This field cannot be empty.";
var error_ApiUserAlreadyExists = "This ApiUser is used for another provider.";
var error_CannotActivateOffer = "Title and CPI should be set before activating the offer.";

//This function is used to display messages to the user in 2 forms(error or notification)
function showMessage(containerId, message, error) {
    $(containerId).show();
    if (error == 1) {
        $(containerId).html("<em class='red'>" + message + "</em>")
    } else if (error == -1) {
        $(containerId).html("<em class='green'>" + message + "</em>")
    } else {
        $(containerId).html("<em>" + message + "</em>")
    }
}

function LoadContainer(url, containerId, obj, applyLoader) {

    if (applyLoader) {
        $("#grid").append("<div id='container-loader'><div id='child-loader'>" + msg_ProcessingRequest + "</div></div>");
    }

    $.ajax({
        type: "get",
        url: url,
        data: obj,
        success: function (e) {
            if (e != "") {
                var e = e.replace('<script type="text/javascript">', '<script type="text/javascript"> $("#grid").undelegate();');
                $(containerId).undelegate();
                $(containerId).html(e);
            } else {
                showMessage(containerId, error_FailedToLoadSection, 1);
            }
        },
        error: function (e) {
            showMessage(containerId, error_FailedToLoadSection, 1);
        }
    });
}

function BuildUrl(url) {
    return baseUrl + url;
}

function StartLoading(imageUrl, containerId) {
    $(containerId).html(msg_ProcessingRequest);
}
//This function is used in the UI in order to set the CPI
function SetCPI(id, offerDetailsPage) {
    var offerId = id;
    var error = 0;
    //Handling the CPI field => 0 < CPI < 10000 and not empty
    if ($("#offer-element-set-cpi-value-" + offerId).val() % 1 != 0 || $("#offer-element-set-cpi-value-" + offerId).val() == "" || ($("#offer-element-set-cpi-value-" + offerId).val() < 1 || $("#offer-element-set-cpi-value-" + offerId).val() > 9999)) {
        $("#offer-element-set-cpi-value-" + offerId).css("border-color", "#F00");
        $("<div class=\"cpi-tooltip\">" + error_InvalidCPI + "</div>").insertAfter("#offer-element-set-cpi-value-" + offerId);
        $("#cpi-error").show();
        return false;
    } else {
        $("#offer-element-set-cpi-value-" + offerId).parent().find(".cpi-tooltip").remove();
        var cpiValue = $("#offer-element-set-cpi-value-" + offerId).val();
        cpiValue /= 100;
        $("#offer-element-set-cpi-value-" + offerId).removeClass("field-error");
        $("#offer-element-set-cpi-value-" + offerId).css("border-color", "#CCC");

        var obj = {};
        obj.OfferId = offerId;
        obj.CPI = cpiValue;
        
        var c = confirm("Are you sure you want to set the CPI to " + cpiValue.toFixed(2) + "$ ?");
        if (c == false) {
            return false;
        }

        //Sending ajax request to TermController
        $.ajax({
            type: "post",
            url: BuildUrl("/Term/SetCPI"),
            data: obj,
            success: function (e) {
                $("#offer-element-set-cpi-value-" + offerId).val(cpiValue.toFixed(2));
                $("#cpi-error").hide();
                $("#cpi-msg-edit-error").hide();
                if (offerDetailsPage != null && offerDetailsPage) {
                    $.ajax({
                        type: "get",
                        url: BuildUrl("/Offer/GetStatus"),
                        data: { offerId: offerId },
                        success: function (e) {
                            SetStatusHtml(e);
                        }, error: function (e) {

                        }

                    });
                }
            }, error: function (e) {
                alert("An error occured while trying to process your request.");
            }
        });
    }
}

function SetCPIEnter(e, input) {
    if (e.keyCode == 13) {
        SetCPI(input.id.replace("offer-element-set-cpi-value-", ""));
    }
}

function OfferResultCallBack()
{
    UpdateSortingLinks("Offer");
    UpdateFooterLinks("Offer");
}

function AttributeResultCallBack() {
    UpdateFooterLinks("Attribute");
}

function ProviderResultCallBack() {
    UpdateFooterLinks("Provider");
}

function UpdateSortingLinks(page)
{
    $("th").children().each(function (index) {
        var originalLink = $(this).prop("href");
        var match = originalLink.search("/"+page+"/"+page+"Result");
        var updatedlink = baseUrl + "" + originalLink.substring(match, originalLink.length);
        $(this).prop("href", updatedlink);
    });
}

function UpdateFooterLinks(page)
{
    $("tfoot tr td").children().each(function (index) {
        var originalLink = $(this).prop("href");
        var match = originalLink.search("/"+page+"/"+page+"Result");
        var updatedlink = baseUrl + "" + originalLink.substring(match, originalLink.length);
        $(this).prop("href", updatedlink);
    });
}