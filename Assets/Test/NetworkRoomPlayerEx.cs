using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkRoomPlayerEx : NetworkRoomPlayer
{
    public override void OnStartClient()
    {
        // start directly
        CmdChangeReadyState(true);
    }
}
