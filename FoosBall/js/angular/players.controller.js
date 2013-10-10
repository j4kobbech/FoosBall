function PlayersController($scope, $resource) {
    $scope.players = [];
    
    // Start fetching players, return a promise
    $scope.getPlayers = function () {
        var Players = $resource('Players/GetPlayers');
        var promise = Players.query().$promise;

        promise.then(function (players) {
            var preparedPlayer;
            
            angular.forEach(players, function (player, index) {
                preparedPlayer = preparePlayer(player, index);
                $scope.players.push(preparedPlayer);
            });
        });
    };

    $scope.getPlayers();

    function preparePlayer(player, index) {
        player.GravatarUrl = 'http://www.gravatar.com/avatar/' + md5.hex_md5(player.Email) + '?d=mm';
        player.Ranking = index + 1;
        player.Rating1 = parseFloat(player.Rating).toFixed(0);
        player.Rating2 = parseFloat(player.Rating).toFixed(2);
        player.Ratio1 = parseFloat(player.Ratio).toFixed(2);
        player.Ratio2 = parseFloat(player.Ratio).toFixed(4);

        return player;
    }
}

PlayersController.$inject = ['$scope', '$resource'];
