jQuery(document).ready(function () {
    $.globals = {
        errorState: {},
        notificationTimeout: null,
    };
    
    /* ******************************************************************
     * jQuery Ajax default configuration
     */
    $.ajaxSetup({
        error: function (jqHxr, statusText, errorThrown) {
            displayErrorMessage(statusText + ': ' + errorThrown);
        }
    });

    initializeSignlRHub();
    showDesktopNotificationRequest();
    autoResizeBackgroundImage();
    autoLogin();
});

/* ******************************************************************
 * Custom js functions
 */

function displayRequestForUsingWebkitNotifications() {
    var $requestDiv = $('#request-notification');
    $requestDiv.slideDown(250, function() {
        var $closeNotification = $requestDiv.find('#notify-me');

        $closeNotification.on('click', function () {
            window.webkitNotifications.requestPermission();
            $requestDiv.slideUp(250);
        });
    });
}

function displayErrorMessage(errorMessage, selector) {
    $.globals.errorState[selector] = true;
    var $container = (!!selector === true) ? $(".validation-message." + selector) : $(".validation-message.All");
    if ($container.size() !== false) {
        $container.html(errorMessage).show();
    } else {
        alert(selector + " error: " + errorMessage);
    }
}

function clearErrorMessage(selector) {
    $.globals.errorState[selector] = false;
    var $container = (!!selector === true) ? $(".validation-message." + selector) : $(".validation-message");
    $container.html("").hide();
}


function errorState() {
    var state = false;

    $.each($.globals.errorState, function (key, value) {
        if (value) {
            return state = true;
        }
        return undefined;
    });
    
    return state;
}

function nvl(nullValue, replace) {
    return nullValue ? nullValue : replace;
}
 
// Shorthand logging
function log(str, timer) {
    var returnedString = undefined;
    
    if (typeof timer === "boolean" && timer === true) {
        returnedString = now() + ": " + str;
    } else {
        returnedString = str;
    }
    console.log(returnedString);
}

// Fine grained timing function. Returns time in milliseconds from window.open event is fired
window.performance.now = (function (window) {
    return window.performance.now ||
           window.performance.mozNow ||
           window.performance.msNow ||
           window.performance.oNow ||
           window.performance.webkitNow ||
           function () {
               return new Date().getTime();
           };
})(window);

// Shorthand method for window.performance.now()
function now() {
    return parseInt(window.performance.now());
}

function autoLogin() {
    $.ajax({
        url: '/Account/Logon',
        success: function () {
            $.ajax({
                url: '/Account/GetLogonStatus',
                success: function (updatedHtml) {
                    $('.login-display').html(updatedHtml);
                }
            });
        }
    });
}

function autoResizeBackgroundImage() {
    $(window).load(function () {
        var $window = $(window),
            $bg = $("#bg"),
            aspectRatio = $bg.width() / $bg.height();

        function resizeBg() {
            if (($window.width() / $window.height()) < aspectRatio) {
                $bg.removeClass().addClass('bgheight');
            } else {
                $bg.removeClass().addClass('bgwidth');
            }
        }

        $window.resize(function () {
            resizeBg();
        }).trigger("resize");
    });
}

function showDesktopNotificationRequest() {
    if (typeof window.webkitNotifications !== 'undefined') {
        if (window.webkitNotifications.checkPermission() === 0) {
            $('body').on('match-resolved', onMatchResolved);

        } else {
            if (window.webkitNotifications.checkPermission() === 1) {
                displayRequestForUsingWebkitNotifications();
            }
        }
    }
}

function initializeSignlRHub() {
    // Declare a proxy to reference the hub. 
    var chat = $.connection.eventHub;

    // A function that the hub/server can call to broadcast messages.
    chat.client.broadcastMessage = function (eventData) {
        $('body').trigger('match-resolved', eventData);
    };

    // Start the connection.
    $.connection.hub.start();
}