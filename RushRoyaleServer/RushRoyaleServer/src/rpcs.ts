let findMatch: nkruntime.RpcFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, payload: string): string {
    let lobby = JSON.parse(payload);

    let matches: nkruntime.Match[];
    const MatchesLimit = 5;
    const MinimumPlayers = 0;
    var label: MatchLabel = { open: true, lobbyName: lobby["lobbyName"] };
    matches = nakama.matchList(MatchesLimit, true, null, MinimumPlayers, MaxPlayers - 1);
    if (matches.length == 0)
        return "";

    return matches[parseInt(lobby["matchNumber"])].matchId;
}

let writeMoneyData: nkruntime.RpcFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, payload: string): string {
    let obj = JSON.parse(payload);
    let player = JSON.parse(obj["value"]);

    let userId = AdminId;

    let objectIds: nkruntime.StorageReadRequest[] = [
        { collection: "Items", key: "Money", userId: userId },
    ];

    let data = nakama.storageRead(objectIds);
    let moneyData = data[0].value;
    moneyData[player["userId"]] = player["money"]

    let newObjects: nkruntime.StorageWriteRequest[] = [
        { collection: obj.collection, key: obj.key, userId, value: moneyData }
    ];
    try {
        nakama.storageWrite(newObjects);
        return "Data saved successfully!"
    }
    catch (error) {
        return "An error occurred while data saving!"
    }
}

let readMoneyData: nkruntime.RpcFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, payload: string): string {
    let obj = JSON.parse(payload);
    let userId = AdminId;

    let objectIds: nkruntime.StorageReadRequest[] = [
        { collection: "Items", key: "Money", userId: userId },
    ];

    try {
        let data = nakama.storageRead(objectIds);
        let moneyData = data[0].value[obj];

        return JSON.stringify(moneyData);
    }
    catch (error) {
        return JSON.stringify({ Status: false, Data: "", Message: error });
    }
}

