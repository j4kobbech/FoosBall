function MatchesController($scope, $resource) {
    $scope.pageSize = 10;
    $scope.matches = [];
    
    // Groom the data
    function prepareData(matches) {
        angular.forEach(matches, function (value, key) {
            var time = parseInt(value.GameOverTime.replace(/\D/g, ""));
            var date = new Date(time);

            matches[key].DistributedRating = value.DistributedRating.toString().replace(/(\d{1,2})(\.)(\d{2})(.*)/g, "$1,$3");
            matches[key].GameOverDate = date.toDateString().replace(/(\w{3}) (\w{3}) (\d{2}) (\d{2})(\d{2})/g, date.getDate() + " $2 " + " $5");
            matches[key].GameOverTime = date.toLocaleTimeString().replace(/(\d{2})(\.)(\d{2})(\.)(\d{2})/g, ", $1:$3");
        });

        return matches;
    }
    
    // Start fetching matches, return a promise
    $scope.getMatches = function(numberOfMatches, startFromMatch) {
        var num = numberOfMatches || $scope.pageSize;
        var start = startFromMatch || 0;

        var Matches = $resource('Matches/GetMatches?numberOfMatches=' + num + '&startFromMatch=' + start);
        var promise = Matches.query().$promise;

        promise.then(function(matches) {
            var groomedMatches = prepareData(matches);

            angular.forEach(groomedMatches, function(value, key) {
                $scope.matches.push(value);
            });

            $scope.hex_md5 = hex_md5;
        });
    };
    
    $scope.getMatches($scope.pageSize, $scope.matches.length);
}
