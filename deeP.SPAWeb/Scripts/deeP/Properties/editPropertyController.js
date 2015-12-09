(function () {
    "use strict";
    /*global _ */

    angular.module("deePProperties")
        .controller("editPropertyController", ["$scope", "$uibModalInstance", "$timeout", "uiGmapGoogleMapApi", "uuid2", "FileUploader", "propertyServiceUrl", "propertyService", "property",
            function ($scope, $uibModalInstance, $timeout, uiGmapGoogleMapApi, uuid2, FileUploader, propertyServiceUrl, propertyService, property) {
                var _this = this;

                _this.newProperty = !property || !property.id;

                // Take property; create new if none was supplied
                _this.property = property ? _.cloneDeep(property) : { type: 0, bedrooms: 0, imageInfos: [] };
                if (!_this.property.id) {
                    _this.property.id = uuid2.newguid();
                }
                _this.property.locationDetails = _this.property.locationDetails || {};

                _this.ok = function () {

                    if (_this.newProperty) {
                        // Try to add new property
                        propertyService.addProperty(_this.property).then(
                          function (response) {
                              // Success; close dialog
                              $uibModalInstance.close({ property: _this.property });
                          },
                          function (error) {
                              // Failure; present error details
                              _this.error = error;
                          });
                    } else {
                        // Try to edit existing property
                        propertyService.editProperty(_this.property).then(
                          function (response) {
                              // Success; close dialog and return updated property as result
                              $uibModalInstance.close({ property: _this.property });
                          },
                          function (error) {
                              // Failure; present error details
                              _this.error = error;
                          });
                    }
                };

                _this.cancel = function () {
                    $uibModalInstance.dismiss();
                };

                // Initialize address view (and Google map details - there are plenty)
                var map = {
                    control: {},
                    // Default to Europe
                    center: { latitude: 48.5490951, longitude: 11.8266606 },
                    zoom: 5,
                    options: {
                        // _this is to mitigate the issue of over-zooming when setting a single marker
                        maxZoom: 16
                    },
                    markers: [],
                    fit: _this.property.locationDetails.zoom <= 3 && _this.property.locationDetails.markers && _this.property.locationDetails.markers.length > 0,
                    events: {
                        idle: function (/*maps, eventName, args*/) { map.fit = false; }
                    }
                };
                angular.extend(map, _this.property.locationDetails);
                _this.map = map;
                _this.showMap = false;

                // Note: it must be delayed to make sure the event does not fire too early
                uiGmapGoogleMapApi.then(function (maps) {
                    $timeout(function () {
                        var events = {
                            places_changed: function (searchBox) {
                                var places = searchBox.getPlaces();
                                if (places.length === 0) {
                                    _this.map.markers = [];
                                    _this.property.address = "";
                                    return;
                                }

                                // For each place, get the icon, place name, and location.
                                var markers = [];
                                for (var i = 0, place; !!(place = places[i]) ; i++) {
                                    var image = {
                                        url: place.icon,
                                        size: new maps.Size(71, 71),
                                        origin: new maps.Point(0, 0),
                                        anchor: new maps.Point(12, 24),
                                        scaledSize: new maps.Size(25, 25)
                                    };

                                    // Create a marker for each place.
                                    var marker = new maps.Marker({
                                        icon: image,
                                        title: place.name,
                                        position: place.geometry.location,
                                        latitude: place.geometry.location.lat(),
                                        longitude: place.geometry.location.lng(),
                                        addressComponents: place.address_components,
                                        id: i
                                    });

                                    markers.push(marker);
                                }

                                // Cause the markers to be (re-)fit once
                                _this.map.fit = true;
                                _this.map.markers = _.take(markers, 5);

                                // Remember full place name
                                _this.property.address = places[0].formatted_address;
                            }
                        };
                        _this.searchbox = { template: 'searchbox.tpl.html', events: events };

                        // Initiate the addition of the map to the dialog DOM in the next digest cycle
                        $timeout(function () { _this.showMap = true; });
                    });
                });

                _this.uploader = new FileUploader(
                    {
                        url: "/api/images/storeimage",
                        method: "PUT",
                        autoUpload: true
                    });

                _this.uploader.onCompleteItem = function (fileItem, response, status, headers) {
                    if (status === 200) {
                        _this.property.imageInfos.push({ id: response[0], title: response[0], uri: propertyServiceUrl + "api/images/" + response[0] });
                    }
                };

                _this.dropImage = function ($index) {
                    _this.property.imageInfos.splice($index, 1);
                };

                // Setup watch to mirror map into location details
                $scope.$watch(function () { return _this.map; }, function (newMap) {
                    _this.property.locationDetails.center = newMap.center;
                    _this.property.locationDetails.zoom = newMap.zoom;
                    _this.property.locationDetails.markers = newMap.markers;
                }, true);
            }]);
})();