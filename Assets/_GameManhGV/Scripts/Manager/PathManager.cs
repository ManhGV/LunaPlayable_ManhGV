using System;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class PathManager : Singleton<PathManager>
{
    public List<WayPointlist> Listwaypoint = new List<WayPointlist>();

    public WayPoint GetWayPoint(BotType poolType)
    {
        var wayPointList = Listwaypoint.Find(list => list.botType == poolType);
        if (wayPointList != null)
        {
            var paths = wayPointList._wayPointlist;
            if (paths.Count == 0)
                throw new Exception("No available paths for bot type: " + poolType);
            
            // int index = wayPointList.GetIndexPath();
            return paths[UnityEngine.Random.Range(0, paths.Count)];
        }

        throw new Exception("No paths found for bot type: " + poolType);
    }
}

[Serializable]
public class WayPointlist
{
    public BotType botType;
    private int indexPath;
    public List<WayPoint> _wayPointlist = new List<WayPoint>();
    public int GetIndexPath()
    {
        indexPath++;
        
        if (indexPath >= _wayPointlist.Count)
            indexPath = 0;
        
        return indexPath;
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