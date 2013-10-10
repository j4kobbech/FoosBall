var Event = function () { };
Event.prototype = function () {

    // Singleton pattern
    if (Event.prototype._singletonInstance) {
        return Event.prototype._singletonInstance;
    }

    Event.prototype._singletonInstance = this;

    var callbackBus = {};
    var publish = function (eventName) {
        if (!eventName) {
            throw new Error('Missing an "eventName" in event.subscribe()');
        }

        var callbackArguments = [].slice.call(arguments, 1);

        if (!callbackBus[eventName]) {
            callbackBus[eventName] = {
                callbacks: [],
                callbackArguments: [callbackArguments]
            };
        }

        for (var i = 0, l = callbackBus[eventName].callbacks.length; i < l; i++) {
            callbackBus[eventName].callbacks[i].apply(null, callbackArguments);
        }
    };

    var subscribe = function (eventName, eventCallback) {
        if (!eventName || !eventCallback) {
            throw new Error('Missing an "eventName" or an "eventCallback" in event.subscribe()');
        }

        if (!callbackBus[eventName]) {
            callbackBus[eventName] = {
                callbacks: [eventCallback],
                callbackArguments: []
            };
        } else {
            callbackBus[eventName].callbacks.push(eventCallback);
        }

        for (var i = 0, l = callbackBus[eventName].callbackArguments.length; i < l; i++) {
            eventCallback.apply(null, callbackBus[eventName].callbackArguments[i]);
            callbackBus[eventName].callbackArguments.pop();
        }
    };

    var unsubscribe = function (eventName, eventCallback) {
        var index;

        if (!eventName) {
            return;
        }

        if (!eventCallback) {
            callbackBus = {};
        } else {
            index = callbackBus[eventName].callbacks.indexOf(eventCallback);
            if (index > -1) {
                callbackBus[eventName].callbacks.slice(0, index).concat(callbackBus[eventName].callbacks.slice(index + 1));
            }
        }
    };

    return {
        publish: publish,
        subscribe: subscribe,
        unsubscribe: unsubscribe
    };
}();
var event = new Event();