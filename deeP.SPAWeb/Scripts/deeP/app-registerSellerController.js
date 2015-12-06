/// <reference path="../lodash.js" />
(function () {
    "use strict";
    /*global _ */

    angular.module("deeP")
        .controller("registerSellerController", ["$location", "navigationUris", "accountService",
            function ($location, navigationUris, accountService) {
                this.user = {
                    username: "",
                    password: "",
                    confirmPassword: "",
                    firstName: "",
                    middleNames: "",
                    lastName: "",
                    email: ""
                };
                this.error = null;

                // Request seller registration with current user details
                this.register = function () {
                    var _this = this;

                    // Copy current user details
                    var currentDetails = _.clone(_this.user);

                    // Send user details to server for registration
                    accountService.registerSeller(currentDetails).then(
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