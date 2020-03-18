//#define TEST_OUTSIDE_WHITE
#define TEST_OUTSIDE_BLACK

#if TEST_OUTSIDE_WHITE || TEST_OUTSIDE_BLACK
    #define TEST
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public Dice currentDice;
    public Button RollDiceButton;

    private Piece[] pieces = new Piece[30];
    private void Awake()
    {
        if (instance == null)
            instance = this;

        RollDiceButton.onClick.AddListener(RollDices);

        InitializePieces();
        PlacePiecesOnBoard();
    }

    public void PlacePiecesOnBoard()
    {
        //------------------------------------
        // place whites on board
        //------------------------------------
        // white index starts from 0
        int index = 0;

        // place 5 whites to slot 6, total = 5
        for (int i = 0; i < 5; i++, index++)
        {

#if !(TEST)
            var slotId = 6;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 6;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), slotArray[slotId - 1].GetComponent<Slot>().pieces.Count);
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>(), 0);
#endif

        }

        // place 3 whites to slot 8, total = 8
        for (int i = 0; i < 3; i++, index++)
        {

#if !(TEST)
            var slotId = 8;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 3;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), slotArray[slotId - 1].GetComponent<Slot>().pieces.Count);
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>(), 0);
#endif

        }

        // place 5 whites to slot 13, total = 13
        for (int i = 0; i < 5; i++, index++)
        {

#if !(TEST)
            var slotId = 13;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 2;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), slotArray[slotId - 1].GetComponent<Slot>().pieces.Count);
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>(), 0);
#endif

        }

        // place 2 whites to slot 24, total = 15
        for (int i = 0; i < 2; i++, index++)
        {

#if !(TEST)
            var slotId = 24;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 1;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), slotArray[slotId - 1].GetComponent<Slot>().pieces.Count);
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>(), 0);
#endif

        }

        //------------------------------------
        // place blacks on board
        //------------------------------------
        // black index starts from 15
        index = 15;

        // place 2 blacks to slot 1, total = 2
        for (int i = 0; i < 2; i++, index++)
        {
#if !(TEST)
            var slotId = 1;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 19;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            pieces[index].PlaceOn(blackBar.GetComponent<Slot>(), 0);
#endif
        }

        // place 5 blacks to slot 12, total = 7
        for (int i = 0; i < 5; i++, index++)
        {
#if !(TEST)
            var slotId = 12;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 20;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            pieces[index].PlaceOn(blackBar.GetComponent<Slot>(), 0);
#endif
        }

        // place 3 blacks to slot 17, total = 10
        for (int i = 0; i < 3; i++, index++)
        {
#if !(TEST)
            var slotId = 17;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 21;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            pieces[index].PlaceOn(blackBar.GetComponent<Slot>(), 0);
#endif
        }

        // place 5 blacks to slot 19, total = 15
        for (int i = 0; i < 5; i++, index++)
        {
#if !(TEST)
            var slotId = 19;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 22;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>(), i);
#elif (TEST && TEST_OUTSIDE_WHITE)
            pieces[index].PlaceOn(blackBar.GetComponent<Slot>(), 0);
#endif
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

    private void RollDices()
    {
        if (!IsCurrentPlayerRolledDice())
        {
            currentDice.Roll();
            currentDice.gameObject.SetActive(true);
            GameManager.instance.currentPlayer.rolledDice = true;
        }
        else
        {
            Debug.LogError("Current player rolled the dice");
        }
    }

    private static bool IsCurrentPlayerRolledDice()
    {
        return GameManager.instance.currentPlayer.rolledDice;
    }

    public IEnumerable<Piece> GetAllPiecesByType(PieceType type)
    {
        return pieces.Where(x => x.pieceType == type);
    }
}
