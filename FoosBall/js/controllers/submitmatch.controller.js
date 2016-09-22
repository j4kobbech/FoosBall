FoosBall.controller('SubmitMatchController', ['$scope', '$resource', function ($scope, $resource) {

    $scope.submitMatch = function () {
        var Match = $resource('Matches/SubmitMatch');
        var promise = Match.save($scope.match).$promise;

        promise.then(function (response) {
            resetMatchForm($scope);
            if (response.success) {
                var preparedMatch = prepareMatch(response.returnedMatch);
                $scope.matches.unshift(preparedMatch);
            }
        });
    };

    function resetMatchForm(scope) {
        $('.score-prediction').find('.rating, .chance, .gain').text('');
        angular.forEach(scope.match, function (value, key) {
            scope.match[key] = null;
        });
        $('option:disabled').removeAttr('disabled');
    }
}]);