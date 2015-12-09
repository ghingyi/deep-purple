
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("propertyListController", ["$scope", "accountContext", "propertyService",
            function ($scope, accountContext, propertyService) {
                var _this = this;

                _this.filter = { type: null, minBedrooms: 0 }
                _this.filterIsDirty = false;
                _this.sorting = 0;

                _this.properties = null;

                _this.refreshList = function (restricted) {
                    // Restricted means that we only want to get properties that the user can edit.
                    // However, fooling this will not mean much; the server does authrize edit requests separately.
                    var currentFilter = _.clone(_this.filter);
                    if (restricted) {
                        currentFilter.sellerName = accountContext.username;
                    }

                    propertyService.queryProperties(currentFilter, _this.sorting).then(
                        function (result) {
                            // Note: in this sample, we'll not implement paging; however, the API method and the repository supports skip and take parameters
                            _this.properties = result.data;
                            _this.filterIsDirty = false;
                        },
                        function (error) {
                            // User alert and offline trace collection would be better
                            console.error(error);
                        });
                };

                $scope.$on("refreshPropertyList", function ($e, restrict) {
                    _this.refreshList(restrict);
                });

                $scope.$watch(function () { return _this.filter },
                    function () {
                        _this.filterIsDirty = true;
                    }, true);
            }]);
})();