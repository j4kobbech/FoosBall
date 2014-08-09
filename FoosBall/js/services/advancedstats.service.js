FoosBall.service('advancedStats', [function () {
    this.stats = {
        rank: {
            teamAPlayer1: 0,
            teamAPlayer2: 0,
            teamBPlayer1: 0,
            teamBPlayer2: 0
        },
        matches: 0,
        matchesTotal: {
            teamA: 0,
            teamB: 0
        },
        fightsWon: {
            teamA: 0,
            teamB: 0
        },
        pointsWon: {
            teamA: 0,
            teamB: 0
        },
        goalsScored: {
            teamA: 0,
            teamB: 0
        },
        preferredColor: {
            teamA: { color: "", count: 0 },
            teamB: { color: "", count: 0 }
        },
        biggestWin: {
            teamA: 0,
            teamB: 0
        },
        flawlessWins: {
            teamA: 0,
            teamB: 0
        },
        mostPointsWon: {
            teamA: 0,
            teamB: 0
        },
        experience: {
            teamAPlayer1: "",
            teamAPlayer2: "",
            teamBPlayer1: "",
            teamBPlayer2: ""
        }
    };
    this.get = function (matches, participants, players) {
        var sortedPlayers = angular.copy(players).sort(function (a, b) { return a.Rating - b.Rating; }).reverse();
        var elligibleMatches = getMatchesWithAllParticipants(participants, matches);
        this.stats.rank = getRanks(participants, sortedPlayers);
        this.stats.matchesVersus = elligibleMatches.length;
        this.stats.matchesTotal.teamA = getMatchesWithTeam(participants.teamAPlayer1Id, participants.teamAPlayer2Id, matches).length;
        this.stats.matchesTotal.teamB = getMatchesWithTeam(participants.teamBPlayer1Id, participants.teamBPlayer2Id, matches).length;
        this.stats.fightsWon = getFightsWon(participants, elligibleMatches);
        this.stats.pointsWon = getPointsWon(participants, elligibleMatches);
        this.stats.goalsScored = getGoalsScored(participants, elligibleMatches);
        this.stats.preferredColor = getPreferredColor(participants, elligibleMatches);
        this.stats.biggestWin = getBiggestWin(participants, elligibleMatches);
        this.stats.flawlessWins = getFlawlessWins(participants, elligibleMatches);
        this.stats.mostPointsWon = getMostPointsWon(participants, elligibleMatches);
        this.stats.experience = getPlayerExperiences(participants, players, matches);
        return this.stats;
    };
    function getPlayerExperiences(participants, players, allMatches) {
        var counts = {
            teamAPlayer1: { red: 0, blue: 0, total: 0 },
            teamAPlayer2: { red: 0, blue: 0, total: 0 },
            teamBPlayer1: { red: 0, blue: 0, total: 0 },
            teamBPlayer2: { red: 0, blue: 0, total: 0 }
        };
        var experience = {
            teamAPlayer1: "",
            teamAPlayer2: "",
            teamBPlayer1: "",
            teamBPlayer2: ""
        };
        var redTitles = {
            "Tooka": 1,
            "Tusken Raider": 5,
            "Jawa": 25,
            "Sith Acolyte": 50,
            "Sith Apprentice": 100,
            "Sith Knight": 150,
            "Boba Fett": 200,
            "Jabba the Hutt": 250,
            "Sith Overlord": 300,
            "Count Dooku": 350,
            "Darth Maul": 400,
            "Darth Revan": 450,
            "Lord Vader": 500,
            "Darth Sidious": Infinity
        };
        var blueTitles = {
            "Jar Jar Binks": 1,
            "Ewok": 5,
            "R2-D2": 25,
            "C-3PO": 50,
            "Jedi Apprentice": 100,
            "Jedi Knight": 150,
            "Jedi Captain": 200,
            "Admiral Ackbar": 250,
            "Jedi Master": 300,
            "Han Solo": 350,
            "Qui-Gon Jinn": 400,
            "Obi-Wan Kenobi": 450,
            "Master Yoda": 500,
            "Luke Skywalker": Infinity
        };
        angular.forEach(allMatches, function (match, index) {
            var matchExt = new MatchExtension(match);

            if (matchExt.containsPlayer(participants.teamAPlayer1Id)) {
                if (matchExt.playerIsOnRedTeam(participants.teamAPlayer1Id)) {
                    counts.teamAPlayer1.red++;
                } else {
                    counts.teamAPlayer1.blue++;
                }
                counts.teamAPlayer1.total++;
            }
            if (participants.teamAPlayer2Id && matchExt.containsPlayer(participants.teamAPlayer2Id)) {
                if (matchExt.playerIsOnRedTeam(participants.teamAPlayer1Id)) {
                    counts.teamAPlayer2.red++;
                } else {
                    counts.teamAPlayer2.blue++;
                }
                counts.teamAPlayer2.total++;
            }
            if (matchExt.containsPlayer(participants.teamBPlayer1Id)) {
                if (matchExt.playerIsOnRedTeam(participants.teamAPlayer1Id)) {
                    counts.teamBPlayer1.red++;
                } else {
                    counts.teamBPlayer1.blue++;
                }
                counts.teamBPlayer1.total++;
            }
            if (participants.teamBPlayer2Id && matchExt.containsPlayer(participants.teamBPlayer2Id)) {
                if (matchExt.playerIsOnRedTeam(participants.teamAPlayer1Id)) {
                    counts.teamBPlayer2.red++;
                } else {
                    counts.teamBPlayer2.blue++;
                }
                counts.teamBPlayer2.total++;
            }
        });
        angular.forEach(counts, function (player, playerIndex) {
            var titles;

            if (player.red > player.blue) {
                titles = redTitles;
            } else {
                titles = blueTitles;
            }
            for (var titleIndex in titles) {
                var title = titles[titleIndex];
                if (player.total && player.total <= title) {
                    experience[playerIndex] = titleIndex + " (" + player.total + ")";
                    break;
                }
            };
        });
        return experience;
    }
    function getBiggestWin(participants, matches) {
        var biggestWin = {
            teamA: { diff: 0, result: "" },
            teamB: { diff: 0, result: "" }
        };
        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            var diff = Math.abs(matchExt.match.RedScore - matchExt.match.BlueScore);
            var result = Math.max(matchExt.match.RedScore, matchExt.match.BlueScore) + "-" + Math.min(matchExt.match.RedScore, matchExt.match.BlueScore);
            if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                if (diff > biggestWin.teamA.diff) {
                    biggestWin.teamA.diff = diff;
                    biggestWin.teamA.result = result;
                }
            } else {
                if (diff > biggestWin.teamB.diff) {
                    biggestWin.teamB.diff = diff;
                    biggestWin.teamB.result = result;
                }
            }
        }
        return biggestWin;
    }
    function getMostPointsWon(participants, matches) {
        var mostPointsWon = {
            teamA: 0,
            teamB: 0
        };
        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                mostPointsWon.teamA = mostPointsWon.teamA < matchExt.match.DistributedRating ? matchExt.match.DistributedRating : mostPointsWon.teamA;
            } else {
                mostPointsWon.teamB = mostPointsWon.teamB < matchExt.match.DistributedRating ? matchExt.match.DistributedRating : mostPointsWon.teamB;
            }
        }
        return mostPointsWon;
    }
    function getFlawlessWins(participants, matches) {
        var flawlessWins = {
            teamA: 0,
            teamB: 0
        };
        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            if (matchExt.match.RedScore === 10 && matchExt.match.BlueScore === 0 || matchExt.match.BlueScore === 10 && matchExt.match.RedScore === 0) {
                if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                    flawlessWins.teamA++;
                } else {
                    flawlessWins.teamB++;
                }
            }
        }
        return flawlessWins;
    }
    function getPreferredColor(participants, matches) {
        var counts = {
            teamA: { red: 0, blue: 0 },
            teamB: { red: 0, blue: 0 }
        };
        var preferredColor = {
            teamA: { color: "", count: 0 },
            teamB: { color: "", count: 0 }
        };

        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            if (matchExt.playerIsOnRedTeam(participants.teamAPlayer1Id)) {
                counts.teamA.red++;
                counts.teamB.blue++;
            } else {
                counts.teamA.blue++;
                counts.teamB.red++;
            }
        }
        if (counts.teamA.red > counts.teamA.blue) {
            preferredColor.teamA.color = "red";
            preferredColor.teamA.count = counts.teamA.red;
            preferredColor.teamB.color = "blue";
            preferredColor.teamB.count = counts.teamB.blue;
        } else if (counts.teamA.red < counts.teamA.blue) {
            preferredColor.teamA.color = "blue";
            preferredColor.teamA.count = counts.teamA.blue;
            preferredColor.teamB.color = "red";
            preferredColor.teamB.count = counts.teamB.red;
        } else {
            preferredColor.teamA.color = "both";
            preferredColor.teamA.count = counts.teamA.red;
            preferredColor.teamB.color = "both";
            preferredColor.teamB.count = counts.teamB.red;
        }

        return preferredColor;
    }
    function getFightsWon(participants, matches) {
        var fightsWon = {
            teamA: 0,
            teamB: 0
        };
        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                fightsWon.teamA++;
            } else {
                fightsWon.teamB++;
            }
        }
        return fightsWon;
    }
    function getPointsWon(participants, matches) {
        var pointsWon = {
            teamA: 0,
            teamB: 0
        };
        for (var match in matches) {
            var matchExt = new MatchExtension(matches[match]);
            if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                pointsWon.teamA = pointsWon.teamA + matchExt.match.DistributedRating;
            } else {
                pointsWon.teamB = pointsWon.teamB + matchExt.match.DistributedRating;
            }
        }
        return pointsWon;
    }
    function getGoalsScored(participants, matches) {
        var goalsScored = {
            teamA: 0,
            teamB: 0
        };
        for (var key in matches) {
            var match = matches[key];
            var winningScore = match.RedScore > match.BlueScore ? match.RedScore : match.BlueScore;
            var losingScore = match.RedScore > match.BlueScore ? match.BlueScore : match.RedScore;
            var matchExt = new MatchExtension(matches[key]);
            if (matchExt.playerIsOnWinningTeam(participants.teamAPlayer1Id)) {
                goalsScored.teamA = goalsScored.teamA + winningScore;
                goalsScored.teamB = goalsScored.teamB + losingScore;
            } else {
                goalsScored.teamB = goalsScored.teamB + winningScore;
                goalsScored.teamA = goalsScored.teamA + losingScore;
            }
        }
        return goalsScored;
    }
    function getRanks(participants, players) {
        var index = 0;
        var rankingDiscrepancy = 0;
        var playerCount = players.length;
        var rankings = {
            teamAPlayer1: 0,
            teamAPlayer2: 0,
            teamBPlayer1: 0,
            teamBPlayer2: 0
        };
        for (index; index < playerCount; index++) {
            var player = players[index];
            if (participants.teamAPlayer1Id === player.Id) {
                if (player.Deactivated === true) {
                    rankingDiscrepancy++;
                    rankings.teamAPlayer1 = "n/a";
                } else {
                    rankings.teamAPlayer1 = index - rankingDiscrepancy + 1;
                }
                continue;
            }
            if (participants.teamBPlayer1Id === player.Id) {
                if (player.Deactivated === true) {
                    rankingDiscrepancy++;
                    rankings.teamBPlayer1 = "n/a";
                } else {
                    rankings.teamBPlayer1 = index - rankingDiscrepancy + 1;
                }
                continue;
            }
            if (participants.teamAPlayer2Id && participants.teamAPlayer2Id === player.Id) {
                if (player.Deactivated === true) {
                    rankingDiscrepancy++;
                    rankings.teamAPlayer2 = "n/a";
                } else {
                    rankings.teamAPlayer2 = index - rankingDiscrepancy + 1;
                }
                continue;
            }
            if (participants.teamBPlayer2Id && participants.teamBPlayer2Id === player.Id) {
                if (player.Deactivated === true) {
                    rankingDiscrepancy++;
                    rankings.teamBPlayer2 = "n/a";
                } else {
                    rankings.teamBPlayer2 = index - rankingDiscrepancy + 1;
                }
                continue;
            }
        };
        return rankings;
    }
    function getMatchesWithTeam(player1Id, player2Id, matches) {
        var eligibleMatches = [];
        angular.forEach(matches, function (match) {
            var matchExt = new MatchExtension(match); // create a match helper object
            if (teamMatch(player1Id, player2Id, matchExt)) {
                eligibleMatches.push(match);
            }
        });
        return eligibleMatches;
    }
    function getMatchesWithAllParticipants(participants, matches) {
        var eligibleMatches = [];
        angular.forEach(matches, function (match) {
            var matchExt = new MatchExtension(match); // create a match helper object
            if (participantsMatch(matchExt, participants)) {
                eligibleMatches.push(match);
            }
        });
        return eligibleMatches;
    };
    function teamMatch(player1Id, player2Id, matchExt) {
        if (matchExt.containsPlayer(player1Id) &&
            matchExt.containsPlayer(player2Id) &&
            matchExt.playersAreTeammates(player1Id, player2Id)) {
            return true;
        }
        return false;
    }
    function participantsMatch(matchExt, participants) {
        if (!matchExt.containsPlayer(participants.teamAPlayer1Id)) {
            return false;
        }
        if (!matchExt.containsPlayer(participants.teamBPlayer1Id)) {
            return false;
        }
        if (!matchExt.playersAreOpponents(participants.teamAPlayer1Id, participants.teamBPlayer1Id)) {
            return false;
        }
        if (participants.teamAPlayer2Id && !matchExt.containsPlayer(participants.teamAPlayer2Id)) {
            return false;
        }
        if (participants.teamBPlayer2Id && !matchExt.containsPlayer(participants.teamBPlayer2Id)) {
            return false;
        }
        if (participants.teamAPlayer2Id && participants.teamBPlayer2Id && !matchExt.playersAreOpponents(participants.teamAPlayer2Id, participants.teamBPlayer2Id)) {
            return false;
        }
        if (participants.teamAPlayer2Id && !matchExt.playersAreTeammates(participants.teamAPlayer2Id, participants.teamAPlayer1Id)) {
            return false;
        }
        return true;
    }
    function MatchExtension(match) {
        this.match = match;
        this.playerIsOnRedTeam = function (id) {
            if (this.match.RedPlayer1.Id === id || this.match.RedPlayer2.Id === id) {
                return true;
            }
            return false;
        };
        this.playerIsOnWinningTeam = function (id) {
            if (this.match.RedPlayer1.Id === id || (this.match.RedPlayer2 && this.match.RedPlayer2.Id === id)) {
                return this.match.RedScore > this.match.BlueScore;
            } else {
                return this.match.BlueScore > this.match.RedScore;
            }
        };
        this.containsPlayer = function (id) {
            return this.match.RedPlayer1.Id == id ||
                   this.match.RedPlayer2.Id == id ||
                   this.match.BluePlayer1.Id == id ||
                   this.match.BluePlayer2.Id == id;
        };
        this.playersAreOpponents = function (id1, id2) {
            return (this.match.RedPlayer1.Id == id1 || this.match.RedPlayer2.Id == id1) &&
                   (this.match.BluePlayer1.Id == id2 || this.match.BluePlayer2.Id == id2) ||
                   (this.match.RedPlayer1.Id == id2 || this.match.RedPlayer2.Id == id2) &&
                   (this.match.BluePlayer1.Id == id1 || this.match.BluePlayer2.Id == id1);
        };
        this.playersAreTeammates = function (id1, id2) {
            return (this.match.RedPlayer1.Id == id1 || this.match.RedPlayer2.Id == id1) &&
                   (this.match.RedPlayer1.Id == id2 || this.match.RedPlayer2.Id == id2) ||
                   (this.match.BluePlayer1.Id == id2 || this.match.BluePlayer2.Id == id2) &&
                   (this.match.BluePlayer1.Id == id1 || this.match.BluePlayer2.Id == id1);
        };
    }
}]);
