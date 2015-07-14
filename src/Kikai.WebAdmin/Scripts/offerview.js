$(document).ready(function (e) {

    var OfferId = $("#offer_view_id").val();
    DisplaySearchResult(OfferId, null);
    var client = new ZeroClipboard($(document.getElementById("copy-to-clipboard")), {
        moviePath: BuildUrl("/Scripts/ZeroClipboard.swf")
    });

    client.on("ready", function (readyEvent) {

        client.on("aftercopy", function (event) {
            $("#copied-msg").show();
            setTimeout(function () {
                $("#copied-msg").fadeOut(300);
            }, 800);
        });
    });
});

function DisplaySearchResult(OfferId, pageSize) {
    var object = {};
    object.OfferId = OfferId;
    object.PageSize = pageSize;
    
    LoadContainer( BuildUrl("/Offer/GetOfferAttributesView"), "#AttributesResult", object);
}
