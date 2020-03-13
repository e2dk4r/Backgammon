using System;

public enum MoveError
{
    // no error
    NoError,
    // there is not enough step value to move in
    NotEnoughSteps,
    // unknown error
    Unknown,
    BlockedByEnemy,
    WrongHomeDirection,
}
public enum MoveActionTypes
{
    // move piece from one point to another
    Move,
    // recover piece from bar and place it on board
    Recover,
    // hit opponent's piece and send it to bar
    Hit,
    // move outside the board
    Bear,
}

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

        // required before anything
        if (Slot.GetRequiredStepCount(piece.currentSlot, requestedSlot) != steps)
            return MoveError.NotEnoughSteps;

        if (IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            return MoveError.BlockedByEnemy;

        if (!IsMovingToHome(piece, requestedSlot))
            return MoveError.WrongHomeDirection;

        if (IsSlotEmpty(requestedSlot))
            action = MoveActionTypes.Move;

        else if (IsSlotYours(requestedSlot, piece.pieceType))
            action = MoveActionTypes.Move;

        else if (!IsSlotBlockedByEnemy(requestedSlot, Piece.GetEnemyType(piece.pieceType)))
            action = MoveActionTypes.Hit;

        // TODO: Bear action

        return MoveError.NoError;
    }

    private static bool IsMovingToHome(Piece piece, Slot requestedSlot)
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