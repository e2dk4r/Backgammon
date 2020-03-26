using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Button playButton;

    [Header("Animation")]
    public DiceSingle blackDicePrefab;
    public DiceSingle whiteDicePrefab;

    private ICollection<DiceSingle> generatedDices;
    private const float DICEGEN_X = 6;
    private const float DICEGEN_Y = 3;
    private const float DICE_REROLL = .75f;
    private const int DICE_MAX_LIMIT = 5;
    private float diceTimer = 0f;
    private DiceSingle currentDice;
   

    private void Awake()
    {
        generatedDices = new List<DiceSingle>();
        playButton.onClick.AddListener(PlayGame);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (currentDice == null && generatedDices.Count < DICE_MAX_LIMIT)
        {
            var dicePrefab = (generatedDices.Count & 1) == 0 ? blackDicePrefab : whiteDicePrefab;
            var (location, direction) = GenerateRandom();

            dicePrefab.rollSpeed = Random.Range(1f, 5f);
            dicePrefab.moveSpeed = UnityEngine.Random.Range(3f, 6f);
            dicePrefab.direction = direction;

            currentDice = Instantiate(dicePrefab, location, Quaternion.identity);
            currentDice.RollAgain();
            generatedDices.Add(currentDice);
        }

        if (currentDice != null && diceTimer > 0f)
        {
            diceTimer -= Time.deltaTime;
        }

        if (currentDice != null && currentDice.animationFinished)
        {
            diceTimer = DICE_REROLL;
            currentDice = null;
        }

        if (generatedDices.Count >= DICE_MAX_LIMIT)
        {
            KillGeneratedDices();
        }
    }

    private void KillGeneratedDices()
    {
        foreach(var dice in generatedDices)
            Destroy(dice.gameObject);

        generatedDices.Clear();
        currentDice = null;
        diceTimer = 0f;
    }

    private (Vector3, Vector2) GenerateRandom()
    {
        var pos = new Vector3(UnityEngine.Random.Range(-DICEGEN_X, DICEGEN_X), UnityEngine.Random.Range(-DICEGEN_Y, DICEGEN_Y), 0);
        var dir = new Vector2(pos.x * -1, pos.y * -1);
        return (pos, dir);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(Constants.SCENE_WHO_IS_FIRST);
    }
}
