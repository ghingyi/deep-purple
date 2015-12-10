(function () {
    "use strict";
    angular.module("deePProperties")
       .filter('bidStatus', [
           function () {
               function bidStatusFilter(value) {
                   switch (value) {
                       case 0:
                           return "Open";
                       case 1:
                           return "Accepted";
                       case 2:
                           return "Rejected";
                       default:
                           return "Unknown";
                   }
               }

               return bidStatusFilter;
           }]);
})();