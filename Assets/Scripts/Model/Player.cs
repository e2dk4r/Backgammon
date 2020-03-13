using System.Linq;
using System.Collections.Generic;

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

    public bool IsMoveLeft()
    {
        var dice = BoardManager.instance.currentDice;

        if (!IsPlayableMoveExist())
            return false;

        if (dice.IsDoubleMove() && movesPlayed.Count == 4)
            return false;
        
        if (!dice.IsDoubleMove() && movesPlayed.Count == 2)
            return false;

        return true;
    }

    private bool IsPlayableMoveExist()
    {
        // current dice of player
        var dice = BoardManager.instance.currentDice;
        // get moves left
        var movesLeft = dice.GetMovesLeftList(movesPlayed.Select(x => x.step));
        // all pieces that player owns
        var pieces = BoardManager.instance.GetAllPiecesByType(pieceType);

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
}

