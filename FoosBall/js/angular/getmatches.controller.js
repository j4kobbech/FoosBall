// Test Get All Matches
function GetMatchesController($scope, api) {
    // Define an array for the matches in the scope
    $scope.matches = [];
    
    // Get the promise of all matches from the api
    var promiseOfMatches = api.getAllMatches();

    // When matches are delivered assign the matches to the scope 
    promiseOfMatches.then(function(arrayOfAllMatches) {
        $scope.matches = arrayOfAllMatches;
    });
}

GetMatchesController.$inject = ['$scope', 'api'];