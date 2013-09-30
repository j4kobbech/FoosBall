function StatsController($scope, $resource) {
    $scope.stats = [];
    $scope.hex_md5 = hex_md5;
    
    // Start fetching statistics, return a promise
    $scope.getStats = function () {
        var Stats = $resource('Stats/GetStatistics');
        var promise = Stats.get().$promise;

        promise.then(function (stats) {
            $scope.stats = prepareStats(stats);
        });
    };

    $scope.getPlayerStatsUrl = function (playerId) {
        return '/#/playerstats?playerId=' + playerId;
    };

    $scope.getStats();

    function prepareStats(stats) {
        stats.MostFights.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.MostFights.Email) + '?d=mm';
        stats.MostWins.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.MostWins.Email) + '?d=mm';
        stats.MostLosses.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.MostLosses.Email) + '?d=mm';
        stats.TopRanked.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.TopRanked.Email) + '?d=mm';
        stats.BottomRanked.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.BottomRanked.Email) + '?d=mm';
        stats.HighestRatingEver.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.HighestRatingEver.Email) + '?d=mm';
        stats.LowestRatingEver.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.LowestRatingEver.Email) + '?d=mm';
        stats.LongestWinningStreak.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.LongestWinningStreak.Player.Email) + '?d=mm';
        stats.LongestLosingStreak.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(stats.LongestLosingStreak.Player.Email) + '?d=mm';

        return stats;
    }
}

StatsController.$inject = ['$scope', '$resource'];
