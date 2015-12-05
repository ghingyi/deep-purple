(function () {
    var StatefulFastclick, root,
      extend = function (child, parent) { for (var key in parent) { if (hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
      hasProp = {}.hasOwnProperty;

    StatefulFastclick = (function (superClass) {

        /**
         * the class to be added when the element gets touched
         * @type {String}
         */
        extend(StatefulFastclick, superClass);

        StatefulFastclick.TOUCHEDSTATECLASSNAME = 'fastclick-touched';


        /**
         * the class to be added when the touched element becomes active
         * @type {String}
         */

        StatefulFastclick.ACTIVESTATECLASSNAME = 'fastclick-active';


        /**
         * the name of the initial state
         * this constant is required for the changeState function of the FastClick object
         * @type {Number}
         */

        StatefulFastclick.INITIALSTATE = 0;


        /**
         * the name of the touched state
         * this constant is required for the changeState function of the FastClick object
         * @type {Number}
         */

        StatefulFastclick.TOUCHEDSTATE = 1;


        /**
         * the name of the active state
         * this constant is required for the changeState function of the FastClick object
         * @type {Number}
         */

        StatefulFastclick.ACTIVESTATE = 2;


        /**
         * Constructor
         * @param  {DOMNode} layer  The dom node based on which to track clicks and apply states
         * @param  {Object} options supports the following options
         *                          - tapDelay: minimum time between touchstart and touchend to detect a tap / click
         */

        function StatefulFastclick(layer, options) {
            if (!options) {
                options = {};
            }
            options.tapDelay = 50;
            StatefulFastclick.__super__.constructor.call(this, layer, options);
            if (!options.stateLayer) {
                options.stateLayer = layer;
            }
            this.stateLayer = options.stateLayer;
        }


        /**
         * changes the state of the touched / clicked element to the desired new state
         * available states are (constants):
         * - TOUCHEDSTATE (when an element is touched for the first time)
         * - ACTIVESTATE  (when an element is clicked, i.e. the touch ends a
         *
         * @param  {Number} newState  newState the desired new state as a numeric constant
         */

        StatefulFastclick.prototype.changeState = function (newState) {
            var element, reset;
            element = this.stateLayer;
            reset = (function (_this) {
                return function () {
                    element.classList.remove(_this.constructor.TOUCHEDSTATECLASSNAME);
                    return element.classList.remove(_this.constructor.ACTIVESTATECLASSNAME);
                };
            })(this);
            switch (newState) {
                case this.constructor.TOUCHEDSTATE:
                    return element.classList.add(this.constructor.TOUCHEDSTATECLASSNAME);
                case this.constructor.ACTIVESTATE:
                    return element.classList.add(this.constructor.ACTIVESTATECLASSNAME);
                case this.constructor.INITIALSTATE:
                    return reset();
                default:
                    return reset();
            }
        };

        StatefulFastclick.prototype.resetState = function () {
            return this.changeState(this.constructor.INITIALSTATE);
        };

        StatefulFastclick.prototype.onTouchStart = function (event) {
            var result;
            result = StatefulFastclick.__super__.onTouchStart.call(this, event);
            if (result) {
                return this.changeState(this.constructor.TOUCHEDSTATE);
            }
        };

        StatefulFastclick.prototype.onTouchMove = function (event) {
            var result;
            result = StatefulFastclick.__super__.onTouchMove.call(this, event);
            if (result && !this.trackingClick && !this.targetElement) {
                return this.resetState();
            }
        };

        StatefulFastclick.prototype.onTouchEnd = function (event) {
            var result;
            result = StatefulFastclick.__super__.onTouchEnd.call(this, event);
            if (!this.targetElement && !result) {
                return this.resetState();
            }
        };

        StatefulFastclick.prototype.onTouchCancel = function (event) {
            var result;
            result = StatefulFastclick.__super__.onTouchCancel.call(this, event);
            return this.resetState();
        };

        StatefulFastclick.prototype.onClick = function (event) {
            var permitted;
            permitted = StatefulFastclick.__super__.onClick.call(this, event);
            if (!permitted || (!this.trackingClick && !this.targetElement)) {
                return this.resetState();
            }
        };

        StatefulFastclick.prototype.sendClick = function (targetElement, event) {
            this.changeState(this.constructor.ACTIVESTATE);
            return StatefulFastclick.__super__.sendClick.call(this, targetElement, event);
        };

        return StatefulFastclick;

    })(window.FastClick);

    root = typeof exports !== "undefined" && exports !== null ? exports : window;

    root.StatefulFastclick = StatefulFastclick;

}).call(this);