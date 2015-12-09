(function () {
    "use strict";
    angular.module("deePProperties")
        .factory("propertyService", ["$http", "propertyServiceUrl",
            function ($http, propertyServiceUrl) {

                // Adds a property
                var _addProperty = function (property) {
                    return $http.post(propertyServiceUrl + "api/properties/addproperty", property);
                };

                // Adds a property
                var _editProperty = function (property) {
                    return $http.post(propertyServiceUrl + "api/properties/editproperty", property);
                };

                // Adds a bid for a property
                var _addBid = function (bid) {
                    return $http.post(propertyServiceUrl + "api/properties/addbid", bid);
                };

                // Accept or reject a bid
                var _closeBid = function (bid, accept) {
                    var closeBidModel = {
                        bid: bid,
                        accept: accept
                    };
                    return $http.post(propertyServiceUrl + "api/properties/closebid", closeBidModel);
                };

                // Query for properties that match the filter
                // Notes: typically used either to query properties listed by a seller or
                // to query properties that are still open for bid by buyers
                var _queryProperties = function (propertyFilter, propertySorting) {
                    var queryPropertiesModel = {
                        filter: propertyFilter,
                        sorting: propertySorting
                    };
                    return $http.post(propertyServiceUrl + "api/properties/queryproperties", queryPropertiesModel);
                };

                // Query for bids that match the filter
                // Notes: typically used either to query bids posted by a given buyer or
                // to query open bids posted for any of the properties of a given seller
                var _queryBids = function (bidFilter) {
                    return $http.post(propertyServiceUrl + "api/properties/querybids", bidFilter);
                };

                return {
                    addProperty: _addProperty,
                    editProperty: _editProperty,
                    addBid: _addBid,
                    closeBid: _closeBid,
                    queryProperties: _queryProperties,
                    queryBids: _queryBids
                };
            }]);
})();