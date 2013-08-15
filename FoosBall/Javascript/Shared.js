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