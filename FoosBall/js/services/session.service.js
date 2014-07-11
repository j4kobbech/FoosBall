FoosBall.service('session', ['$resource', '$http', function ($resource, $http) {
    var self = this;

    this.logout = function() {
        var Logout = $resource('Session/Logout'),
                logout = new Logout(),
                logoutPromise = logout.$save();

        return logoutPromise.then(clearSession);
    };

    this.getCurrentUser = function() {
        var User = $resource('Account/GetCurrentUserInformation');
        var promise = User.get().$promise;

        return promise;
    };

    this.getSessionUserName = function () {
        var accessToken = this.getSessionStorage('AccessToken'),
            userName = "";

        if (accessToken && accessToken.UserName) {
            userName = accessToken.UserName;
        }

        return userName;
    };

    this.authenticateUser = function(loginParameters) {
        var Login = $resource('Session/Login'),
            login = new Login(loginParameters),
            loginPromise = login.$save();

        return loginPromise.then(handleAuthenticateResult);
    };

    this.setSessionStorage = function (key, value) {
        if (!key || typeof key !== 'string' || !value || typeof value === 'function') {
            return undefined;
        }

        var stringValue = (typeof value === 'object') ? JSON.stringify(value) : value;
    
        sessionStorage.setItem(key, stringValue);
    };

    this.getSessionStorage = function(key) {
        var value = sessionStorage.getItem(key),
            parsedJson;

        try {
            parsedJson = JSON.parse(value);
        } catch (e) {
            // means the json was bad or not json at all ... 
            // just continue ...
        }

        return parsedJson || value;
    };

    // If an AccessToken exists in the sessionStorage then set it as default in $http request headers
    this.refreshRequestHeaders = function () {
        if (self.isLoggedIn()) {
            $http.defaults.headers.common.AccessToken = self.getSessionStorage('AccessToken').TokenValue;
        }
    };

    this.isLoggedIn = function() {
        var token = self.getSessionStorage('AccessToken');
        return !!token && !!self.getSessionStorage('AccessToken').TokenValue;
    };

    function handleAuthenticateResult(ajaxResponse) {
        if (ajaxResponse && ajaxResponse.Success) {
            self.setSessionStorage('AccessToken', ajaxResponse.Data.AccessToken);
            $http.defaults.headers.common.AccessToken = ajaxResponse.Data.AccessToken.TokenValue;
        } else {
            clearSession(ajaxResponse);
        }

        return ajaxResponse;
    }

    function clearSession(ajaxResponse) {
        sessionStorage.removeItem('AccessToken');
        $http.defaults.headers.common.AccessToken = null;

        return ajaxResponse.Data;
    }
}]);
