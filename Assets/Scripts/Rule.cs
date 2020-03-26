using System.Collections.Generic;
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

        //---------------------------------------
        // handle errors
        //---------------------------------------
        if (requestedSlot.slotType == SlotType.Outside && !IsAllPiecesHome(piece.pieceType))
            return MoveError.AllPiecesNotInHome;

        if (requestedSlot.slotType == SlotType.Outside)
        {
            var lastSlot = Slot.GetLastSlotThatHasPiece(piece.pieceType);
            var requiredStepFromLastSlot = Slot.GetRequiredStepCount(lastSlot, requestedSlot);

            if (piece.currentSlot != lastSlot && requiredStep != steps)
                return MoveError.NotEnoughSteps;

            if (piece.currentSlot == lastSlot && steps < requiredStepFromLastSlot)
                return MoveError.NotEnoughSteps;
        }

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

    public static MoveError ValidateCombinedMove(Piece piece, Slot requestedSlot, IEnumerable<int> steps, out ICollection<Move> movesPlayed)
    {
        movesPlayed = new List<Move>();

        // combined move must at least be 2
        if (steps.Count() < 2)
            return MoveError.NotEnoughSteps;

        var requiredStep = Slot.GetRequiredStepCount(piece.currentSlot, requestedSlot);
        var stepsWeight = steps.Sum();
        var forwardSlots = piece.GetForwardSlots();
        var isMovesEqual = steps.First() == steps.Last();

        if (requiredStep < 1)
            return MoveError.WrongHomeDirection;

        // are moveable steps enough?
        if (stepsWeight < requiredStep)
            return MoveError.NotEnoughSteps;

        // create referance piece for test
        var pieceRef = Piece.CreateEmpty();

        if (pieceRef == null)
            return MoveError.Unknown;

        pieceRef.pieceId = -1;
        pieceRef.pieceType = piece.pieceType;
        pieceRef.currentSlot = piece.currentSlot;

        // test each moveable step
        MoveError error = MoveError.Unknown;
        foreach (var step in steps)
        {
            var stepsPlayed = movesPlayed.Select(x => x.step);
            var nextSlot = forwardSlots.Skip((stepsPlayed.Sum() + step) - 1).FirstOrDefault();

            if (nextSlot == null)
                return MoveError.Unknown;

            MoveActionTypes action;
            error = ValidateMove(pieceRef, nextSlot, step, out action);

            if (error == MoveError.NoError)
            {
                // add move to played list
                // - add step
                // - add action that occurred when moving
                movesPlayed.Add(new Move
                {
                    from = pieceRef.currentSlot,
                    to = nextSlot,
                    step = step,
                    action = action,
                });

                // move referance piece to next slot
                pieceRef.currentSlot = nextSlot;

                // if we achived destination, stop1
                if (nextSlot == requestedSlot)
                    break;
            }
            else
            {
                break;
            }

            if (Slot.GetBar(pieceRef.pieceType).pieces.Count - movesPlayed.Count > 0)
                break;
        }

        // if any error happened and moves are not equal
        if (error != MoveError.NoError && !isMovesEqual)
        {
            // reset variables
            pieceRef.currentSlot = piece.currentSlot;
            movesPlayed.Clear();

            // test each moveable step, but reversed
            foreach (var step in steps.Reverse())
            {
                var stepsPlayed = movesPlayed.Select(x => x.step);
                var nextSlot = forwardSlots.Skip((stepsPlayed.Sum() + step) - 1).First();

                MoveActionTypes action;
                error = ValidateMove(pieceRef, nextSlot, step, out action);

                if (error == MoveError.NoError)
                {
                    // add move to played list
                    // - add step
                    // - add action that occurred when moving
                    movesPlayed.Add(new Move
                    {
                        from = pieceRef.currentSlot,
                        to = nextSlot,
                        step = step,
                        action = action,
                    });

                    // move referance piece to next slot
                    pieceRef.currentSlot = nextSlot;

                    // if we achived destination, stop
                    if (nextSlot == requestedSlot)
                        break;
                }
                else
                {
                    break;
                }

                if (Slot.GetBar(pieceRef.pieceType).pieces.Count - movesPlayed.Count > 0)
                    break;
            }
        }

        Object.Destroy(pieceRef.gameObject);

        if (movesPlayed.Count != 0 && movesPlayed.Last().to != requestedSlot)
            return MoveError.Unknown;


        return error;
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

    private static bool IsBarEmpty(PieceType type)
    {
        var bar = (type == PieceType.White)
            ? BoardManager.instance.whiteBar.GetComponent<Slot>()
            : BoardManager.instance.blackBar.GetComponent<Slot>();

        return bar.pieces.Count == 0;
    }
}