function PlayerStatsController($scope, $resource, $location) {
    $scope.playerStats = [];
    $scope.hex_md5 = hex_md5;

    // Start fetching player statistics, return a promise
    $scope.getPlayerStats = function() {
        var search = "?playerId=" + $location.search()["playerId"],
            PlayerStats = $resource('/Stats/GetPlayerStatistics' + search),
            promise = PlayerStats.get().$promise;
        

        promise.then(function(playerStats) {
            $scope.playerStats = preparePlayerStats(playerStats);
        });
    };

    $scope.getPlayerRatingData = function() {
        var search = "?playerId=" + $location.search()["playerId"],
            ChartData = $resource('/Stats/GetPlayerRatingData' + search),
            promise = ChartData.get().$promise,
            preparedChartData;
        
        promise.then(function (chartData) {
            preparedChartData = prepareChartData(chartData);
            renderChart(preparedChartData);
        });
    };

    $scope.getPlayerStatsUrl = function (playerId) {
        return '/#/playerstats?playerId=' + playerId;
    };

    $scope.getPlayerStats();
    $scope.getPlayerRatingData();

    function preparePlayerStats(playerStats) {
        var statsUrl = '/#/playerstats?playerId=';
        playerStats.Player.GravatarUrl = 'http://www.gravatar.com/avatar/' + hex_md5(playerStats.Player.Email) + '?d=mm';
        playerStats.Player.Url = statsUrl + playerStats.Player.Id;
        playerStats.Bff.Player.Url = statsUrl + playerStats.Bff.Player.Id;
        playerStats.Rbff.Player.Url = statsUrl + playerStats.Rbff.Player.Id;
        playerStats.Eae.Player.Url = statsUrl + playerStats.Eae.Player.Id;

        return playerStats;
    }

    function prepareChartData(chartData) {
        var preparedChartData = { DataPoints: [] };

        angular.forEach(chartData.DataPoints, function (value, key) {
            preparedChartData.DataPoints.push([
                new Date(Date.UTC(
                    value.TimeSet[0], // year
                    parseInt(value.TimeSet[1]) - 1, // month (0-based)
                    value.TimeSet[2], // day
                    value.TimeSet[3], // hour
                    value.TimeSet[4], // minute
                    value.TimeSet[5]  // second
                )).toDateString(),
                parseFloat(value.Rating.toPrecision(6))
            ]);
        });

        preparedChartData.MinimumValue = chartData.MinimumValue - 50;
        preparedChartData.MaximumValue = chartData.MaximumValue + 50;
        preparedChartData.PlayerName = chartData.Player.Name;
        preparedChartData.DataPoints[0][0] = 'The "birth" of ' + preparedChartData.PlayerName;

        return preparedChartData;
    }

    function renderChart(chartData) {
        new Highcharts.Chart({
            chart: {
                renderTo: 'player-rating-chart',
                zoomType: 'x',
                spacingRight: 10
            },
            title: {
                text: chartData.PlayerName + 's rating over time'
            },
            subtitle: {
                text: document.ontouchstart === undefined
                        ? 'Click and drag in the plot area to zoom in'
                        : 'Drag your finger over the plot to zoom in'
            },
            xAxis: {
                title: {
                    text: null
                }
            },
            yAxis: {
                title: {
                    text: null
                },
                min: chartData.MinimumValue,
                max: chartData.MaximumValue,
                startOnTick: false,
                showFirstLabel: false
            },
            tooltip: {
                shared: true
            },
            legend: {
                enabled: false
            },
            plotOptions: {
                area: {
                    fillColor: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, Highcharts.getOptions().colors[0]],
                            [1, 'rgba(2,0,0,0)']
                        ]
                    },
                    lineWidth: 1,
                    marker: {
                        enabled: false,
                        states: {
                            hover: {
                                enabled: true,
                                radius: 5
                            }
                        }
                    },
                    shadow: false,
                    states: {
                        hover: {
                            lineWidth: 1
                        }
                    }
                }
            },
            series: [{
                type: 'area',
                name: 'Rating',
                data: chartData.DataPoints
            }]
        });
    }
}

PlayerStatsController.$inject = ['$scope', '$resource', '$location'];
