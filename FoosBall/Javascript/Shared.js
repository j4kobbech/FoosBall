jQuery(document).ready(function() {
    // ******************************************************************
    // Menu highlighting and animation 
    //
    var activeTab = $('#page').attr('class');
    var $menu = $('#main-menu');
    var $menuarrow = $('.menu-arrow');

    if (!!activeTab !== false) {
        $menu.find('li').removeClass('selected');
        $('.' + activeTab, $menu).addClass('selected');
    }

    $('.menu-list-button').on('click', function (e) {
        e.preventDefault();
        $menu.toggle();
        $menuarrow.toggle();
    });

    $menu.on('click', 'li', function (event) {
        event.preventDefault();
        var redirectUrl;
        var $target = $(event.target);

        if ($target.is('li')) {
            redirectUrl = $target.children('a').attr('href');
        } else if ($target.is('a')) {
            redirectUrl = $target.attr('href');
        }

        if (redirectUrl) {
            window.location = redirectUrl;
        }
    });
});

function toggleOverlay() {
    var overlay = $('#overlay');

    if (overlay.size() === 0) {

        var htmlHeight = $('html').innerHeight();
        $('body').append('<div id="overlay"></div>');

        overlay = $('#overlay');

        overlay.css({
            'height': htmlHeight + 'px',
            'background-color': 'rgba(0, 0, 0, 0.6)',
            'left': '0',
            'position': 'absolute',
            'top': '0',
            'width': '100%',
            'z-index': '1000',
        });

        overlay.append('<img src="https://s3-eu-west-1.amazonaws.com/images.trustpilot.com/static/foosball/ajax-loader.gif"/>');

        overlay.on('click', function () {
            toggleOverlay();
        });

    } else {
        overlay.remove();
    }
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
    // Fix to avoid duplicate mysterious notifications
    if (!$.globals.notificationTimeout) {
        var icon = 'https://s3-eu-west-1.amazonaws.com/images.trustpilot.com/static/foosball/icon_football.png';
        var title = 'FoosBall Fight resolved';
        var body = buildNotificationText(eventData);
        if (!body) {
            return;
        }
        var notification = window.webkitNotifications.createNotification(icon, title, body);
        $.globals.notificationTimeout = setTimeout(function() {
            notification.show();
            $.globals.notificationTimeout = null;
        }, 500);
    }
}
