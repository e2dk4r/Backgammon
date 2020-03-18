using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameEndScreen;

    public Player playerWhite;
    public Player playerBlack;
    public Player currentPlayer;
    public Player turnPlayer;
    public Player playerWonRound;

    public Button undoButton;
    public Button nextTurnButton;
    public Image turnColorImage;

    private const int ROUND_LIMIT = 3;
    private int currentRound = 1;

    #region Unity API

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playerWhite = new Player { id = 0, pieceType = PieceType.White };
        playerBlack = new Player { id = 1, pieceType = PieceType.Black };

        gameEndScreen.transform.Find(UI_BUTTON_NEXT_ROUND).GetComponent<Button>().onClick.AddListener(OnNextRoundButtonClick);
        nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
        undoButton.onClick.AddListener(UndoPiece);
    }

    private void Start()
    {
        HideGameEndScreen();

        if (PlayerPrefs.GetString(Constants.PREF_CURRENT_PLAYER) == Constants.PREF_CURRENT_PLAYER1)
            currentPlayer = playerWhite;
        else
            currentPlayer = playerBlack;

        turnPlayer = currentPlayer;
        HideCurrentDice();
        UpdateTurnColor();
    }

    #endregion

    #region UI
    private const string UI_TEXT_ROUND = "RoundText";
    private const string UI_PANEL_SCORE = "GameScore";
    private const string UI_PANEL_SCORE_PLAYER_WHITE = "PlayerWhite";
    private const string UI_PANEL_SCORE_PLAYER_BLACK = "PlayerBlack";
    private const string UI_TEXT_SCORE = "Score";
    private const string UI_BUTTON_NEXT_ROUND = "NextRoundButton";

    private void OnNextTurnButtonClick()
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
    }

    private void OnNextRoundButtonClick()
    {
        // increment current round
        currentRound++;

        // reset players
        Player.ResetForNextRound(playerWhite);
        Player.ResetForNextRound(playerBlack);

        // who wins the round starts first
        turnPlayer = playerWonRound;
        playerWonRound = null;
        currentPlayer = turnPlayer;

        RestartBoard();
        HideGameEndScreen();
    }

    private void UpdateGameEndScreen()
    {
        // get ui elements
        var roundText = gameEndScreen.transform.Find(UI_TEXT_ROUND).GetComponent<Text>();
        var playerWhiteWinText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_WHITE).Find(UI_TEXT_SCORE).GetComponent<Text>();
        var playerBlackWinText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_BLACK).Find(UI_TEXT_SCORE).GetComponent<Text>();

        // update ui elements
        if (currentRound != ROUND_LIMIT)
            roundText.text = $"Round { currentRound }";
        else
            roundText.text = $"Player { Player.Winner(playerWhite, playerBlack).id } won";

        playerWhiteWinText.text = playerWhite.winRound.ToString();
        playerBlackWinText.text = playerBlack.winRound.ToString();
    }
    
    private void ShowGameEndScreen()
    {
        // update game end screen
        UpdateGameEndScreen();

        // enable game end screen
        gameEndScreen.SetActive(true);
    }

    private void HideGameEndScreen()
    {
        // disable game end screen
        gameEndScreen.SetActive(false);
    }

    #endregion

    #region Public

    public void CheckRoundFinish()
    {
        if (IsFinished())
        {
            // increment won round of player
            playerWonRound.winRound++;

            ShowGameEndScreen();
        }
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
        ResetDice();

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

    private bool IsFinished()
    {
        var whiteFinished = Slot.GetOutside(PieceType.White).pieces.Count == 15;
        var blackFinished = Slot.GetOutside(PieceType.Black).pieces.Count == 15;

        if (whiteFinished)
            playerWonRound = playerWhite;

        if (blackFinished)
            playerWonRound = playerBlack;

        if (whiteFinished || blackFinished)
            return true;

        return false;
    }

    private void RestartBoard()
    {
        ResetDice();

        BoardManager.instance.ResetBoard();

        // reset pieces
        BoardManager.instance.PlacePiecesOnBoard();
    }

    private void ResetDice()
    {
        turnPlayer.rolledDice = false;
        HideCurrentDice();
    }

    private void UndoPiece()
    {
        if (currentPlayer.movesPlayed.Count == 0)
        {
            Debug.LogError("You must have played a move for undo");
            return;
        }

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
