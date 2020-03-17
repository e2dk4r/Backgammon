using System.Linq;
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
        if (Input.GetButtonDown("Fire1") && IsMouseOverThis() &&
            IsCurrentPlayerTurn() &&
            IsCurrentPlayerRolled() &&
            IsCurrentPlayerPiece() &&
            IsCurrentPlayerMoveLeft())
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

    #region Draw Methods

    public void PlaceOn(Slot slot, int index)
    {
        var slotPos = slot.transform;
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
        this.GetComponent<SpriteRenderer>().sortingOrder = slot.pieces.Count;
    }

    private void ResetToOldPosition()
    {
        transform.position = new Vector3(startPos.x, startPos.y, 0);
    }

    #endregion

    private bool IsCurrentPlayerMoveLeft()
    {
        return GameManager.instance.currentPlayer.IsMoveLeft();
    }

    public IEnumerable<Slot> GetForwardSlots()
    {
        if (IsOutside())
            return null;

        var list = new List<Slot>();
        var allSlots = BoardManager.instance.slotArray;
        var outside = Slot.GetOutside(pieceType);

        foreach (var slot in allSlots.Select(x => x.GetComponent<Slot>()))
        {
            if (Rule.IsMovingToHome(this, slot))
                list.Add(slot);
        }
        list.Add(outside);

        return list;
    }

    private bool IsOutside()
    {
        var list = pieceType == PieceType.White ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return list.pieces.Contains(this);
    }

    private bool IsCurrentPlayerRolled()
    {
        return GameManager.instance.currentPlayer.rolledDice;
    }

    private bool IsCurrentPlayerPiece()
    {
        return GameManager.instance.currentPlayer.pieceType == pieceType;
    }

    private bool IsCurrentPlayerTurn()
    {
        return GameManager.instance.currentPlayer == GameManager.instance.turnPlayer;
    }

    private void OnPieceHold()
    {
        // TODO: if it is not top piece
        // TODO: if there is piece on bar, it must be placed on first
        if (!GameManager.instance.currentPlayer.rolledDice)
        {
            Debug.LogError("Player is not rolled the dice");
            isBeingHeld = false;
        }
        else if (!Slot.IsTopPiece(currentSlot, this))
        {
            Debug.LogError("Piece is not top of the stack");
            isBeingHeld = false;
        }
        else if (!IsBarEmpty() && currentSlot.slotType != SlotType.Bar)
        {
            Debug.LogError("First, pieces on bar must be placed");
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

    private bool IsBarEmpty()
    {
        var barList = (GameManager.instance.currentPlayer.pieceType == PieceType.White) ?
            BoardManager.instance.whiteBar.GetComponent<Slot>().pieces :
            BoardManager.instance.blackBar.GetComponent<Slot>().pieces;

        if (barList.Count != 0)
            return false;

        return true;
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
            // TODO: if there is no other moves left for player
            // TODO: use rolled dice values

            // current dice of player
            var dice = BoardManager.instance.currentDice;
            // current player
            var currentPlayer = GameManager.instance.currentPlayer;
            // get moves left
            var movesLeft = dice.GetMovesLeftList(currentPlayer.movesPlayed.Select(x => x.step));

            MoveActionTypes action = MoveActionTypes.Move;
            MoveError error = MoveError.Unknown;
            int stepPlayed = -1;

            foreach (var step in movesLeft)
            {
                stepPlayed = step;
                error = Rule.ValidateMove(this, collisionSlot, step, out action);

                if (error == MoveError.NoError)
                    break;
            }

            if (error == MoveError.NoError)
            {
                OnSuccessfulMove(action, stepPlayed);
            }
            else
            {
                OnFailedMove(error);
            }
        }
    }

    private void OnFailedMove(MoveError error)
    {
        ResetToOldPosition();
        Debug.LogError(error);
    }

    private void OnSuccessfulMove(MoveActionTypes action, int stepPlayed)
    {
        // TODO: make hit action respond
        var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

        Debug.LogWarning(action);

        // log played moves for undo
        movesPlayedList.Add(new Move { piece = this, from = currentSlot, to = collisionSlot, step = stepPlayed, action = action });

        //---------------------------------------
        // action events
        //---------------------------------------
        // move enemy to bar
        if ((action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyPiece = collisionSlot.GetComponent<Slot>().pieces.Last();
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(pieceType));

            enemyPiece.PlaceOn(enemyBar.GetComponent<Slot>(), 0);
        }

        // move yourself to outside
        if ((action & MoveActionTypes.Bear) == MoveActionTypes.Bear)
        {
            var slotOutside = Slot.GetOutside(pieceType);

            PlaceOn(slotOutside.GetComponent<Slot>(), 0);
        }
        // place on new slot
        else
            PlaceOn(collisionSlot, collisionSlot.pieces.Count);
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
