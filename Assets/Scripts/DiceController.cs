using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class DiceController : MonoBehaviour
{
    public static DiceController instance;

    [Header("Prefabs")]
    [SerializeField]
    private Dice whiteDicePrefab;
    [SerializeField]
    private Dice blackDicePrefab;

    [Header("Vectors")]
    [SerializeField]
    private ThrowLocation[] throwLocations;

    [HideInInspector]
    public Sprite firstValueSprite;
    [HideInInspector]
    public Sprite secondValueSprite;

    // 2 white and 2 black dices
    private Dice[] dices = new Dice[4];
    private IEnumerable<Dice> rollingDices = null;
    public bool animationStarted = false;
    public bool animationFinished = false;
    public int[] values = new int[2];

    private IEnumerable<Dice> WhiteDices
    {
        get { return dices.Take(2); }
    }

    private IEnumerable<Dice> BlackDices
    {
        get { return dices.Skip(2).Take(2); }
    }

    #region Unity API

    private void Awake()
    {
        if (instance == null)
            instance = this;

        InitializeDices();
    }

    private void Update()
    {
        if (!animationStarted || animationFinished)
            return;

        var firstDice = rollingDices.First();
        var secondDice = rollingDices.Last();

        firstValueSprite = firstDice.GetComponent<SpriteRenderer>().sprite;
        secondValueSprite = secondDice.GetComponent<SpriteRenderer>().sprite;

        animationFinished = true;
        foreach (var dice in rollingDices)
            animationFinished &= dice.AnimationFinished;

        if (animationFinished)
        {
            StartCoroutine(HideRollingDices());
        }
    }

    #endregion

    #region Draw Methods

    private void ShowRollingDices()
    {
        foreach (var dice in rollingDices)
            dice.gameObject.SetActive(true);
    }

    private IEnumerator HideRollingDices()
    {
        yield return new WaitForSeconds(1f);

        foreach (var dice in rollingDices)
            dice.gameObject.SetActive(false);

        rollingDices = null;
    }

    public void ThrowDices()
    {
        var throwLocation = GetThrowLocation();
        var direction = throwLocation.direction;
        var pos = throwLocation.transform.position;
        var speed = Random.Range(3f, 7f);

        Roll();

        SetRollingDices();
        ShowRollingDices();

        var firstDice = rollingDices.First();
        var secondDice = rollingDices.Last();

        // set direction
        firstDice.direction = new Vector2(direction.x, direction.y + Random.Range(-.25f, .25f));
        secondDice.direction = new Vector2(direction.x + .25f, direction.y + Random.Range(-.25f, .25f));

        // set move speed
        firstDice.moveSpeed = speed;
        secondDice.moveSpeed = speed + Random.Range(.75f, 3f);

        // throw from position
        firstDice.Throw(values[0], pos);
        secondDice.Throw(values[1], pos + Vector3.up * (direction.y > 0 ? 1 : -1));

        // start checking animation finish
        animationStarted = true;
        animationFinished = false;
    }

    #endregion

    private void InitializeDices()
    {
        int index = 0;

        // instantiate 2 white dices
        for (int i = 0; i < 2; i++, index++)
        {
            dices[index] = Instantiate(whiteDicePrefab, Vector3.zero, Quaternion.identity);
        }

        // instantiate 2 black dices
        for (int i = 0; i < 2; i++, index++)
        {
            dices[index] = Instantiate(blackDicePrefab, Vector3.zero, Quaternion.identity);
        }

        foreach (var dice in dices)
            dice.gameObject.SetActive(false);
    }

    private ThrowLocation GetThrowLocation()
    {
        return throwLocations[Random.Range(0, throwLocations.Length)];
    }

    private void SetRollingDices()
    {
        rollingDices = GameManager.instance.turnPlayer.pieceType == PieceType.White 
            ? WhiteDices
            : BlackDices;
    }

    #region Dice Functions

#if TEST_VALUES
    static int counter = 0;
#endif
    public void Roll()
    {
#if !TEST_VALUES
        values[0] = UnityEngine.Random.Range(1, 7);
        values[1] = UnityEngine.Random.Range(1, 7);
#else
        if ((counter & 1) == 0)
        {
            values[0] = 6;
            values[1] = 6;
        }
        else
        {
            values[0] = 3;
            values[1] = 3;
        }
        counter++;
#endif

        SortValues();
    }

    private void SortValues()
    {
        System.Array.Sort(values);
    }

    public bool IsDoubleMove()
    {
        return values[0] == values[1];
    }

    public int GetWeight()
    {
        var sum = values.Sum();
        return IsDoubleMove() ? sum * 2 : sum;
    }

    public IEnumerable<int> GetMovesList()
    {
        if (!IsDoubleMove())
            return values;

        return new int[] { values[0], values[0], values[0], values[0] };
    }

    public IEnumerable<int> GetMovesLeftList(IEnumerable<int> playedSteps)
    {
        var list = GetMovesList().ToList();

        foreach (var step in playedSteps)
            list.RemoveAt(list.FindIndex(x => x == step));

        return list;
    }

    #endregion
}
