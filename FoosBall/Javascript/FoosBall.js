jQuery(document).ready(function() {
    $.globals = {
        errorState: {}
    };
    
    /* ******************************************************************
     * jQuery Ajax default configuration
     */
    $.ajaxSetup({
        error: function (jqHxr, statusText, errorThrown) {
            displayErrorMessage(statusText + ': ' + errorThrown);
        }
    });
    
    /* ******************************************************************
     * jQuery SignalR Hub Config
     */
    // Declare a proxy to reference the hub. 
    var chat = $.connection.eventHub;

    if (typeof window.webkitNotifications !== 'undefined') {
        if (window.webkitNotifications.checkPermission() === 0) {
            $('body').on('match-resolved', onMatchResolved);
            
        } else {
            if (window.webkitNotifications.checkPermission() === 1) {
                displayRequestForUsingWebkitNotifications();
            }
        }
    }

    // A function that the hub/server can call to broadcast messages.
    chat.client.broadcastMessage = function (eventData) {
        $('body').trigger('match-resolved', eventData);
    };

    // Start the connection.
    $.connection.hub.start();
});

/* ******************************************************************
 * Custom js functions
 */

function prettyPlayerName(playerName) {
    return playerName.indexOf(" ") < 0 ? playerName.length : playerName.indexOf(" ");
}

function buildNotificationText(eventData) {
    if (!eventData) {
        return undefined;
    }
    var matchResult = JSON.parse(eventData);
    
    var redPlayer1ShorteningIndex = prettyPlayerName(matchResult.RedPlayer1);
    var redPlayer2ShorteningIndex = prettyPlayerName(matchResult.RedPlayer2);
    var bluePlayer1ShorteningIndex = prettyPlayerName(matchResult.BluePlayer1);
    var bluePlayer2ShorteningIndex = prettyPlayerName(matchResult.BluePlayer2);

    var redTeam = matchResult.RedPlayer1.substr(0, redPlayer1ShorteningIndex);
    var blueTeam = matchResult.BluePlayer1.substr(0, bluePlayer1ShorteningIndex);

    var redScore = matchResult.RedScore;
    var blueScore = matchResult.BlueScore;

    var outcome = matchResult.RedScore > matchResult.BlueScore ? "\ngave a beating to \n" : "\ngot a beating by \n";
    
    redTeam += (matchResult.RedPlayer2.length === 0) ? "" : " & " + matchResult.RedPlayer2.substr(0, redPlayer2ShorteningIndex);
    blueTeam += (matchResult.BluePlayer2.length === 0) ? "" : " & " + matchResult.BluePlayer2.substr(0, bluePlayer2ShorteningIndex);
    
    return redTeam + outcome + blueTeam + "\n" + redScore + " - " + blueScore;
}

function onMatchResolved(event, eventData) {
    var icon = 'https://s3-eu-west-1.amazonaws.com/images.trustpilot.com/static/foosball/icon_football.png';
    var title = 'FoosBall Fight resolved';
    var body = buildNotificationText(eventData);
    if (!body) {
        return;
    }
    var notification = window.webkitNotifications.createNotification(icon, title, body);
    notification.show();
}

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

// Shorthand logging
function log(str) {
    console.log(str);
}

// Fine grained timing function. Returns time in milliseconds from window.open event is fired
performance.now = (function (window) {
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
    return window.performance.now();
}