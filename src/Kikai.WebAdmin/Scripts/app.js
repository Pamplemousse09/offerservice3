var ViewModel = function () {
    var self = this;
    self.offers = ko.observableArray();
    self.error = ko.observable();

    
    function getAllOffers() {
        var tempOffers = new Array();
        var offer1 = new Object();
        var offer2 = new Object();
        offer1.Description = "Canadian French speakers";
        offer1.Title = "Canadian M who speak French";
        offer1.LOI = "0.10";
        offer1.CPI = "0.5";
        offer1.OfferLink = "https://devhub.globaltestmarket.com/hub/tplm/welcome?E_165_QAhash170&oid=5b801e45-ef73-4c57-4ff1-3c6b70a4747d&tid=4e9281dd-a2aa-8774-8cd0-5cd4f9c1578e&id=1715115";

        offer2.Description = "AddGovernersToMS B - USA";
        offer2.Title = "R175 AddGovernersToMS B - USA";
        offer2.LOI = "3";
        offer2.CPI = "0.1";
        offer2.OfferLink =  "https://devhub.globaltestmarket.com/hub/tplm/welcome?E_165_QAhash170&oid=5b801e45-ef73-4c57-9fd2-3c6b70a4747d&tid=4e9281dd-a3dd-4119-8cd0-5cd4f9c1578e&id=1817815";

        tempOffers[0] = offer1;
        tempOffers[1] = offer2;
        self.offers = tempOffers;
 
    }
    

    // Fetch the initial data.
    getAllOffers();
}

ko.applyBindings(new ViewModel());