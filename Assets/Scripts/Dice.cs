using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [HideInInspector]
    public int value;

    #region Unity API

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    #endregion

    public void Roll()
    {
        value = Random.Range(1, 6);
    }
}
