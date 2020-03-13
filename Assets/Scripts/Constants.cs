using UnityEngine;

public class Constants
{
    public const string LAYER_PIECE_STR = "Piece";
    public static int LAYER_PIECE => LayerMask.GetMask(LAYER_PIECE_STR);
}
