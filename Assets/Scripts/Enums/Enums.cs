public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}

public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
}

public static class Tags
{
    public const string ground = "groundTilemap";
    public const string decoration1Tilemap = "decoration1Tilemap";
    public const string decoration2Tilemap = "decoration2Tilemap";
    public const string frontTilemap = "frontTilemap";
    public const string collisionTilemap = "collisionTilemap";
    public const string minimapTilemap = "minimapTilemap";
    public const string player = "Player";
    public const string playerWeapon = "playerWeapon";
}

