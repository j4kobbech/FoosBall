FoosBall.service('api', ['$resource', function ($resource) {
    this.getAllMatches = function () {
        var url = '/Matches/GetMatches?numberOfMatches=0'; // "numberOfMatches=0" fetches all matches
        var AllMatches = $resource(url);
        var promise = AllMatches.query().$promise;
        return promise;
    };
    this.getExperiencedPlayers = function () {
        var url = '/Players/GetExperiencedPlayers';
        var ExperiencedPlayers = $resource(url);
        var promise = ExperiencedPlayers.query().$promise;
        return promise;
    };
    this.getRankedPlayers = function () {
        var url = '/Players/GetRankedPlayers';
        var RankedPlayers = $resource(url);
        var promise = RankedPlayers.query().$promise;
        return promise;
    };
    this.getConfig = function () {
        var url = 'Admin/Config';
        var GetConfig = $resource(url);
        var promise = GetConfig.get().$promise;
        return promise;
    };
    this.setConfig = function (newConfig) {
        var url = 'Admin/Config';
        var SetConfig = $resource(url);
        var promise = SetConfig.save({ config: newConfig }).$promise;
        return promise;
    };

    this.replayAllMatches = function () {
        var url = 'Admin/ReplayMatches';
        var Replay = $resource(url);
        var promise = Replay.save().$promise;
        return promise;
    };
}]);
