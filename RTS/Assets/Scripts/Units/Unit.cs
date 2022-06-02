using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawn;
    public static event Action<Unit> ServerOnUnitDespawn;

    public static event Action<Unit> AuthorityOnUnitSespawned;
    public static event Action<Unit> AuthorityOnUnitDesawned;



    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawn?.Invoke(this);
    }
    #endregion

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    #region client
    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    public override void OnStartClient()
    {
        if(!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitSespawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if(!isClientOnly || !hasAuthority) { return; }
        AuthorityOnUnitDesawned?.Invoke(this);
    }

    #endregion
}
