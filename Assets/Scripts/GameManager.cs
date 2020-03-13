using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            // TODO: error if player does not played their move
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
        currentPlayer = playerWhite;
        turnPlayer = currentPlayer;
        UpdateTurnColor();
    }

    #endregion

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
        // TODO: undo move
    }
}
