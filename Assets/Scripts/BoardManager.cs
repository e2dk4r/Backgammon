using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public GameObject[] slotArray;

    public GameObject whiteBar;
    public GameObject blackBar;

    public GameObject whiteOutside;
    public GameObject blackOutside;

    public GameObject piecePrefab;
    public PieceObject blackPieceObject;
    public PieceObject whitePieceObject;

    private Piece[] pieces = new Piece[30];
    private void Awake()
    {
        if (instance == null)
            instance = this;

        InitializePieces();
        PlacePiecesOnBoard();
    }

    private void PlacePiecesOnBoard()
    {
        //------------------------------------
        // place whites on board
        //------------------------------------
        // white index starts from 0
        int index = 0;

        // place 5 whites to slot 6, total = 5
        for (int i = 0; i < 5; i++, index++)
        {
            var slotId = 6;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 3 whites to slot 8, total = 8
        for (int i = 0; i < 3; i++, index++)
        {
            var slotId = 8;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 5 whites to slot 13, total = 13
        for (int i = 0; i < 5; i++, index++)
        {
            var slotId = 13;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 2 whites to slot 24, total = 15
        for (int i = 0; i < 2; i++, index++)
        {
            var slotId = 24;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        //------------------------------------
        // place blacks on board
        //------------------------------------
        // black index starts from 15
        index = 15;

        // place 2 blacks to slot 1, total = 2
        for (int i = 0; i < 2; i++, index++)
        {
            var slotId = 1;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 5 blacks to slot 12, total = 7
        for (int i = 0; i < 5; i++, index++)
        {
            var slotId = 12;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 3 blacks to slot 17, total = 10
        for (int i = 0; i < 3; i++, index++)
        {
            var slotId = 17;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        // place 5 blacks to slot 19, total = 15
        for (int i = 0; i < 5; i++, index++)
        {
            var slotId = 19;
            pieces[index].PlaceOn(slotArray[slotId - 1].transform, slotArray[slotId - 1].GetComponent<Slot>(), i);
        }

        
    }

    private void InitializePieces()
    {
        for (int i = 0; i < 30; i++)
        {
            var pieceObject = (i < 15) ? whitePieceObject : blackPieceObject;
            var piece = Piece.CreateFromPrefab(piecePrefab, i + 1, pieceObject);
            pieces[i] = piece;
        }
    }
}
