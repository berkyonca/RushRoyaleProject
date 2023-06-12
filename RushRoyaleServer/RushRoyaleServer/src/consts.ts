const AdminId = "00000000-0000-0000-0000-000000000000";
const TickRate = 16;
const DurationBattle = 180;
const DurationLobby = 10;
const DurationRoundResults = 5;
const DurationBattleEnding = 3;
const NecessaryWins = 3;
const MaxPlayers = 7;
const PlayerNotFound = -1;

const SpawnPositionCount = 12;
const TotalHeroCount = 5;
const BeginningManaValue = 1000;

const CollectionUser = "User";
const KeyTrophies = "Trophies";

const MessagesLogic: { [opCode: number]: (message: nkruntime.MatchMessage, state: GameState, dispatcher: nkruntime.MatchDispatcher, nakama: nkruntime.Nakama,logger:nkruntime.Logger) => void } =
{
    3: playerWon,
    4: draw,
    6: spawnHeroRequest
}
