using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHelgthUpdated))]

    private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int,int> ClientOnHealthUpDated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

  

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0) { return; }

        ServerOnDie?.Invoke();

    }

    private void ServerHandlePlayerDie(int obj)
    {
        if (connectionToClient.connectionId != obj) { return; }

        DealDamage(currentHealth);
    }

    #endregion

    #region Client

    private void HandleHelgthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpDated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
