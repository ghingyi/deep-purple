(function () {
    "use strict";
    /*global _ */
    angular.module("deePProperties")
        .controller("placeBidController", ["$uibModalInstance", "uuid2", "propertyService", "property",
            function ($uibModalInstance, uuid2, propertyService, property) {
                var _this = this;

                if (!property)
                    throw Error("A property must be supplied in order to be able to place a bid.");

                // Note: this could go into a localization service
                var getBidTitle = function (property) {
                    var propertyType;
                    switch (property.type) {
                        case 0:
                            propertyType = "flat";
                            break;
                        case 1:
                            propertyType = "house";
                            break;
                        default:
                            propertyType = "property";
                            break;
                    }

                    return property.bedrooms + ' bedroom ' + propertyType + ' at ' + property.address.substring(0, Math.min(property.address.length, 100));
                };

                _this.bid = {
                    id: uuid2.newguid(),
                    propertyId: property.id,
                    title: getBidTitle(property)
                };

                _this.ok = function () {
                    // Try to add new property
                    propertyService.addBid(_this.bid).then(
                      function (response) {
                          // Success; close dialog
                          $uibModalInstance.close({ bid: _this.bid });
                      },
                      function (error) {
                          // Failure; present error details
                          _this.error = error;
                      });
                };

                _this.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
})();