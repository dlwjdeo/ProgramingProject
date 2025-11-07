public static class TagName
{
    public const string Untagged = "Untagged";
    public const string Respawn = "Respawn";
    public const string Finish = "Finish";
    public const string EditorOnly = "EditorOnly";
    public const string MainCamera = "MainCamera";
    public const string Player = "Player";
    public const string GameController = "GameController";
    public const string Ground = "Ground";
}

public enum Item
{
    RustKey,
    Knife,
    Doll,
    SeveredFinger,



    Null,
}

public enum CameraArea
{
    Floor1,
    Floor2
}

public enum PlayerStateType
{
    Idle,
    Hide,
    Run,
    Jump,
    Fall,
    Walk,

}

public enum EnemyStateType
{
    Suspicious,
    Patrol,
    Chase,
    Wait
}

public enum GameState
{
    Playing,
    Dialog,
    Paused,
}