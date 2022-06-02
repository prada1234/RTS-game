using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]private List<Unit> myUnits = new List<Unit>();

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawn -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    #region client

    public override void OnStartClient()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSespawned += AurhorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDesawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopAuthority()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSespawned -= AurhorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDesawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Remove(unit);
    }

    private void AurhorityHandleUnitSpawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Add(unit);
    }

    #endregion
}
