using System.Linq;
using System.Collections.Generic;
using System;

public class Player
{
    // player's identifier
    public int id;
    // player's piece type
    public PieceType pieceType;
    // flag that indicates player rolled the dice
    public bool rolledDice = false;
    // list of player's moves for undo
    public List<Move> movesPlayed = new List<Move>();
    // round count that won
    public int score;

    public bool IsMoveLeft()
    {
        if (!IsPlayableMoveExist())
            return false;

        var dice = BoardManager.instance.currentDice;
        var moveWeight = movesPlayed.Sum(x => x.step);
        
        if (dice.GetWeight() == moveWeight)
            return false;

        return true;
    }

    private bool IsPlayableMoveExist()
    {
        // current dice of player
        var dice = BoardManager.instance.currentDice;
        // get moves left
        var movesLeft = dice.GetMovesLeftList(movesPlayed.Select(x => x.step));

        // all pieces that player can move
        IEnumerable<Piece> pieces = null;
        var bar = Slot.GetBar(pieceType);
        if (bar.pieces.Count != 0)
            pieces = bar.pieces;
        else
            pieces = BoardManager.instance.GetAllPiecesByType(pieceType).Where(x => x.currentSlot.slotType != SlotType.Outside);

        foreach (var step in movesLeft)
        {
            foreach (var piece in pieces)
            {
                foreach (var slot in piece.GetForwardSlots())
                {
                    MoveActionTypes action;
                    var error = Rule.ValidateMove(piece, slot, step, out action);

                    if (error == MoveError.NoError)
                        return true;
                }
            }
        }

        return false;
    }

    public static Player Winner(Player lhs, Player rhs)
    {
        return (lhs.score > rhs.score) ? lhs : rhs;
    }

    public static void ResetForNextRound(Player player)
    {
        player.movesPlayed.Clear();
        player.rolledDice = false;
    }
}

