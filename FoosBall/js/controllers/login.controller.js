FoosBall.controller('LoginController', ['$scope', 'session', '$location', function ($scope, session, $location) {
    $scope.loginMessage = "";
    $scope.showLoginMessage = false;

    $scope.submitLogin = function() {
        var authPromise = session.authenticateUser({
            email: $scope.email,
            password: $scope.password,
            rememberMe: $scope.rememberMe || false
        });

        authPromise.then(function (authenticateResponse) {
            if (authenticateResponse && authenticateResponse.Success) {
                $scope.session.isLoggedIn = session.isLoggedIn();
                $scope.session.userName = authenticateResponse.Data.AccessToken.UserName;
                clearLogonForm($scope);

                $location.path('/');
            } else {
                $scope.loginMessage = authenticateResponse.Message;
                $scope.showLoginMessage = true;
            }
        });
    };
    
    function clearLogonForm(scope) {
        scope.loginMessage = "";
        scope.showLoginMessage = false;
        scope.email = "";
        scope.name = "";
        scope.password = "";
    }
}]);