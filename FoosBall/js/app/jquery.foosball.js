jQuery(document).ready(function () {
    $.globals = {
        notificationTimeout: null,
    };
    
    autoResizeBackgroundImage();
});

function nvl(nullValue, replace) {
    return nullValue ? nullValue : replace;
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