FoosBall.service('staticResources', [function () {

    this.getBackgroundImageUrl = function (appSettings) {
        var url = '/css/images/' + appSettings.SportName.toLowerCase() + '-background.jpg';
        return url;
    };

    this.getIconUrl = function (appSettings) {
        var url = '/css/images/' + appSettings.SportName.toLowerCase() + '-icon.png';
        return url;
    };
}]);
