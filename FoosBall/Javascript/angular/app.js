angular.module('FoosBall', ['ngResource'])
    .directive('foosballScore', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    var otherScore = document.querySelector('.team-score:not(#' + elm.attr('id') + ')');
                    
                    if (!viewValue || !otherScore.value || viewValue === otherScore.value) {
                        scope.resolveMatchForm.redScore.$setValidity('foosballScore', false);
                        scope.resolveMatchForm.blueScore.$setValidity('foosballScore', false);
                    } else {
                        scope.resolveMatchForm.redScore.$setValidity('foosballScore', true);
                        scope.resolveMatchForm.blueScore.$setValidity('foosballScore', true);
                    }
                    // update scope with the validated value
                    return viewValue;
                });
            }
        };
    })
    .directive('timeLeftToEdit', function($timeout) {
        return function (scope, element, attributes) {
            if (scope.currentUserId) {
                var timeoutId,
                    matchTime = attributes.timeLeftToEdit;

                function getTimeDifference() {
                    var currentTime = new Date().getTime(),
                        timeLimit = 1000 * 60 * 5, // equals 5 minutes
                        timeDifference = matchTime - currentTime + timeLimit;

                    if (timeDifference <= 0) {
                        element.remove();
                    } else {
                        element.removeClass('hide');
                    }
                    return prettyTime(timeDifference);
                }

                function prettyTime(milliseconds) {
                    var seconds = parseInt((milliseconds / 1000) % 60),
                        minutes = parseInt((milliseconds / (1000 * 60)) % 60);

                    if (seconds < 10) {
                        seconds = "0" + seconds;
                    }
                    return minutes + ':' + seconds;
                }

                function updateTime() {
                    element.val('Cancel match (' + getTimeDifference() + ')');
                }

                function updateLater() {
                    timeoutId = $timeout(function () {
                        updateTime();
                        updateLater();
                    }, 1000);
                }

                // listen on DOM destroy (removal) event, and cancel the next UI update
                // to prevent updating time after the DOM element was removed.
                element.on('$destroy', function () {
                    $timeout.cancel(timeoutId);
                });
                
                // Only update if the user is logged in and participated in the match
                if (scope.match.PlayersHash.indexOf(scope.currentUserId) > -1)
                {
                    updateLater();
                }
            }
        };
    });
