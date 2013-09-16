function MatchesController($scope, $resource) {
    var $thisSelect,
        valueBeforeChange,
        currentUserTag = document.getElementById('current-user-id');

    $scope.pageSize = 10;
    $scope.hideForm = true;
    $scope.matches = [];
    $scope.hex_md5 = hex_md5;
    $scope.currentUserId = currentUserTag ? currentUserTag.value : '';

    $scope.cancelMatch = function(index) {
        var match = $scope.matches[index],
            CancelMatch = $resource('Matches/Delete?id=' + match.Id),
            promise = CancelMatch.save().$promise;

        promise.then(function() {
            $scope.matches.splice(index, 1);
            var ReplayMatches = $resource('Admin/ReplayMatches'),
            replayPromise = ReplayMatches.save().$promise;

            replayPromise.then(function () {
                
            });
        });
    };

    $scope.onPlayerSelectFocus = function(event) {
        $thisSelect = $(event.target);
        valueBeforeChange = $thisSelect.find(':selected').val();
    };

    $scope.onPlayerSelectChange = function() {
        var $thisOption = $thisSelect.find(':selected');
        // reset options 
        $.each($('option[value="' + valueBeforeChange + '"]').not($thisOption), function(idx, element) {
            $(element).removeAttr('disabled');
        });
        // if the chosen option is default (empty)
        if (!$thisOption.val() === false) {
            $.each($('option[value="' + $thisOption.val() + '"]').not($thisOption), function(idx, element) {
                $(element).attr('disabled', 'disabled');
            });
        }
        valueBeforeChange = $thisSelect.find(':selected').val();
    };

    // Start fetching players, return a promise
    $scope.getPlayers = function () {
        var Players = $resource('Matches/GetPlayers');
        var promise = Players.query().$promise;

        promise.then(function(players) {
            $scope.players = players;
        });
    };

    // Start fetching matches, return a promise
    $scope.getMatches = function (numberOfMatches, startFromMatch) {
        var num = numberOfMatches || $scope.pageSize;
        var start = startFromMatch || 0;
        var Matches = $resource('Matches/GetMatches?numberOfMatches=' + num + '&startFromMatch=' + start);
        var promise = Matches.query().$promise;

        promise.then(function(matches) {
            angular.forEach(matches, function (value, key) {
                var preparedMatch = prepareMatch(matches[key]);
                $scope.matches.push(preparedMatch);
            });
        });
    };

    $scope.getMatches($scope.pageSize, $scope.matches.length);
    $scope.getPlayers();
}

function SubmitMatchController($scope, $resource) {
    $scope.submitMatch = function () {
        var Match = $resource('Matches/SubmitMatch');
        var promise = Match.save($scope.match).$promise;

        promise.then(function (response) {
            // Reset the match form
            angular.forEach($scope.match, function (value, key) {
                $scope.match[key] = null;
            });
            $('option:disabled').removeAttr('disabled');
            $scope.$parent.hideForm = true;

            if (response.success) {
                var preparedMatch = prepareMatch(response.returnedMatch);
                $scope.matches.unshift(preparedMatch);
            }
        });
    };
}

function prepareMatch(match) {
    var time = parseInt(match.GameOverTime.replace(/\D/g, ""));
    var date = new Date(time);

    match.UnixTime = time;
    match.DistributedRating = match.DistributedRating.toString().replace(/(\d{1,2})(\.)(\d{2})(.*)/g, "$1,$3");
    match.GameOverDate = date.toDateString().replace(/(\w{3}) (\w{3}) (\d{2}) (\d{2})(\d{2})/g, date.getDate() + " $2 " + " $5");
    match.GameOverTime = date.toLocaleTimeString().replace(/(\d{2})(\.)(\d{2})(\.)(\d{2})/g, ", $1:$3");
    return match;
}
