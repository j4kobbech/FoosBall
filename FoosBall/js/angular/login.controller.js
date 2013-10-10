function LoginController($scope) {
    $scope.submitLogin = function () {
        var requestParameters = {
            email: $scope.email,
            password: $scope.password,
            rememberMe: $scope.rememberMe || false
        };

        var requestConfig = {
            url: 'Account/LogOn',
            method: 'post',
            data: requestParameters,
            success: function (loginInfo) {
                $scope.$apply(function() {
                    angular.forEach(loginInfo.Data, function (value, key) {
                        $scope.session[key] = value;
                    });
                    $scope.uiSettings.hideMainMenu = true;
                    $scope.uiSettings.hideLogonMenu = true;
                });
            }
        };

        $.ajax(requestConfig);
    };
}

LoginController.$inject = ['$scope'];
