"use strict";
var FindMatchRpc = "FindMatchRpc";
var LogicLoadedLoggerInfo = "Custom logic loaded.";
var MatchModuleName = "match";
function InitModule(ctx, logger, nk, initializer) {
    initializer.registerRpc(FindMatchRpc, findMatch);
    initializer.registerMatch(MatchModuleName, {
        matchInit: matchInit,
        matchJoinAttempt: matchJoinAttempt,
        matchJoin: matchJoin,
        matchLeave: matchLeave,
        matchLoop: matchLoop,
        matchTerminate: matchTerminate,
        matchSignal: matchSignal
    });
    logger.info(LogicLoadedLoggerInfo);
}
var findMatch = function (context, logger, nakama, payload) {
    var lobby = JSON.parse(payload);
    var matches;
    var MatchesLimit = 5;
    var MinimumPlayers = 0;
    var label = { open: true, lobbyName: lobby["lobbyName"] };
    matches = nakama.matchList(MatchesLimit, true, null, MinimumPlayers, MaxPlayers - 1);
    if (matches.length == 0)
        return "";
    return matches[parseInt(lobby["matchNumber"])].matchId;
};
var writeMoneyData = function (context, logger, nakama, payload) {
    var obj = JSON.parse(payload);
    var player = JSON.parse(obj["value"]);
    var userId = AdminId;
    var objectIds = [
        { collection: "Items", key: "Money", userId: userId },
    ];
    var data = nakama.storageRead(objectIds);
    var moneyData = data[0].value;
    moneyData[player["userId"]] = player["money"];
    var newObjects = [
        { collection: obj.collection, key: obj.key, userId: userId, value: moneyData }
    ];
    try {
        nakama.storageWrite(newObjects);
        return "Data saved successfully!";
    }
    catch (error) {
        return "An error occurred while data saving!";
    }
};
var readMoneyData = function (context, logger, nakama, payload) {
    var obj = JSON.parse(payload);
    var userId = AdminId;
    var objectIds = [
        { collection: "Items", key: "Money", userId: userId },
    ];
    try {
        var data = nakama.storageRead(objectIds);
        var moneyData = data[0].value[obj];
        return JSON.stringify(moneyData);
    }
    catch (error) {
        return JSON.stringify({ Status: false, Data: "", Message: error });
    }
};
var matchInit = function (context, logger, nakama, params) {
    var label = { open: true, lobbyName: params.lobbyName };
    var gameState = {
        password: params.password,
        players: [],
        spawnAreaList: [],
        scene: 3 /* Scene.Lobby */,
        countdown: DurationLobby * TickRate,
        endMatch: false,
        gameStarted: false
    };
    return {
        state: gameState,
        tickRate: TickRate,
        label: JSON.stringify(label),
    };
};
var matchJoinAttempt = function (context, logger, nakama, dispatcher, tick, state, presence, metadata) {
    var gameState = state;
    // if (gameState.password != metadata.password )
    //     return{
    //         state: gameState,
    //         accept: false,
    //     }
    return {
        state: gameState,
        accept: gameState.scene == 3 /* Scene.Lobby */,
    };
};
var matchJoin = function (context, logger, nakama, dispatcher, tick, state, presences) {
    logger.warn("asdasdasdas");
    var gameState = state;
    if (gameState.scene != 3 /* Scene.Lobby */)
        return { state: gameState };
    var presencesOnMatch = [];
    gameState.players.forEach(function (player) { if (player != undefined)
        presencesOnMatch.push(player.presence); });
    for (var _i = 0, presences_1 = presences; _i < presences_1.length; _i++) {
        var presence = presences_1[_i];
        var account = nakama.accountGetId(presence.userId);
        var player = {
            presence: presence,
            displayName: account.user.displayName,
            currentMana: BeginningManaValue
        };
        var nextPlayerNumber = getNextPlayerNumber(gameState.players);
        gameState.players[nextPlayerNumber] = player;
        dispatcher.broadcastMessage(1 /* OperationCode.PlayerJoined */, JSON.stringify(gameState.players[nextPlayerNumber]));
        presencesOnMatch.push(presence);
    }
    for (var x = 0; x < SpawnPositionCount; x++) // Set all spawn area points
     {
        var area = {
            isSpawnable: true,
            listNumber: x
        };
        gameState.spawnAreaList[x] = area;
    }
    dispatcher.broadcastMessage(0 /* OperationCode.Players */, JSON.stringify(gameState.players));
    dispatcher.broadcastMessage(7 /* OperationCode.SpawnAreaData */, JSON.stringify(gameState.spawnAreaList)); // send all clients to update HeroSpawnAreaList
    gameState.countdown = DurationLobby * TickRate;
    return { state: gameState };
};
var matchLoop = function (context, logger, nakama, dispatcher, tick, state, messages) {
    logger.warn("asdasdasdadqwd");
    var gameState = state;
    processMessages(messages, gameState, dispatcher, nakama, logger);
    processMatchLoop(gameState, nakama, dispatcher, logger);
    return gameState.endMatch ? null : { state: gameState };
};
var matchLeave = function (context, logger, nakama, dispatcher, tick, state, presences) {
    var gameState = state;
    for (var _i = 0, presences_2 = presences; _i < presences_2.length; _i++) {
        var presence = presences_2[_i];
        var playerNumber = getPlayerNumber(gameState.players, presence.sessionId);
        gameState.players.splice(playerNumber, 1);
    }
    if (getPlayersCount(gameState.players) == 0)
        return null;
    return { state: gameState };
};
var matchTerminate = function (context, logger, nakama, dispatcher, tick, state, graceSeconds) {
    return { state: state };
};
var matchSignal = function (context, logger, nk, dispatcher, tick, state, data) {
    return { state: state };
};
function processMessages(messages, gameState, dispatcher, nakama, logger) {
    for (var _i = 0, messages_1 = messages; _i < messages_1.length; _i++) {
        var message = messages_1[_i];
        var opCode = message.opCode;
        if (MessagesLogic.hasOwnProperty(opCode))
            MessagesLogic[opCode](message, gameState, dispatcher, nakama, logger);
        else
            messagesDefaultLogic(message, gameState, dispatcher);
    }
}
function messagesDefaultLogic(message, gameState, dispatcher) {
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
function processMatchLoop(gameState, nakama, dispatcher, logger) {
    switch (gameState.scene) {
        case 4 /* Scene.Battle */:
            matchLoopBattle(gameState, nakama, dispatcher);
            break;
        case 3 /* Scene.Lobby */:
            matchLoopLobby(gameState, nakama, dispatcher);
            break;
    }
}
function matchLoopBattle(gameState, nakama, dispatcher) {
    if (gameState.countdown > 0) {
        gameState.countdown--;
        if (gameState.countdown == 0) {
            gameState.countdown = 10 * TickRate;
        }
    }
}
function matchLoopLobby(gameState, nakama, dispatcher) {
    gameState.scene = 4 /* Scene.Battle */;
    gameState.countdown = DurationBattle * TickRate;
    dispatcher.matchLabelUpdate(JSON.stringify({ open: false }));
}
function matchLoopRoundResults(gameState, nakama, dispatcher) {
    if (gameState.countdown > 0) {
        gameState.countdown--;
        if (gameState.countdown == 0) {
            gameState.scene = 4 /* Scene.Battle */;
            gameState.countdown = DurationBattle * TickRate;
            dispatcher.broadcastMessage(5 /* OperationCode.ChangeScene */, JSON.stringify(gameState.scene));
        }
    }
}
function playerWon(message, gameState, dispatcher, nakama, logger) {
    if (gameState.scene != 4 /* Scene.Battle */ || gameState.gameStarted == false)
        return;
    var data = JSON.parse(nakama.binaryToString(message.data));
    var playerNumber = getPlayerNumber(gameState.players, data.presence.sessionId);
    gameState.countdown = DurationBattleEnding * TickRate;
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
function draw(message, gameState, dispatcher, nakama, logger) {
    if (gameState.scene != 4 /* Scene.Battle */ || gameState.gameStarted == false)
        return;
    var data = JSON.parse(nakama.binaryToString(message.data));
    var tick = data.tick;
    gameState.countdown = DurationBattleEnding * TickRate;
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
function getPlayersCount(players) {
    var count = 0;
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined)
            count++;
    return count;
}
function playerObtainedNecessaryWins(playersWins) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (playersWins[playerNumber] == NecessaryWins)
            return true;
    return false;
}
function getWinner(playersWins, players) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (playersWins[playerNumber] == NecessaryWins)
            return players[playerNumber];
    return null;
}
function getPlayerNumber(players, sessionId) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined && players[playerNumber].presence.sessionId == sessionId)
            return playerNumber;
    return PlayerNotFound;
}
function getNextPlayerNumber(players) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (!playerNumberIsUsed(players, playerNumber))
            return playerNumber;
    return PlayerNotFound;
}
function playerNumberIsUsed(players, playerNumber) {
    return players[playerNumber] != undefined;
}
function gameOver(gameState) {
    gameState.countdown = 10 * TickRate;
    return gameState;
}
function checkIfBoardIsAvailable(gameState) {
    for (var _i = 0, _a = gameState.spawnAreaList; _i < _a.length; _i++) {
        var area = _a[_i];
        if (area.isSpawnable) {
            return true;
        }
    }
    return false;
}
function spawnHeroRequest(message, gameState, dispatcher, nakama, logger) {
    if (checkIfBoardIsAvailable(gameState) == false)
        return;
    var area = Math.floor(Math.random() * SpawnPositionCount);
    var hero = Math.floor(Math.random() * TotalHeroCount);
    if (gameState.spawnAreaList[area].isSpawnable) {
        var data = { randomArea: area, randomHero: hero };
        logger.warn("sdaaaaaawdqwdsadqwddsad");
        gameState.spawnAreaList[area].isSpawnable = false;
        dispatcher.broadcastMessage(message.opCode, JSON.stringify(data), null, message.sender);
        dispatcher.broadcastMessage(7 /* OperationCode.SpawnAreaData */, JSON.stringify(gameState.spawnAreaList));
    }
    else {
        spawnHeroRequest(message, gameState, dispatcher, nakama, logger);
        return;
    }
}
var AdminId = "00000000-0000-0000-0000-000000000000";
var TickRate = 16;
var DurationBattle = 180;
var DurationLobby = 10;
var DurationRoundResults = 5;
var DurationBattleEnding = 3;
var NecessaryWins = 3;
var MaxPlayers = 7;
var PlayerNotFound = -1;
var SpawnPositionCount = 12;
var TotalHeroCount = 5;
var BeginningManaValue = 1000;
var CollectionUser = "User";
var KeyTrophies = "Trophies";
var MessagesLogic = {
    3: playerWon,
    4: draw,
    6: spawnHeroRequest
};
