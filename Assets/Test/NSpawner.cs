using Mirror;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSpawner : NetworkBehaviour
{
    [SerializeField]
    private NetworkIdentity itemPrefab;

    public override void OnStartServer()
    {
        foreach (var index in Enumerable.Range(1, 10))
            SpawnItem(index);
    }

    private void SpawnItem(int index)
    {
        GameObject go = Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity);
        go.name = itemPrefab.name;

        var room = NetworkRoomManagerEx.RoomPlayers;
        var owner = (index & 1) == 0 ? room[0].connectionToClient : room[1].connectionToClient;

        NetworkServer.Spawn(go, owner);
    }
}
