const enum Scene
{
    Initializer = 0,
    Splash = 1,
    Home = 2,
    Lobby = 3,
    Battle = 4,
}

const enum OperationCode
{
    Players = 0,
    PlayerJoined = 1,
    PlayerInput = 2,
    PlayerWon = 3,
    Draw = 4,
    ChangeScene = 5,
    SpawnHero = 6,
    SpawnAreaData = 7,
    Hit = 8,
    Die = 9,
    GameOver = 10
}
