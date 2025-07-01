using System;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using Random = UnityEngine.Random;


public class PathManager : Singleton<PathManager>
{
    public List<WayPointlist> Listwaypoint = new List<WayPointlist>();

    public WayPoint GetWayPoint(PoinSpawnbot point)
    {
        WayPointlist wayPointList = null;
        if (point == PoinSpawnbot.All)
            wayPointList = Listwaypoint[Random.Range(0, Listwaypoint.Count)];
        else
            wayPointList = Listwaypoint.Find(list => list.point == point);

        if (wayPointList != null)
        {
            var paths = wayPointList._wayPointlist;
            var availablePaths = paths.FindAll(x => x.isUse == false);
            if (availablePaths.Count <= 0)
            {
                // throw new Exception("No available paths for bot type: " + poolType);
                return paths[wayPointList.GetIndexRandomPath()];
            }
            else
            {
                int index = UnityEngine.Random.Range(0, availablePaths.Count);
                availablePaths[index].isUse = true;
                return availablePaths[index];
            }
        }
        Debug.LogError("Null WayPointList");
        return null;
    }
}

[Serializable]
public class WayPointlist
{
    public PoinSpawnbot point;
    public List<WayPoint> _wayPointlist = new List<WayPoint>();
    public int GetIndexRandomPath()
    {
        return Random.Range(0, _wayPointlist.Count);
    }
}

[Serializable]
public class listLimitWayPoint
{
    public List<LimitWayPoint> LimitWayPoint;
}

[Serializable]
public class LimitWayPoint
{
    public List<Transform> Limited;
}

[Serializable]
public class WayPoint
{
    public bool isUse;
    public List<Transform> WayPoints = new List<Transform>();
    public List<Transform> AttackWayPoints = new List<Transform>();
    
    public Transform GetRandomWayPoint()
    {
        if (WayPoints.Count == 0)
            throw new Exception("No waypoints available.");
        
        return WayPoints[UnityEngine.Random.Range(0, WayPoints.Count)];
    }
    
    public Transform GetRandomAttackWayPoints()
    {
        if (AttackWayPoints.Count == 0)
            throw new Exception("No AttackWayPoints available.");
        
        return AttackWayPoints[UnityEngine.Random.Range(0, AttackWayPoints.Count)];
    }
}