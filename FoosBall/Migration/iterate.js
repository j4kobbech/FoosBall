db.Matches
    .find()
    .forEach(function(match){
        db.Matches.update(match, {$set: {"Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Matches
    .find()
    .forEach(function(match){
        db.Matches.update(match, {$set: {"RedPlayer1.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Matches
    .find()
    .forEach(function(match){
        db.Matches.update(match, {$set: {"RedPlayer2.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Matches
    .find()
    .forEach(function(match){
        db.Matches.update(match, {$set: {"BluePlayer1.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Matches
    .find()
    .forEach(function(match){
        db.Matches.update(match, {$set: {"BluePlayer2.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
print('Updated Matches');

db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, { $set: {"Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, { $set: {"Player.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, { $set: {"Match.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, { $set: {"Match.CreationTime": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, {$set: {"Match.RedPlayer1.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, {$set: {"Match.RedPlayer2.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, {$set: {"Match.BluePlayer1.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
db.Events
    .find()
    .forEach(function(event){
        db.Events.update(event, {$set: {"Match.BluePlayer2.Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
print('Updated Events');

db.Players
    .find()
    .forEach(function(player){
        db.Players.update(player, { $set: {"Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
print('Updated Players');

db.AutoLogin
    .find()
    .forEach(function(autologin){
        db.AutoLogin.update(autologin, { $set: {"Updated": ISODate("0001-01-01T00:00:00.000Z")}});
    });
print('Updated AutoLogin');

