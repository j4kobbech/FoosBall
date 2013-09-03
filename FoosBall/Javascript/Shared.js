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