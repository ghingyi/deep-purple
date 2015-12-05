(function () {
    "use strict";

    angular.module("deeP", ["ngRoute", "ngTouch", "ui.bootstrap", "angularMoment", "angularUUID2", "blockUI", "uiGmapgoogle-maps", "Chronicle", "as.sortable", "angularFileUpload"])
        .config(["$routeProvider", "$locationProvider", "uiGmapGoogleMapApiProvider",
        function ($routeProvider, $locationProvider, uiGmapGoogleMapApiProvider) {

            // Setup routes
            $routeProvider.
                when("/", {
                    templateUrl: "/content/templates/views/app-default.html",
                    controller: "deePDefaultController"
                }).
                otherwise({
                    redirectTo: "/"
                });

            // Enable HTML5 mode processing of URIs
            $locationProvider.html5Mode(true).hashPrefix('!');

            // Initialize Google Maps
            uiGmapGoogleMapApiProvider.configure({
                //v: "3.17",
                libraries: "places"/*,
                china: false*/
            });
        }]);
})();