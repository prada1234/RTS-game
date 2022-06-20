using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField]private List<Unit> myUnits = new List<Unit>();
    [SerializeField] private List<Building> myBuildings = new List<Building>();


    #region server
    public List<Unit> GetMyUnit()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawned;
    }
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawn -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingID,Vector3 point)
    {
        Building buildingToPlace = null;

        foreach(Building building in buildings)
        {
            if (building.GetID() == buildingID)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) { return; }

       GameObject buildingInstance =  Instantiate(buildingToPlace.gameObject, point,buildingToPlace.transform.rotation);

       NetworkServer.Spawn(buildingInstance, connectionToClient);
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

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
    }

    #endregion

    #region client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSespawned += AurhorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDesawned += AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;

    }

    public override void OnStopAuthority()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.AuthorityOnUnitSespawned -= AurhorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDesawned -= AuthorityHandleUnitDespawned;


        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;

    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AurhorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleBuildingDespawned(Building obj)
    {
        myBuildings.Add(obj);
    }

    private void AuthorityHandleBuildingSpawned(Building obj)
    {
        myBuildings.Remove(obj);
    }

    #endregion
}
