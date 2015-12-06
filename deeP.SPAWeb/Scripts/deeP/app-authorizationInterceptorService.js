(function () {
    "use strict";
    angular.module("deePAuth")
        .provider("authorizationInterceptorService", [
            function () {
                this.$get = ["$q", "$location", "navigationUris", "accountContext",
                    function ($q, $location, navigationUris, accountContext) {
                        // Request access token augmentation handler
                        var _request = function (config) {

                            if (accountContext && accountContext.accessToken) {
                                config.headers = config.headers || {};
                                config.headers.Authorization = "Bearer " + accountContext.accessToken;
                            }

                            return config;
                        };

                        // Login redirection handler for 401 responses
                        var _responseError = function (rejection) {
                            if (rejection.status === 401) {
                                $location.path(navigationUris.login);
                            }
                            return $q.reject(rejection);
                        };

                        return {
                            request: _request,
                            responseError: _responseError
                        };
                    }];
            }]);
})();