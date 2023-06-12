interface MatchLabel
{
    open: boolean
    lobbyName: string
}

interface GameState
{
    password: string
    players: Player[]
    spawnAreaList: SpawnArea[]
    scene: Scene
    countdown: number
    endMatch: boolean
    gameStarted: boolean
}

interface Player
{
    presence: nkruntime.Presence
    displayName: string
    currentMana: number
}

interface TimeRemainingData
{
    time: number
}

interface PlayerWonData
{
    tick: number
    playerNumber: number
}

interface DrawData
{
    tick: number
}

interface TrophiesData
{
    amount: number
}

interface CardsData
{
    weaponName: string
    weaponDamage: number
    fireFrequency: number
}

interface SpawnArea
{
    isSpawnable: boolean
    listNumber: number
}

interface HeroData
{

}

interface RandomHeroAndArea
{
    randomArea: number
    randomHero: number
}
