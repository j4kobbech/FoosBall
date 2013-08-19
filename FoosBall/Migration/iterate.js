// mongo shell command:
// Local:   c:\mongodb\bin\mongo.exe -u admin -p 4va4lbert FoosBall c:\source\FoosBall\FoosBall\Migration\iterate.js
// Staging: c:\mongodb\bin\mongo.exe -u admin -p 4va4lbertml ds039417.mongolab.com:39417/appharbor_4ca40675-7f79-47f6-a83f-b227f24172c1 c:\source\FoosBall\FoosBall\Migration\iterate.js
// Production:   c:\mongodb\bin\mongo.exe -u admin -p 4va4lbertml ds039467.mongolab.com:39467/appharbor_91a02254-b6c1-4f8a-8a8d-8acbeb96c62f c:\source\FoosBall\FoosBall\Migration\iterate.js

db.Config
    .find()
    .forEach(function (config) {
        db.Config.update(config, { $set: { "Updated": ISODate("0001-01-01T00:00:00.000Z") } });
    });
db.Config
    .find()
    .forEach(function (config) {
        db.Config.update(config, { $set: { "Created": ISODate("0001-01-01T00:00:00.000Z") } });
    });
print('Updated Config');

db.Matches
    .find()
    .forEach(function (match) {
        db.Matches.update(match, { $set: { "Updated": ISODate("0001-01-01T00:00:00.000Z") } });
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

/*
*/