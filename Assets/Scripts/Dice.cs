//#define TEST_VALUES

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        AfterRolled();
    }

    private void AfterRolled()
    {
        SortValues();
        // display the values
        DisplayValue();
    }

    private void SortValues()
    {
        Array.Sort(values);
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
}
