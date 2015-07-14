var formSubmitting = false;
var setFormSubmitting = function () { formSubmitting = true; };
var isDirty = function () { return false; }
var offerDetailsPage = true;
var suspendedMsg;
var stopThread = false;

$(document).ready(function (e) {
    $("#PublishedAttributes option[value=-2]").prop("disabled", true);
    var OfferId = $("#offer_edit_id").val();
    DisplaySearchResult(OfferId, null);
    UpdateStatusLabel(OfferId);
    var times = 0;
    window.setInterval(function () {
        times++;
        if (!stopThread) {
            var object = {};
            object.OfferId = OfferId;

            UpdateStatusLabel(OfferId);
            LoadContainer(BuildUrl("/Offer/GetOfferAttributes"), "#AttributesResult", object);
        }
    }, 10000);

    $('#PublishedAttributes').on('change', function () {
        var selectedIndex = $(this).find(":selected").val();
        var selectedAttributeIdent = $(this).find(":selected").html();
        if (selectedIndex == 0) {
            $("#attribute-value").prop('disabled', true);
            $("#add-attribute").prop('disabled', true);
            $("#chosen-attribute-info").hide();
        }
        else if (selectedIndex == -1) {
            $("#attribute-value").prop('disabled', true);
            $("#add-attribute").prop('disabled', true);
            $("#chosen-attribute-info").hide();
            window.location.href = BuildUrl("/Attribute");
        }
        else {
            $("#attribute-value").prop('disabled', false);
            $("#add-attribute").prop('disabled', false);
            $.ajax({
                type: "get",
                url: BuildUrl("/RespondentAttribute/AttributeInfo"),
                data: { attributeId: selectedAttributeIdent },
                success: function (info) {
                    var attribute = JSON.parse(info);
                    $("#info-attribute-id").html(attribute.Id);
                    $("#info-attribute-type").html(attribute.Type);
                    $("#info-attribute-label").html(attribute.Label);
                    $("#chosen-attribute-info").show();
                },
                error: function (e) {
                    //In case there was an internal server error
                    alert(error_Unknow);
                }
            });
        }
    });

    $('#add-attribute').on('click', function () {
        $("#attribute-value").css("border-color", "#CCC");
        var selectedAttributeIndex = $("#PublishedAttributes").find(":selected").val();
        var selectedAttributeIdent = $('#PublishedAttributes').find(":selected").html();
        var selectedAttributeValue = $('#attribute-value').val().trim();
        alert(selectedAttributeValue);
        var Attribute = {};
        var errors = 0;
        Attribute.Ident = selectedAttributeIdent;
        Attribute.Values = selectedAttributeValue;
        Attribute.OfferId = OfferId;
        if (selectedAttributeValue == "") {
            $("#attribute-value").css("border-color", "#F00");
            errors++;
        }
        if (errors == 0) {
            $.ajax({
                type: "get",
                url: BuildUrl("/RespondentAttribute/AddRespondentAttribute"),
                data: { result: JSON.stringify(Attribute) },
                success: function (inserted) {
                    $('#PublishedAttributes option:eq(0)').prop('selected', true);
                    $('#attribute-value').val("");
                    $("#PublishedAttributes option[value='" + selectedAttributeIndex + "']").remove();
                    $("#attribute-value").prop('disabled', true);
                    $("#add-attribute").prop('disabled', true);
                    $("#chosen-attribute-info").hide();
                    $("#attribute-value").css("border-color", "#CCC");
                    DisplaySearchResult(OfferId, null);
                },
                error: function (e) {
                    //In case there was an internal server error
                    alert(error_Unknow);
                }
            });
        }
    });

    $("#offer-edit-form").submit(function (e) {
        e.preventDefault();
        ResetErrors();
        var errors = 0;
        var sampleId = parseInt($("#offer_edit_sampleid").val());
        var offerTitle = $("#offer_edit_title").val();
        var offerTopic = $("#offer_edit_topic").val();
        var offerDescription = $("#offer_edit_description").val();
        var offerTest = $("#provider_edit_test").prop("checked");
        var cpi = $("#offer-element-set-cpi-value-" + OfferId).val();
        var Offer = {};
        Offer.Id = OfferId;
        Offer.SampleId = sampleId;
        Offer.Title = offerTitle;
        Offer.Topic = offerTopic;
        Offer.Description = offerDescription;
        Offer.TestOffer = offerTest;
        if (Offer.Title == "") {
            showMessage("#error-name", error_FieldCannotBeEmpty, 1);
            $("#offer_edit_title").css("border-color", "#F00");
            errors++;
        }
        if (errors == 0) {
            $('#offer-edit-form').data('serialize', $('#offer-edit-form').serialize());
            showMessage("#saving-offer", msg_ProcessingRequest);
            UpdateOffer(Offer);
            setTimeout(function () {
                $("#saving-offer").hide();
            }, 3000);
        }
    });

    $("body").delegate('#AttributesDropDownResult', 'click', function () {
        $("#PublishedAttributes option[value=-2]").prop("disabled", true);
    });
});



function AttributeChange(e) {
    var OfferId = $("#offer_edit_id").val();
    var selectedIndex = $(e).find(":selected").val();
    var selectedAttributeIdent = $(e).find(":selected").html();
    if (selectedIndex == 0) {
        $("#attribute-value").prop('disabled', true);
        $("#add-attribute").prop('disabled', true);
        $("#chosen-attribute-info").hide();
    }
    else if (selectedIndex == -1) {
        $("#attribute-value").prop('disabled', true);
        $("#add-attribute").prop('disabled', true);
        $("#chosen-attribute-info").hide();
        window.location.href = BuildUrl("/Attribute");
    }
    else {
        $("#attribute-value").prop('disabled', false);
        $("#add-attribute").prop('disabled', false);
        $.ajax({
            type: "get",
            url: BuildUrl("/RespondentAttribute/AttributeInfo"),
            data: { attributeId: selectedAttributeIdent },
            success: function (info) {
                var attribute = JSON.parse(info);
                $("#info-attribute-id").html(attribute.Id);
                $("#info-attribute-type").html(attribute.Type);
                $("#info-attribute-label").html(attribute.Label);
                $("#chosen-attribute-info").show();
            },
            error: function (e) {
                //In case there was an internal server error
                alert(error_Unknow);
            }
        });
    }

    $('#add-attribute').on('click', function () {
        $("#attribute-value").css("border-color", "#CCC");
        var selectedAttributeIndex = $("#PublishedAttributes").find(":selected").val();
        var selectedAttributeIdent = $('#PublishedAttributes').find(":selected").html();
        var selectedAttributeValue = $('#attribute-value').val().trim();
        var Attribute = {};
        var errors = 0;
        Attribute.Ident = selectedAttributeIdent;
        Attribute.Values = selectedAttributeValue;
        Attribute.OfferId = OfferId;
        if (selectedAttributeValue == "") {
            $("#attribute-value").css("border-color", "#F00");
            errors++;
        }
        if (errors == 0) {
            $.ajax({
                type: "get",
                url: BuildUrl("/RespondentAttribute/AddRespondentAttribute"),
                data: { result: JSON.stringify(Attribute) },
                success: function (inserted) {
                    $('#PublishedAttributes option:eq(0)').prop('selected', true);
                    $('#attribute-value').val("");
                    $("#PublishedAttributes option[value='" + selectedAttributeIndex + "']").remove();
                    $("#attribute-value").prop('disabled', true);
                    $("#add-attribute").prop('disabled', true);
                    $("#chosen-attribute-info").hide();
                    $("#attribute-value").css("border-color", "#CCC");
                    DisplaySearchResult(OfferId, null);
                },
                error: function (e) {
                    //In case there was an internal server error
                    alert(error_Unknow);
                }
            });
        }
    });
}

$('#offer-edit-form').data('serialize', $('#offer-edit-form').serialize()); // On load save form current state

$(window).bind('beforeunload', function (e) {
    if ($('#offer-edit-form').serialize() != $('#offer-edit-form').data('serialize')) return msg_LeaveWithoutSaving;
    else e = null; // i.e; if form state change show warning box, else don't show it.
});

function DisplaySearchResult(OfferId, pageSize) {
    var object = {};
    object.OfferId = OfferId;
    object.PageSize = pageSize;

    LoadContainer(BuildUrl("/Offer/GetDropDownOfferAttributes"), "#AttributesDropDownResult", object);
    LoadContainer(BuildUrl("/Offer/GetOfferAttributes"), "#AttributesResult", object);
}

function UpdateOffer(Offer, status) {
    $.ajax({
        type: "post",
        url: BuildUrl("/Offer/EditSubmit"),
        data: { result: JSON.stringify(Offer) },
        success: function (updated) {
            showMessage("#saving-offer", msg_ChangesSaved, -1);
            $("#item-title").html("Offer " + Offer.Title);
            UpdateStatusLabel(Offer.Id);
        },
        error: function (e) {
            alert(error_Unknow);
        }
    });
}

function ResetErrors() {
    $("#error-cpi").hide();
    $("#error-name").hide();
    $("#offer_edit_title").css("border-color", "#CCC");
}

function DeleteAttribute(id, name, offerId) {
    var conf = confirm("Are you sure you want to deleted attribute " + name + "?");
    if (conf == true) {
        $.ajax({
            type: "post",
            url: BuildUrl("/RespondentAttribute/DeleteAttribute"),
            data: { attributeId: id },
            success: function (deleted) {
                DisplaySearchResult(offerId, null);
            },
            error: function (e) {

            }
        });
    }
    else {
        return false;
    }
}

function UpdateStatusLabel(OfferId) {
    $.ajax({
        type: "get",
        url: BuildUrl("/Offer/GetStatus"),
        data: { OfferId: OfferId },
        success: function (e) {
            SetStatusHtml(e);
        }, error: function (e) {
            $("#status-label").html(error_Unknow);
        }
    });

    $.ajax({
        type: "get",
        url: BuildUrl("/Offer/GetQuotaRemaining"),
        data: { OfferId: OfferId },
        success: function (e) {
            $("#offer_edit_quota_remaining").val(e);
        }, error: function (e) {
            $("#offer_edit_quota_remaining").val("Failed to get Quota Remaining for this offer.");
        }
    });
}

function ChangeStatus(OfferId, Status) {
    stopThread = true;
    $("#submit-button").prop("disabled", true);
    $("#status-label").html(msg_ProcessingRequest).css({ "font-size": "0.8em", "font-style": "italic" });
    $("#activate-btn").prop("disabled", true);
    $("#trying-status").hide();
    $("#suspended-status").hide();
    var offer = {};
    offer.Id = OfferId;
    offer.Status = Status;
    offer.SampleId = parseInt($("#offer_edit_sampleid").val());
    clearInterval();
    $.ajax({
        type: "put",
        url: BuildUrl("/Offer/UpdateStatus"),
        data: { result: JSON.stringify(offer) },
        success: function (e) {
            $("#status-label").css({"font-size": "1em", "font-style": "normal"});
            if (e == 21) {
                suspendedMsg = msg_OfferSuspended_NotMainstreamEnabled;
            } else if (e == 22) {
                suspendedMsg = msg_OfferSuspended_NoQuotaCells;
            } else if (e == 2) {
                suspendedMsg = msg_OfferSuspended;
            }
            SetStatusHtml(e);
            DisplaySearchResult(OfferId, null);
            $("#submit-button").prop("disabled", false);
            stopThread = false;
        }
    })
}

function SetStatusHtml(Status) {
    if (Status == -1) {
        $("#status-label").html(msg_OfferInactive);
        $("#suspended-status").hide();
        $("#activate-btn").prop("disabled", true);
        $("#activate-error em").html(error_CannotActivateOffer);
        $("#deactivate-btn").prop("disabled", true);
        $("#activate-error").show();
    } else if (Status == 1) {
        $("#status-label").html(msg_OfferActive);
        $("#trying-status").hide();
        $("#suspended-status").hide();
        $("#activate-btn").prop("disabled", true);
        $("#deactivate-btn").prop("disabled", false);
        $("#activate-error").hide();
    } else if (Status == 2 || Status == 21 || Status == 22) {
        $("#status-label").html(msg_OfferSuspended);
        $("#suspended-status").html(suspendedMsg);
        $("#suspended-status").show();
        $("#trying-status").hide();
        $("#activate-btn").prop("disabled", false);
        $("#deactivate-btn").prop("disabled", false);
        $("#activate-error").hide();
    } else if (Status == 3) {
        $("#status-label").html(msg_OfferPending);
        $("#trying-status").html(msg_OfferRetryingActivation);
        $("#trying-status").show();
        $("#suspended-status").hide();
        $("#activate-btn").prop("disabled", false);
        $("#deactivate-btn").prop("disabled", false);
        $("#activate-error").hide();
    } else {
        $("#status-label").html(msg_OfferInactive);
        $("#trying-status").hide();
        $("#suspended-status").hide();
        $("#activate-btn").prop("disabled", false);
        $("#deactivate-btn").prop("disabled", true);
        $("#activate-error").hide();
    }
}