using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testObject : MonoBehaviour
{
    public Transform[] SlotTransformArray;
    public GameObject piece;
    // Start is called before the first frame update
    void Start()
    {
        piece.GetComponent<Piece>().PlaceOn(SlotTransformArray[5], GameObject.Find("Slot #6").GetComponent<Slot>(), 2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
