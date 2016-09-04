FoosBall.controller('TeamsController', ['$scope', '$resource', function($scope, $resource) {
    $scope.teams = [];
    $scope.teamsDataReady = false;
    $scope.hideForm = true;

    // Start fetching players, return a promise
    $scope.getTeams = function () {
        var Teams = $resource('Teams/GetTeams');
        var promise = Teams.query().$promise;

        promise.then(function (teams) {
            $scope.teams = teams;
            $scope.teamsDataReady = true;
        });
    };

    $scope.getTeams();
}]);
