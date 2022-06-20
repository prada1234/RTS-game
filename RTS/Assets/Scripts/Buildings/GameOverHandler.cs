using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
  
    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;
    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandlerBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandlerBaseDespawned;

    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandlerBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandlerBaseDespawned;
    }
    [Server]
    private void ServerHandlerBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandlerBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1) { return; }

        int playerId = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }
    #endregion
}
