using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TipPositionsBuffer : MonoBehaviour
{
    public Transform trackingTarget;
    public uint maxPoints = 65;
    public float bornTime { get; private set; }

    public float interval { get; private set; }

    // I do not need to set any other interval except fixedDeltaTime, but you can modify it for your purposes
//    public bool useFixedInterval = true;
//    public float intervalSeconds = 1f;
//    private float interval;
    private LinkedList<Vector3> lastKnownPositions;
    private LinkedList<float> lastKnownPositionsTime;
    
    // Start is called before the first frame update
    void Start()
    {
        interval = Time.fixedUnscaledDeltaTime / 1.5f;
        lastKnownPositions = new LinkedList<Vector3>();
        lastKnownPositionsTime = new LinkedList<float>();
        var zeroPos = trackingTarget.position;
        var zeroTime = Time.time;
        bornTime = zeroTime;
        for (var i = 0; i < maxPoints; i++)
        {
            lastKnownPositions.AddLast(zeroPos);
            lastKnownPositionsTime.AddLast(zeroTime);
        }
        StartCoroutine(GetPointWithInterval());
    }
    
    private void CollectPoint()
    {
        lastKnownPositions.AddLast(trackingTarget.position);
        lastKnownPositionsTime.AddLast(Time.time);
        lastKnownPositions.RemoveFirst();
        lastKnownPositionsTime.RemoveFirst();
    }

    // If count == 0, returns all points
    // else returns last "count" points
    // if length is less than count, returns all points

    public int GetElementsCountInTimeWindow(float timeWindow = 0f)
    {
        // var actualTimeWindow = Mathf.Abs(lastKnownPositionsTime.Last.Value - lastKnownPositionsTime.First.Value);
        if (timeWindow <= 0f) 
            timeWindow = 9999f;

        var lastTime = lastKnownPositionsTime.Last.Value;
        var count = 0;
        foreach (var elem in lastKnownPositionsTime)
        {
            if (lastTime - elem <= timeWindow)
                return lastKnownPositions.Count - count;
            count += 1;
        }
        return 0;
    }

    public List<Vector3> TakePositions(int count = 0)
    {
        return Take(ref lastKnownPositions, count);
    }

    public List<float> TakeTimes(int count = 0)
    {
        return Take(ref lastKnownPositionsTime, count);
    }

    private List<T> Take<T>(ref LinkedList<T> from, int count = 0)
    {
        if (count > from.Count || count == 0)
            count = from.Count;
        
        return from.Skip(from.Count - count).Take(count).ToList();;
    }
    
    
    // IEnumerator for custom intervals
    private IEnumerator GetPointWithInterval()
    {
        while (true)
        {
            CollectPoint();
            yield return new WaitForSecondsRealtime(interval);
        }
    }

}
