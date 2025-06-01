using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static GameConstants;

public class PathManager : Singleton<PathManager>
{
    public List<WayPointlist> Listwaypoint = new List<WayPointlist>();

    public WayPoint GetWayPoint(PoolType poolType)
    {
        var wayPointList = Listwaypoint.Find(list => list.botType == (BotType) poolType);
        if (wayPointList != null)
        {
            var paths = wayPointList._wayPointlist;
            if (paths.Count == 0)
                throw new Exception("No available paths for bot type: " + poolType);
            
            int index = wayPointList.GetIndexPath();
            return paths[index];
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
    public GameObject SamplePrefab;
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
}