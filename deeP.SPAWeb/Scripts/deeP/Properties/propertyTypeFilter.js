(function () {
    "use strict";
    angular.module("deePProperties")
       .filter('propertyType', [
           function () {
               function propertyTypeFilter(value) {
                   switch (value) {
                       case 0:
                           return "Flat";
                       case 1:
                           return "House";
                       default:
                           return "Unknown";
                   }
               }

               return propertyTypeFilter;
           }]);
})();