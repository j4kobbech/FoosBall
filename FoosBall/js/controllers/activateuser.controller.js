FoosBall.controller('ActivateUserController', ['$scope', '$location', 'api', function ($scope, $location, api) {

    var queryString = $location.search();

    var promiseOfUserActivation = api.activateUser(queryString.userId, queryString.token);

    promiseOfUserActivation.then(function(response) {
        console.log(response);
    });

}]);
