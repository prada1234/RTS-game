using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawenrPrefab = null;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

       GameObject unitySpawnerInstance = Instantiate(unitSpawenrPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitySpawnerInstance, conn);
    }
}
