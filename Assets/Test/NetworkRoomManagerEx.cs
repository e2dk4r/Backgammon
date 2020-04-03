using Mirror;
using UnityEngine;

public class NetworkRoomManagerEx : NetworkRoomManager
{
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
