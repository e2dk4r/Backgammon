using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //----------------------------
    // identifier fields
    //----------------------------
    public int pieceId;
    public PieceType pieceType;

    //----------------------------
    // identifier properties
    //----------------------------

    public int Position
    {
        get
        {
            if (currentSlot == null)
                return -1;
            return currentSlot.slotId;
        }
    }
    public int DenormalizedPosition
    {
        get
        {
            return pieceType == PieceType.Black ? 24 - Position : Position;
        }
    }

    //----------------------------
    // for touch and drag
    //----------------------------

    // for returning from invalid move
    private Vector2 startPos;

    // flag indicating moving this piece
    private bool isBeingHeld = false;

    [HideInInspector]
    public Slot currentSlot = null;
    private Slot collisionSlot = null;

    #region Unity API
    void Update()
    {
        // TODO: change input touch drag and drop
        // if touch begins
        if (Input.GetButtonDown("Fire1") && IsMouseOverThis())
        {
            OnPieceHold();
        }
        if (isBeingHeld && Input.GetButtonUp("Fire1"))
        {
            OnPieceRelease();
        }

        if (isBeingHeld)
        {
            var mousePos = GetMousePos();

            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // check collision with slot
        var slot = collision.gameObject.GetComponent<Slot>();
        if (slot != null)
        {
            Debug.Log($"Piece #{ pieceId } in Slot #{ slot.slotId }");
            collisionSlot = slot;
        }
    }

    #endregion

    private void OnPieceHold()
    {
        // TODO: if it is not top piece
        // TODO: if there is piece on bar, it must be placed on first
        // TODO: if player has no moves

        if (!Slot.IsTopPiece(currentSlot, this))
        {
            Debug.LogError("Piece is not top of the stack");
            isBeingHeld = false;
        }
        else
        {
            // hold the piece
            isBeingHeld = true;

            // store current position for invalid move
            startPos.x = transform.position.x;
            startPos.y = transform.position.y;
        }
    }

    private void OnPieceRelease()
    {
        isBeingHeld = false;

        // if collision not happen
        if (collisionSlot == null)
        {
            // reset the position
            ResetToOldPosition();
        }
        else
        {
            MoveActionTypes action;
            var error = Rule.ValidateMove(this, collisionSlot, 1, out action);
            if (error == MoveError.NoError)
            {
                Debug.LogWarning(action);
                PlaceOn(BoardManager.instance.slotArray[collisionSlot.slotId - 1].transform, collisionSlot, collisionSlot.pieces.Count);
            }
            else
            {
                ResetToOldPosition();
                Debug.LogError(error);
            }
            collisionSlot = null;
        }
    }

    private bool IsMouseOverThis()
    {
        var hit = Physics2D.Raycast(GetMousePos(), Vector2.zero, 0, Constants.LAYER_PIECE);
        if (hit.collider != null)
        {
            print(hit.collider.name);
            if (hit.collider.name == name)
                return true;
        }
        return false;
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        return new Vector2(mousePos.x, mousePos.y);
    }

    private void ResetToOldPosition()
    {
        transform.position = new Vector3(startPos.x, startPos.y, 0);
    }

    public void PlaceOn(Transform slotPos, Slot slot, int index)
    {
        //-------------------------------------------------
        // calculate offset of y value
        //-------------------------------------------------
        float posY = index * .7f;
        // if slot is on top region
        if (slot.slotId >= 13 && slot.slotId <= 24)
            posY *= -1;

        //-------------------------------------------------
        // change slot
        //-------------------------------------------------
        // if slot has been in another slot
        if (currentSlot != null)
            currentSlot.pieces.Remove(this);

        // change current slot
        currentSlot = slot;

        // add piece to slot
        if (!slot.pieces.Contains(this))
            slot.pieces.Add(this);

        //-------------------------------------------------
        // update ui of piece
        //-------------------------------------------------
        transform.parent = slotPos;
        transform.localPosition = new Vector3(0, posY, 0);
    }

    #region Static Methods

    public static Piece CreateFromPrefab(GameObject prefab, int id, PieceObject pieceObject)
    {
        var go = Instantiate(prefab);

        // set sprite
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = pieceObject.sprite;

        // set piece parameters
        var piece = go.GetComponent<Piece>();
        piece.name = $"Piece #{ id }";
        piece.pieceId = id;
        piece.pieceType = pieceObject.pieceType;

        return piece;
    }


    public static PieceType GetEnemyType(PieceType you)
    {
        if (you == PieceType.White)
            return PieceType.Black;

        return PieceType.White;
    }

    #endregion
}
