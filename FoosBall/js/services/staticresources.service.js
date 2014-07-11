FoosBall.service('staticResources', [function () {

    this.getBackgroundImageUrl = function ($scope) {
        var remoteUrl = 'https://s3-eu-west-1.amazonaws.com/images.trustpilot.com/static/foosball/background.jpg';
        var localUrl = '/css/foosball-background.jpg';

        if (!$scope.appSettings.Environment || $scope.appSettings.Environment === 'Local') {
            return localUrl;
        } else {
            return remoteUrl;
        }
    };

    this.getFootballIconUrl = function ($scope) {
        var remoteUrl = 'https://s3-eu-west-1.amazonaws.com/images.trustpilot.com/static/foosball/icon_football.png';
        var localUrl = '/css/icon_football.png';

        if (!$scope.appSettings.Environment || $scope.appSettings.Environment === 'Local') {
            return localUrl;
        } else {
            return remoteUrl;
        }
    };

    this.getCssUrl = function ($scope) {
        var remoteUrl = '/css/minified/foosball.min.css';
        var localUrl = '/css/minified/foosball.css';

        if (!$scope.appSettings.Environment || $scope.appSettings.Environment === 'Local') {
            return localUrl;
        } else {
            return remoteUrl;
        }
    };
}]);
