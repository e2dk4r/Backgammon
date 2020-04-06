using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NItem : NetworkBehaviour
{
    [SyncVar]
    private bool isBeingHeld;

    private void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetButtonDown("Fire1") && IsMouseOverThis())
        {
            isBeingHeld = true;
        }

        if (Input.GetButtonUp("Fire1"))
            isBeingHeld = false;

        if (isBeingHeld)
        {
            var mousePos = GetMousePos();
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    private bool IsMouseOverThis()
    {
        var hit = Physics2D.Raycast(GetMousePos(), Vector2.zero);
        if (hit.collider != null)
        {
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

}
