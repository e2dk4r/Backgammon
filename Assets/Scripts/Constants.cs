using UnityEngine;

public class Constants
{
    public const string LAYER_PIECE_STR = "Piece";
    public static int LAYER_PIECE => LayerMask.GetMask(LAYER_PIECE_STR);

    public const string SCENE_WHO_IS_FIRST = "WhoIsFirstScene";
    public const string SCENE_GAME = "GameScene";

    public const string PREF_CURRENT_PLAYER = "current_player";
    public const string PREF_CURRENT_PLAYER1 = "player1";
    public const string PREF_CURRENT_PLAYER2 = "player2";
}
