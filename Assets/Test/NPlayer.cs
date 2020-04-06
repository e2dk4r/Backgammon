using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public enum NPlayerType
{
    White,
    Black
}

public class NPlayer : NetworkBehaviour
{
    [SyncVar]
    public int id;
    [SyncVar]
    public NPlayerType type;
}
