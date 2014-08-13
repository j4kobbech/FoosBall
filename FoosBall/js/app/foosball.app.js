var FoosBall = angular.module('FoosBall', ['ngRoute', 'ngResource']);

// Configuration & Routing
FoosBall.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/', {
             redirectTo: '/players'
        })
        .when('/login', {
            templateUrl: '/partials/login.html',
            controller: 'LoginController'
        })
        .when('/signup', {
            templateUrl: '/partials/signup.html',
            controller: 'SignupController'
        })
        .when('/stats', {
            templateUrl: '/partials/stats.html',
            controller: 'StatsController'
        })
        .when('/playerstats', {
            templateUrl: '/partials/stats-player.html',
            controller: 'PlayerStatsController'
        })
        .when('/matches', {
            templateUrl: '/partials/matches.html',
            controller: 'MatchesController'
        })
        .when('/players', {
            templateUrl: '/partials/players.html',
            controller: 'PlayersController'
        })
        .when('/user-profile', {
            templateUrl: '/partials/user-profile.html',
            controller: 'UserController'
        })
        .when('/admin', {
            templateUrl: '/partials/admin.html',
            controller: 'AdminController'
        })
        .when('/activate-user', {
            templateUrl: '/partials/activate-user.html'
        })
        .otherwise({
             redirectTo: '/'
        });
}]);
