using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRoomManagerEx : NetworkRoomManager
{
    public static List<NetworkRoomPlayer> RoomPlayers
    {
        get
        {
            return (singleton as NetworkRoomManager).roomSlots;
        }
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        var networkPlayer = roomPlayer.GetComponent<NetworkRoomPlayer>();
        var player = gamePlayer.GetComponent<NPlayer>();

        // use unique id from server
        player.id = (int)networkPlayer.netId;
        // use index for determining piece type
        player.type = (networkPlayer.index & 1) == 0 ? NPlayerType.White : NPlayerType.Black;

        return true;
    }

    public override void OnRoomServerPlayersReady()
    {
        // switch to game scene
        base.OnRoomServerPlayersReady();
    }
}
