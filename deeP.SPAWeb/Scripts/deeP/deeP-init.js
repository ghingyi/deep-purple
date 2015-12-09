(function () {
    "use strict";

    // Bootstrap application module
    var deePapp = angular.module("deeP",
        ["ngRoute", "ngTouch", "ui.bootstrap", "LocalStorageModule", "angularMoment", "angularUUID2", "blockUI", "uiGmapgoogle-maps", "Chronicle", "as.sortable", "angularFileUpload",
        "deePConfig", "deePAuth", "deePProperties"]);

    deePapp.config(["$routeProvider", "$locationProvider", "$httpProvider", "uiGmapGoogleMapApiProvider", "localStorageServiceProvider", "navigationUris",
    function ($routeProvider, $locationProvider, $httpProvider, uiGmapGoogleMapApiProvider, localStorageServiceProvider, navigationUris) {

        // Setup routes
        $routeProvider.
            when(navigationUris.home, {
                templateUrl: "/content/templates/views/default.html",
                controller: "defaultController",
                controllerAs: "defaultCtrl",
                resolve: {
                    factory: ["$q", "$location", "accountContext", "navigationUris", userHomeRedirection]
                }
            }).
            when(navigationUris.login, {
                templateUrl: "/content/templates/views/login.html",
                controller: "loginController",
                controllerAs: "loginCtrl"
            }).
            when(navigationUris.registerSeller, {
                templateUrl: "/content/templates/views/register-seller.html",
                controller: "registerSellerController",
                controllerAs: "registerCtrl"
            }).
            when(navigationUris.registerBuyer, {
                templateUrl: "/content/templates/views/register-buyer.html",
                controller: "registerBuyerController",
                controllerAs: "registerCtrl"
            }).
            when(navigationUris.user, {
                templateUrl: "/content/templates/views/user.html",
                controller: "userController",
                controllerAs: "userCtrl",
                resolve: {
                    factory: ["$q", "$location", "accountContext", "navigationUris", authorizeRouting]
                }
            }).
            when(navigationUris.propertyDetails + "/:propertyId", {
                templateUrl: "/content/templates/views/propertyDetails.html",
                controller: "propertyDetailsController",
                controllerAs: "propertyDetailsCtrl",
                resolve: {
                    factory: ["$q", "$location", "accountContext", "navigationUris", authorizeRouting]
                }
            }).
            otherwise({
                redirectTo: navigationUris.home
            });

        // Enable HTML5 mode processing of URIs
        $locationProvider.html5Mode(true).hashPrefix("!");

        // Register request interceptor to include access token and redirect on failure
        $httpProvider.interceptors.push("authorizationInterceptorService");

        // Initialize Google Maps
        uiGmapGoogleMapApiProvider.configure({
            //v: "3.17",
            libraries: "places"/*,
                china: false*/
        });

        // Setup local storage
        localStorageServiceProvider.setPrefix("deeP");
        // Use local storage (as opposed to session) - this is the default
        //localStorageServiceProvider.setStorageType("localStorage");
        // Set cookie fallback info: 30 days for root - this is the default
        //localStorageServiceProvider.setStorageCookie(30, "/");
        // Disable events on change to locale storage
        localStorageServiceProvider.setNotify(false, false);
    }])
    .run(["$rootScope", "localStorageService", "accountContext",
        function ($rootScope, localStorageService, accountContext) {

            // Restore account context on SPA start
            // Note: it doesn't care whether the access token is still valid
            // In this sample we will rely on the interceptor to force a new login when needed...
            var ac = localStorageService.get("_ac");
            if (ac) {
                angular.extend(accountContext, ac);
            }

            // Save account context when changes (logout, login)
            $rootScope.$watch(function () { return accountContext; }, function (newAccountContext) {
                localStorageService.set("_ac", newAccountContext);
            },
            true);
        }]);

    var authorizeRouting = function ($q, $location, accountContext, navigationUris) {
        if (accountContext.username) {
            return true;
        } else {
            $location.path(navigationUris.login);
            return $q.reject(false);
        }
    };

    var userHomeRedirection = function ($q, $location, accountContext, navigationUris) {
        if (accountContext.username) {
            $location.path(navigationUris.user);
        } else {
            return false;
        }
    };
})();