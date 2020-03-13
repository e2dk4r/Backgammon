using UnityEngine;

[CreateAssetMenu(fileName = "piece_", menuName = "Backgammon/Piece")]
public class PieceObject : ScriptableObject
{
    public Sprite sprite;
    public PieceType pieceType;
}