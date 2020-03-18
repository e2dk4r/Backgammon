using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player playerWhite;
    public Player playerBlack;
    public Player currentPlayer;
    public Player turnPlayer;

    public Button undoButton;
    public Button nextTurnButton;
    public Image turnColorImage;

    #region Unity API

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playerWhite = new Player { id = 0, pieceType = PieceType.White };
        playerBlack = new Player { id = 1, pieceType = PieceType.Black };

        nextTurnButton.onClick.AddListener(() =>
        {
            if (!turnPlayer.rolledDice)
            {
                Debug.LogError("You have to roll the dice");
                return;
            }
           
            if (turnPlayer.IsMoveLeft())
            {
                Debug.LogError("You have to move");
                return;
            }

            NextTurn();
        });
        undoButton.onClick.AddListener(UndoPiece);
    }

    private void Start()
    {
        if (PlayerPrefs.GetString(Constants.PREF_CURRENT_PLAYER) == Constants.PREF_CURRENT_PLAYER1)
            currentPlayer = playerWhite;
        else
            currentPlayer = playerBlack;

        turnPlayer = currentPlayer;
        HideCurrentDice();
        UpdateTurnColor();
    }

    #endregion

    private void HideCurrentDice()
    {
        BoardManager.instance.currentDice.gameObject.SetActive(false);
    }

    private void UpdateTurnColor()
    {
        turnColorImage.color = currentPlayer.pieceType == PieceType.White ? Color.white : Color.black;
    }

    public void NextTurn()
    {
        //--------------------------------
        // reset current player's fields
        //--------------------------------
        // flush moves log
        turnPlayer.movesPlayed.Clear();

        // reset dice
        turnPlayer.rolledDice = false;
        HideCurrentDice();

        //--------------------------------
        // turn the set to the next player
        //--------------------------------
        if (turnPlayer.pieceType == PieceType.White)
        {
            turnPlayer = playerBlack;
            currentPlayer = turnPlayer;
        }
        else
        {
            turnPlayer = playerWhite;
            currentPlayer = turnPlayer;
        }

        //
        UpdateTurnColor();
    }

    private void UndoPiece()
    {
        if (currentPlayer.movesPlayed.Count == 0)
        {
            Debug.LogError("You must have played a move for undo");
            return;
        }

        // TODO: undo move
        // TODO: undo hit action
        var lastMove = currentPlayer.movesPlayed.Last();
        var lastIndex = lastMove.from.slotType != SlotType.Bar ? 
            lastMove.from.pieces.Count :
            0;

        // undo move action
        lastMove.piece.PlaceOn(lastMove.from, lastIndex);

        // undo hit action
        if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
            var enemyPiece = enemyBar.pieces.Last();
            enemyPiece.PlaceOn(lastMove.to, lastMove.to.pieces.Count);
        }

        currentPlayer.movesPlayed.Remove(lastMove);
    }
}
