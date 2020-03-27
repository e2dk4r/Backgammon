//#define TEST_OUTSIDE_WHITE
//#define TEST_OUTSIDE_BLACK
//#define TEST_ROUNDS
//#define TEST_COMBINED_MOVE
//#define TEST_COMBINED_MOVE_WITH_BAR

#if TEST_OUTSIDE_WHITE || TEST_OUTSIDE_BLACK || TEST_ROUNDS || TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR
#define TEST
#endif

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [Header("Board Items")]
    public GameObject[] slotArray;

    public GameObject whiteBar;
    public GameObject blackBar;

    public GameObject whiteOutside;
    public GameObject blackOutside;

    [Header("Piece Defaults")]
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
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 6;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>());
#elif (TEST && TEST_ROUNDS)
            pieces[index].PlaceOn(whiteOutside.GetComponent<Slot>());
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 18;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif

        }

        // place 3 whites to slot 8, total = 8
        for (int i = 0; i < 3; i++, index++)
        {

#if !(TEST)
            var slotId = 8;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 3;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>());
#elif (TEST && TEST_ROUNDS)
            pieces[index].PlaceOn(whiteOutside.GetComponent<Slot>());
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 2;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif

        }

        // place 5 whites to slot 13, total = 13
        for (int i = 0; i < 5; i++, index++)
        {

#if !(TEST)
            var slotId = 13;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 2;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>());
#elif (TEST && TEST_ROUNDS)
            pieces[index].PlaceOn(whiteOutside.GetComponent<Slot>());
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 3;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif

        }

        // place 2 whites to slot 24, total = 15
        for (int i = 0; i < 2; i++, index++)
        {

#if !(TEST)
            var slotId = 24;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slotId = 1;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            pieces[index].PlaceOn(whiteBar.GetComponent<Slot>());
#elif (TEST && TEST_ROUNDS)
            var slotId = 1;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 4;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
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
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 19;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_ROUNDS)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 24;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif
        }

        // place 5 blacks to slot 12, total = 7
        for (int i = 0; i < 5; i++, index++)
        {
#if !(TEST)
            var slotId = 12;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 20;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_ROUNDS)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 23;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif
        }

        // place 3 blacks to slot 17, total = 10
        for (int i = 0; i < 3; i++, index++)
        {
#if !(TEST)
            var slotId = 17;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 21;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_ROUNDS)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_COMBINED_MOVE)
            var slotId = 22;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_COMBINED_MOVE_WITH_BAR)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif
        }

        // place 5 blacks to slot 19, total = 15
        for (int i = 0; i < 5; i++, index++)
        {
#if !(TEST)
            var slotId = 19;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_BLACK)
            var slotId = 22;
            pieces[index].PlaceOn(slotArray[slotId - 1].GetComponent<Slot>());
#elif (TEST && TEST_OUTSIDE_WHITE)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_ROUNDS)
            var slot = blackBar.GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#elif (TEST && TEST_COMBINED_MOVE || TEST_COMBINED_MOVE_WITH_BAR)
            var slotId = 7;
            var slot = slotArray[slotId - 1].GetComponent<Slot>();
            pieces[index].PlaceOn(slot);
#endif
        }


    }

    public void ResetBoard()
    {
        foreach (var piece in pieces)
            piece.currentSlot = null;

        foreach (var slot in slotArray.Select(x => x.GetComponent<Slot>()))
            slot.pieces.Clear();

        whiteBar.GetComponent<Slot>().pieces.Clear();
        blackBar.GetComponent<Slot>().pieces.Clear();

        whiteOutside.GetComponent<Slot>().pieces.Clear();
        blackOutside.GetComponent<Slot>().pieces.Clear();
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

    public IEnumerable<Piece> GetAllPiecesByType(PieceType type)
    {
        return pieces.Where(x => x.pieceType == type);
    }
}
