using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WhoIsFirst : MonoBehaviour
{
    public DiceSingle dicePlayer1;
    public DiceSingle dicePlayer2;

    public GameObject infoScreen;
    public GameObject rollScreen;

    private Vector2 startPos1;
    private Vector2 startPos2;

    private Button rollButton;
    private Text infoText;
    private Text countdownText;
    private bool countdownStarted = false;
    private bool countdownFinished = false;
    private float countdownTimer = 3f;

    private const string INFO_TEXT = "InfoText";
    private const string COUNTDOWN_TEXT = "CountdownText";
    private const string ROLL_BUTTON = "RollButton";

    #region Unity API

    private void Awake()
    {
        infoText = infoScreen.transform.Find(INFO_TEXT).GetComponent<Text>();
        countdownText = infoScreen.transform.Find(COUNTDOWN_TEXT).GetComponent<Text>();

        rollButton = rollScreen.transform.Find(ROLL_BUTTON).GetComponent<Button>();
        rollButton.onClick.AddListener(RollDicesAgain);

        startPos1 = new Vector2(dicePlayer1.transform.position.x, dicePlayer1.transform.position.y);
        startPos2 = new Vector2(dicePlayer2.transform.position.x, dicePlayer2.transform.position.y);
    }

    private void Start()
    {
        infoScreen.SetActive(false);
        rollScreen.SetActive(true);
    }

    private void Update()
    {
        if (dicePlayer1.animationFinished)
            AfterDicesRolled();

        if (countdownStarted && !countdownFinished)
        {
            countdownTimer -= Time.deltaTime;
            UpdateCountdownText();

            if (countdownTimer <= 0)
            {
                countdownFinished = true;
                AfterCountdownFinished();
            }
        }
    }

    #endregion

    private void UpdateCountdownText()
    {
        countdownText.text = $"{ Mathf.CeilToInt(countdownTimer) }";
    }

    private void AfterDicesRolled()
    {
        if (dicePlayer1.value == dicePlayer2.value)
        {
            rollScreen.SetActive(true);
        }
        else
        {
            infoScreen.SetActive(true);
            UpdateInfoText();
            countdownStarted = true;
        }
    }

    private void AfterCountdownFinished()
    {
        Debug.Log("Switch");
        if (dicePlayer1.value > dicePlayer2.value)
            PlayerPrefs.SetString(Constants.PREF_CURRENT_PLAYER, Constants.PREF_CURRENT_PLAYER1);
        else
            PlayerPrefs.SetString(Constants.PREF_CURRENT_PLAYER, Constants.PREF_CURRENT_PLAYER2);

        SceneManager.LoadScene(Constants.SCENE_GAME);
    }

    private void UpdateInfoText()
    {
        if (dicePlayer1.value > dicePlayer2.value)
            infoText.text = "Player 1 starts in";
        else
            infoText.text = "Player 2 starts in";
    }

    private void RollDicesAgain()
    {
        rollScreen.SetActive(false);

        dicePlayer1.RollAgain(startPos1);
        dicePlayer2.RollAgain(startPos2);
    }
}
