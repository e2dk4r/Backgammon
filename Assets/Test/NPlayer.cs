using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum NPlayerType
{
    White,
    Black
}
public class NPlayer : NetworkBehaviour
{
    public int id;
    public NPlayerType type;

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSayYourName();
    }

    [Command]
    private void CmdSayYourName()
    {
        Debug.Log($"I am { id } and { type }");
    }
}
