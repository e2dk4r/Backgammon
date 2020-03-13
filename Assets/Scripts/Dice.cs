using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Sprite[] diceSprites;
    public int[] values = new int[2];

    private SpriteRenderer firstValueSpriteRenderer;
    private SpriteRenderer secondValueSpriteRenderer;

    #region Unity API

    private void Awake()
    {
        firstValueSpriteRenderer = transform.Find("Value1").GetComponent<SpriteRenderer>();
        secondValueSpriteRenderer = transform.Find("Value2").GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Draw Methods

    private void DisplayValue()
    {
        firstValueSpriteRenderer.sprite = diceSprites[values[0] - 1];
        secondValueSpriteRenderer.sprite = diceSprites[values[1] - 1];
    }

    #endregion

    public void Roll()
    {
        values[0] = UnityEngine.Random.Range(1, 7);
        values[1] = UnityEngine.Random.Range(1, 7);

        AfterRolled();
    }

    private void AfterRolled()
    {
        // display the values
        DisplayValue();

        // check if there is no move left for player
        var turnPlayer = GameManager.instance.turnPlayer;
        if (turnPlayer != null && !turnPlayer.IsMoveLeft())
            GameManager.instance.NextTurn();
    }

    public bool IsDoubleMove()
    {
        return values[0] == values[1];
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
}
