/// <reference path="../lodash.js" />
(function () {
    "use strict";
    /*global _ */

    angular.module("deeP")
        .controller("registerBuyerController", ["$location", "navigationUris", "accountService",
            function ($location, navigationUris, accountService) {
                var _this = this;

                _this.user = {
                    username: "",
                    password: "",
                    confirmPassword: "",
                    firstName: "",
                    middleNames: "",
                    lastName: "",
                    email: ""
                };
                _this.error = null;

                // Request buyer registration with current user details
                _this.register = function () {
                    // Copy current user details
                    var currentDetails = _.clone(_this.user);

                    // Send user details to server for registration
                    accountService.registerBuyer(currentDetails).then(
                        function (/*response*/) {
                            // Success; clear previous errors if any and send user to the login view
                            _this.error = null;
                            $location.path(navigationUris.login);
                        },
                        function (error) {
                            // Failure; present error details and reset creds
                            _this.error = error;
                            _this.user.password = "";
                            _this.user.confirmPassword = "";
                        });
                };
            }]);
})();