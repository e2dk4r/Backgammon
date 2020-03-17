using System;
using System.Linq;
using UnityEngine;

public class Rule
{
    public static bool IsValidMove(Piece piece, Slot requestedSlot)
    {
        // valid moves
        if (IsSlotEmpty(requestedSlot))
            return true;
        if (IsSlotYours(requestedSlot, piece.pieceType))
            return true;

        // invalid moves
        if (!IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            return true;

        return false;
    }

    public static MoveError ValidateMove(Piece piece, Slot requestedSlot, int steps, out MoveActionTypes action)
    {
        action = MoveActionTypes.Move;
        var requiredStep = Slot.GetRequiredStepCount(piece.currentSlot, requestedSlot);

        var lastSlot = Slot.GetLastSlotThatHasPiece(piece.pieceType);
        var requiredStepFromLastSlot = Slot.GetRequiredStepCount(lastSlot, requestedSlot);

        //---------------------------------------
        // handle errors
        //---------------------------------------
        if (requestedSlot.slotType == SlotType.Outside && !IsAllPiecesHome(piece.pieceType))
            return MoveError.AllPiecesNotInHome;

        if (requestedSlot.slotType == SlotType.Outside && piece.currentSlot != lastSlot && requiredStep != steps)
            return MoveError.NotEnoughSteps;

        if (requestedSlot.slotType == SlotType.Outside && piece.currentSlot == lastSlot && steps < requiredStepFromLastSlot)
            return MoveError.NotEnoughSteps;

        if (requestedSlot.slotType != SlotType.Outside && requiredStep != steps)
            return MoveError.NotEnoughSteps;

        if (IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            return MoveError.BlockedByEnemy;

        if (!IsMovingToHome(piece, requestedSlot))
            return MoveError.WrongHomeDirection;

        //---------------------------------------
        // handle actions
        //---------------------------------------
        // bear action
        if (requestedSlot.slotType == SlotType.Outside)
            action |= MoveActionTypes.Bear;

        // recover action
        else if (piece.currentSlot.slotType == SlotType.Bar)
            action |= MoveActionTypes.Recover;

        // if requested slot empty
        if (IsSlotEmpty(requestedSlot))
            action |= MoveActionTypes.Move;
        // if requested slot not empty, and requested slot is yours
        else if (IsSlotYours(requestedSlot, piece.pieceType))
            action |= MoveActionTypes.Move;
        // if requested slot not empty, and requested is not yours, and requested slot not blocked by enemy
        else if (!IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            action |= MoveActionTypes.Hit;

        return MoveError.NoError;
    }

    private static bool IsAllPiecesHome(PieceType type)
    {
        var homeSlots = Slot.GetHomeSlots(type);
        var pieces = BoardManager.instance.GetAllPiecesByType(type);
        var piecesOnBoard = pieces.TakeWhile(x => x.currentSlot != null && x.currentSlot.slotType != SlotType.Outside);

        foreach (var piece in piecesOnBoard)
        {
            if (!homeSlots.Contains(piece.currentSlot))
                return false;
        }

        return true;
    }

    public static bool IsMovingToHome(Piece piece, Slot requestedSlot)
    {
        return (piece.pieceType == PieceType.White) ?
            requestedSlot.slotId < piece.Position :
            requestedSlot.slotId > piece.Position;
    }

    public static bool IsSlotEmpty(Slot slot)
    {
        return slot.pieces.Count == 0;
    }

    public static bool IsSlotYours(Slot slot, PieceType yourType)
    {
        var yourCount = slot.GetPieceTypeCount(yourType);
        return yourCount == slot.pieces.Count;
    }

    public static bool IsSlotBlockedByEnemy(Slot slot, PieceType enemyType)
    {
        var enemyCount = slot.GetPieceTypeCount(enemyType);
        return enemyCount > 1;
    }
}