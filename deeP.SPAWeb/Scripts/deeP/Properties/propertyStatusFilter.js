(function () {
    "use strict";
    angular.module("deePProperties")
       .filter('propertyStatus', [
           function () {
               function propertyStatusFilter(value) {
                   switch (value) {
                       case 0:
                           return "Available";
                       case 1:
                           return "Taken";
                       default:
                           return "Unknown";
                   }
               }

               return propertyStatusFilter;
           }]);
})();