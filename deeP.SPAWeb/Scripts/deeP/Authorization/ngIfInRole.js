/// <reference path="../angular.js" />
/// <reference path="../lodash.js" />
(function () {
    "use strict";
    /*global _ */
    /*global jqLite */

    // Directive to add or remove DOM elements based on a user's role membership
    // Note: it's largely based on ngIf; the goal is to have a simplier syntax
    // and not having to depend on accountContext everywhere we want this
    angular.module("deePAuth")
    .directive("ngIfInRole",
        ["$animate", "$parse", "accountContext", function ($animate, $parse, accountContext) {

            /**
             * Return the DOM siblings between the first and last node in the given array.
             * @param {Array} array like object
             * @returns {Array} the inputted object or a jqLite collection containing the nodes
             */
            function getBlockNodes(nodes) {
                // TODO(perf): update `nodes` instead of creating a new object?
                var node = nodes[0];
                var endNode = nodes[nodes.length - 1];
                var blockNodes;

                for (var i = 1; node !== endNode && (node = node.nextSibling) ; i++) {
                    if (blockNodes || nodes[i] !== node) {
                        if (!blockNodes) {
                            blockNodes = jqLite([].slice.call(nodes, 0, i));
                        }
                        blockNodes.push(node);
                    }
                }

                return blockNodes || nodes;
            }

            return {
                multiElement: true,
                transclude: 'element',
                priority: 600,
                terminal: true,
                restrict: 'A',
                $$tlb: true,
                link: function ($scope, $element, $attr, ctrl, $transclude) {
                    var block, childScope, previousElements;
                    $scope.$watch(function () { return $parse($attr.ngIfInRole)($scope); }, function ngIfInRoleWatchAction(value) {

                        if (value && accountContext &&
                            accountContext.roles && _.any(accountContext.roles, function (item) { return item === value; })) {
                            if (!childScope) {
                                $transclude(function (clone, newScope) {
                                    childScope = newScope;
                                    clone[clone.length++] = document.createComment(' end ngIfInRole: ' + $attr.ngIfInRole + ' ');
                                    // Note: We only need the first/last node of the cloned nodes.
                                    // However, we need to keep the reference to the jqlite wrapper as it might be changed later
                                    // by a directive with templateUrl when its template arrives.
                                    block = {
                                        clone: clone
                                    };
                                    $animate.enter(clone, $element.parent(), $element);
                                });
                            }
                        } else {
                            if (previousElements) {
                                previousElements.remove();
                                previousElements = null;
                            }
                            if (childScope) {
                                childScope.$destroy();
                                childScope = null;
                            }
                            if (block) {
                                previousElements = getBlockNodes(block.clone);
                                $animate.leave(previousElements).then(function () {
                                    previousElements = null;
                                });
                                block = null;
                            }
                        }
                    });
                }
            };
        }]);
})();