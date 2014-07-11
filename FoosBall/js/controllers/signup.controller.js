FoosBall.controller('SignupController', ['$scope', '$resource', '$location', 'session', function ($scope, $resource, $location, session) {
    $scope.signupMessage = "";
    $scope.showSignupMessage = false;

    $scope.submitSignup = function() {
        var requestParameters = {
            email: $scope.email,
            name: $scope.name,
            password: $scope.password
        };

        var User = $resource('Account/Register');
        var newUser = new User(requestParameters);
        var newUserPromise = newUser.$save();

        newUserPromise.then(function(signupResponse) {
            if (signupResponse && signupResponse.Success) {
                session.authenticateUser({
                    email: $scope.email,
                    password: $scope.password,
                    rememberMe: $scope.rememberMe || false
                }).then(function () {
                    $scope.session.isLoggedIn = session.isLoggedIn();
                    $scope.session.userName = signupResponse.Data.AccessToken.UserName;
                    $location.path('/');
                });
            } else {
                $scope.signupMessage = signupResponse.Message;
                $scope.showSignupMessage = true;
            }

            clearSignupForm($scope);
        });
    };
    
    function clearSignupForm(scope) {
        scope.signupMessage = "";
        scope.showSignupMessage = false;
        scope.email = "";
        scope.name = "";
        scope.password = "";
    }
}]);
