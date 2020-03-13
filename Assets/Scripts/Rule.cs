using System;

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

        //---------------------------------------
        // handle errors
        //---------------------------------------
        if (Slot.GetRequiredStepCount(piece.currentSlot, requestedSlot) != steps)
            return MoveError.NotEnoughSteps;

        if (IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            return MoveError.BlockedByEnemy;

        if (!IsMovingToHome(piece, requestedSlot))
            return MoveError.WrongHomeDirection;

        //---------------------------------------
        // handle actions
        //---------------------------------------
        // TODO: Bear action
        if (requestedSlot.slotType == SlotType.Outside)
            action = MoveActionTypes.Bear;

        else if (piece.currentSlot.slotType == SlotType.Bar)
            action = MoveActionTypes.Recover;

        else if (IsSlotEmpty(requestedSlot))
            action = MoveActionTypes.Move;

        else if (IsSlotYours(requestedSlot, piece.pieceType))
            action = MoveActionTypes.Move;

        else if (!IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            action = MoveActionTypes.Hit;
            

        return MoveError.NoError;
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