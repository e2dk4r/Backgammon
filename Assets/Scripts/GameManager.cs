using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    public Button rollButton;
    public Image firstDiceValueImage;
    public Image secondDiceValueImage;

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
        rollButton.onClick.AddListener(RollDices);
    }

    private void Start()
    {
        HideGameEndScreen();

        if (PlayerPrefs.GetString(Constants.PREF_CURRENT_PLAYER) == Constants.PREF_CURRENT_PLAYER1)
            currentPlayer = playerWhite;
        else
            currentPlayer = playerBlack;

        turnPlayer = currentPlayer;
        HideDiceValues();
    }

    private void Update()
    {
        if (DiceController.instance.animationStarted && !DiceController.instance.animationFinished)
        {
            ShowDiceValues();
        }
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
        if (IsAnyPlayerWon())
            SceneManager.LoadScene(Constants.SCENE_WHO_IS_FIRST);

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
        var playerWhiteScoreText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_WHITE).Find(UI_TEXT_SCORE).GetComponent<Text>();
        var playerBlackScoreText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_BLACK).Find(UI_TEXT_SCORE).GetComponent<Text>();

        // update ui elements
        if (!IsAnyPlayerWon())
            roundText.text = $"Round { currentRound }";
        else
            roundText.text = $"Player { Player.Winner(playerWhite, playerBlack).id } won";

        playerWhiteScoreText.text = playerWhite.score.ToString();
        playerBlackScoreText.text = playerBlack.score.ToString();
    }

    private bool IsAnyPlayerWon()
    {
        var potentialWeight = (ROUND_LIMIT - currentRound) * 2;

        // if round is equal to limit
        if (currentRound == ROUND_LIMIT)
            return true;

        // if white player is winning, 
        // and if black player + potential still less than white player,
        // white player won
        if (playerWhite.score > playerBlack.score &&
            playerBlack.score + potentialWeight < playerWhite.score)
            return true;

        // if black player is winning, 
        // and if white player + potential still less than black player,
        // black player won
        if (playerBlack.score > playerWhite.score &&
            playerWhite.score + potentialWeight < playerBlack.score)
            return true;

        return false;
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

    private void RollDices()
    {
        if (!currentPlayer.rolledDice)
        {
            DiceController.instance.ThrowDices();
            currentPlayer.rolledDice = true;
            StartCoroutine(AfterRolledDice());
        }
        else
        {
            Debug.LogError("Current player rolled the dice");
        }
    }

    private IEnumerator AfterRolledDice()
    {
        if (!currentPlayer.IsMoveLeft())
        {
            yield return new WaitForSeconds(2f);
            NextTurn();
        }
    }

    private void HideDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(false);
        secondDiceValueImage.gameObject.SetActive(false);
    }

    private void ShowDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(true);
        secondDiceValueImage.gameObject.SetActive(true);

        firstDiceValueImage.sprite = DiceController.instance.firstValueSprite;
        secondDiceValueImage.sprite = DiceController.instance.secondValueSprite;
    }

    #endregion

    #region Public

    public void CheckRoundFinish()
    {
        if (IsFinished())
        {
            var score = CalculateScore();
            // increment won round of player
            playerWonRound.score += score;

            ShowGameEndScreen();
        }
    }

    #endregion

    private int CalculateScore()
    {
        var enemyOutside = (Piece.GetEnemyType(playerWonRound.pieceType) == PieceType.White) ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return (enemyOutside.pieces.Count == 0) ? 2 : 1;
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
        HideDiceValues();
    }

    private void UndoPiece()
    {
        if (currentPlayer.movesPlayed.Count == 0)
        {
            Debug.LogError("You must have played a move for undo");
            return;
        }

        var lastMove = currentPlayer.movesPlayed.Last();

        // undo move action
        lastMove.piece.PlaceOn(lastMove.from);

        // undo hit action
        if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
            var enemyPiece = enemyBar.pieces.Last();
            enemyPiece.PlaceOn(lastMove.to);
        }

        currentPlayer.movesPlayed.Remove(lastMove);
    }
}
