using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int slotId = -1;
    public List<Piece> pieces;

    public static bool IsTopPiece(Slot slot, Piece piece)
    {
        return slot.pieces.Last() == piece;
    }

    public int GetPieceTypeCount(PieceType type)
    {
        return pieces.Where(x => x.pieceType == type).Count();
    }

    public static int GetRequiredStepCount(Slot from, Slot to)
    {
        return Math.Abs(to.slotId - from.slotId);
    }
}
