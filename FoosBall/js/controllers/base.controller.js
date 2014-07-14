FoosBall.controller('BaseController', ['$scope', 'session', 'appSettings', 'staticResources', function ($scope, session, appSettings, staticResources) {
    $scope.session = {
        isLoggedIn: session.isLoggedIn(),
        userName: session.getSessionUserName()
};
    $scope.appSettings = {};
    $scope.staticResources = {};

    session.refreshRequestHeaders();

    $scope.logout = function () {
        session.logout().then(function() {
            $scope.session.isLoggedIn = session.isLoggedIn();
        });
    }

    var promiseOfAppSettings = appSettings.getAppSettings();
    promiseOfAppSettings.then(function(response) {
        $scope.appSettings = response;
        $scope.appSettings.ready = true;
        $scope.appSettings.AppNameWithEnvironment = getAppNameWithEnvironment($scope.appSettings.AppName, $scope.appSettings.Environment);

        $scope.staticResources.backgroundImageUrl = staticResources.getBackgroundImageUrl($scope.appSettings);
        $scope.staticResources.iconUrl = staticResources.getIconUrl($scope.appSettings);
    });

    function getAppNameWithEnvironment(appName, environment) {
        var env = environment.toLowerCase() === 'production' ? "" : (environment + " ");
        return (env + appName);
    }
}]);
