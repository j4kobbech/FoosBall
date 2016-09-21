FoosBall.controller('CreateTeamController', ['$scope', '$resource', function ($scope, $resource) {
    var valueBeforeChange;

    $scope.team = {
        Name: '',
        Team: {
            Players: [{}, {}]
        }
    };

    $scope.createTeam = function () {
        var Team = $resource('Teams/CreateTeamName');
        var promise = Team.save($scope.team).$promise;

        promise.then(function (response) {
            resetTeamForm($scope);
            if (response.success) {
                $scope.teams.unshift(response.newTeam);
            }
        });
    };

    // Start fetching players, return a promise
    $scope.getPlayers = function () {
        var Players = $resource('Players/GetActivePlayers');
        var promise = Players.query().$promise;

        promise.then(function (players) {
            $scope.players = players;
            $scope.matchesDataReady = true;
        });
    };
    $scope.getPlayers();

    $scope.onPlayerSelectFocus = function (event) {
        $thisSelect = $(event.target);
        valueBeforeChange = $thisSelect.find(':selected').val();
    };

    $scope.onPlayerSelectChange = function () {
        var $thisOption = $thisSelect.find(':selected');
        // reset options 
        $.each($('option[value="' + valueBeforeChange + '"]').not($thisOption), function (idx, element) {
            $(element).removeAttr('disabled');
        });
        // if the chosen option is default (empty)
        if (!$thisOption.val() === false) {
            $.each($('option[value="' + $thisOption.val() + '"]').not($thisOption), function (idx, element) {
                $(element).attr('disabled', 'disabled');
            });
        }
        valueBeforeChange = $thisSelect.find(':selected').val();
    };

    function resetTeamForm(scope) {
        angular.forEach(scope.team, function (value, key) {
            scope.team[key] = null;
        });
    }
}]);