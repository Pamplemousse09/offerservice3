var EdtiorModel = function () {
    var self = this;
    self.offers = ko.observableArray();
    self.error = ko.observable();
    self.terms = ko.observable();
    self.targets = ko.observable();

    self.newOffer = {
        Id: ko.observable(),
        Decription: ko.observable(),
        LOI: ko.observable(),
        IR: ko.observable(),
        active: ko.observable()
    }

    var offerUri = '/api/offer/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllOffers() {
        ajaxHelper(offerUri, 'GET').done(function (data) {
            self.offers(data);
        });
    }

    self.getDetails = function (item) {
        ajaxHelper(offerUri + item.Id, 'GET').done(function (data) {
            self.terms(data.Terms[0]);
            self.targets(data.Target[0]);
        });
    }

    /*self.addOffer = function (formElement) {
        
        var s, p;
        for (p in self.newOffer) {
            s += p + ": " + self.newOffer[p] + "\n";
        }
        alert(s);
    }*/

    self.addOffer = function (formElement) {
        var offer = {
            //AuthorId: self.newBook.Author().Id,
            Description: self.newOffer.Decription(),
            LOI: self.newOffer.LOI(),
            IR: self.newOffer.IR(),
            active: self.newOffer.active()
        };

        ajaxHelper(offerUri, 'POST', offer).done(function (item) {
            self.offers.push(item);
        });
    }

    // Fetch the initial data.
    getAllOffers();
}

ko.applyBindings(new EdtiorModel());