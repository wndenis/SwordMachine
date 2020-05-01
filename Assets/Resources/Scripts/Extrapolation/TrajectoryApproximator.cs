using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TrajectoryApproximator : MonoBehaviour
{
//    public Weapon weapon;
//    public bool parabolicWeapon;
//    private Transform observableTarget;
//    private LineRenderer lineRenderer;
//
//    private float t_delay = 0.005f;
//    private float t_measure = 0.5f;
//    private int n = 3;
//    private float dt;
//
//    private float bulletSpeed = 30f;
//    private bool extrapolating;
    private LineRenderer trailLine;
    private LineRenderer approxLine;

    public TipPositionsBuffer buffer;
    public Material lineMaterial;

    private int extrapolatedPointsCount = 6;
    private float extrapolatedTimeForward = .5f;
    [Range(1, 30)]
    public int smoothingWindow = 4; //10
    
    
    private float dt; // шаг времени для экстраполяции, вычислим сами

    private LineRenderer debug;
    
    private void Start()
    {

        trailLine = new GameObject().AddComponent<LineRenderer>();
        approxLine = new GameObject().AddComponent<LineRenderer>();
        
        debug = new GameObject().AddComponent<LineRenderer>();
        debug.transform.parent = transform;
        debug.material = lineMaterial;
        debug.widthMultiplier = 0.06f;
        debug.startColor = Color.black;
        debug.endColor = Color.black;
        debug.widthCurve = new AnimationCurve(new[] {new Keyframe(0, 0.2f), new Keyframe(1, 1f)});

        trailLine.transform.parent = transform;
        approxLine.transform.parent = transform;
        
        trailLine.material = lineMaterial;
        approxLine.material = lineMaterial;
        trailLine.widthMultiplier = 0.025f;
        var curve1 = new AnimationCurve(new[] {new Keyframe(0, 0.2f), new Keyframe(1, 1f)});
        trailLine.widthCurve = curve1;
            
        var g = new Gradient
        {
            alphaKeys = new[]
            {
                new GradientAlphaKey(1f, 1f), new GradientAlphaKey(1f, .3f), new GradientAlphaKey(0f, 0f)
            }
        };
        trailLine.colorGradient = g;
        
        
        approxLine.widthMultiplier = 0.06f;
        var curve2 = new AnimationCurve(new[] {new Keyframe(0, 1f), new Keyframe(1, 0.2f)});
        approxLine.widthCurve = curve2;
        
        approxLine.startColor = new Color(0f, 0.23f, 1f);
        approxLine.endColor = new Color(1f, 0.25f, 0.24f);
        approxLine.positionCount = 0;

        debug.lightProbeUsage = LightProbeUsage.BlendProbes;
        approxLine.lightProbeUsage = LightProbeUsage.BlendProbes;
        trailLine.lightProbeUsage = LightProbeUsage.BlendProbes;
        
        dt = extrapolatedTimeForward / extrapolatedPointsCount;
    }

    private void FixedUpdate()
    {
        // var timeOffset = buffer.GetElementsCountInTimeWindow(0.4864736f);
        var timeOffset = buffer.GetElementsCountInTimeWindow(0f);
        var times = buffer.TakeTimes(timeOffset);
        if (times.Count < smoothingWindow + 7)
            return;
        var positions = buffer.TakePositions(timeOffset);
        var lastPosition = positions.Last();
        trailLine.positionCount = positions.Count;
        trailLine.SetPositions(positions.ToArray());
        
        Smooth(ref positions, ref times);
        Normalize(ref times);
        Sample(ref positions, ref times, 3);
        
        debug.positionCount = positions.Count;
        debug.SetPositions(positions.ToArray());

        approxLine.positionCount = 1;
        approxLine.SetPosition(0, lastPosition);
        var extrapolation = new Extrapolations.VectorExtrapolation(times, positions);
        for (var x = 0; x < extrapolatedPointsCount; x++)
        {
            var tt = 1f + dt * (x + 1);
            var pos = extrapolation.Extrapolate(tt);
            if (pos.magnitude < 500)
            {
                approxLine.positionCount = x + 2;
                approxLine.SetPosition(x+1, pos);
            }
        }
    }

    public Vector3 PredictIntersection(float timeForward, Vector3 startPoint, float reachingAcceleration)
    {
        //todo: bin search until step < 0.05f
        // It using a sort of sphere marching to approximate closest interception point
        // of defined point (startPoint) which can move in any direction with defined speed
        // and another point moving along predicted trajectory
        
        // var timeOffset = buffer.GetElementsCountInTimeWindow(0.4864736f); // magic const to strip old points
        var timeOffset = buffer.GetElementsCountInTimeWindow(0f);
        var times = buffer.TakeTimes(timeOffset); // pick all time entries within last 0.48... sec
        var positions = buffer.TakePositions(timeOffset); // pick all pos entries within last 0.48... sec
        if (times.Count < smoothingWindow + 7)// if not enough points for extrapolation
        {
            print("no delta");
            print($"times: {times.Count}");
            print($"time offset: {timeOffset}");
            return positions[0]; // return last known position from buffer
        } 
            

        Smooth(ref positions, ref times); // smooth buffer trajectory using smoothing window
        Normalize(ref times);  // normalize times so times[0] became 0f and times[n] became 1f
        Sample(ref positions, ref times, 3); // sample last, mid and first point to feed them into extrapolation
        
        var extrapolation = new Extrapolations.VectorExtrapolation(times, positions); // create extrapolation
        var predictedPoints = new List<Vector3>(); // store predicted points here
        var predictedPointsCount = 4; // how many points do we want
        var timeStep = timeForward / predictedPointsCount; // time interval between two points
        
        // Extrapolate
        for (var pointInd = 0; pointInd < predictedPointsCount; pointInd++)
        {
            var tt = 1f + timeStep * (pointInd + 1);
            var pos = extrapolation.Extrapolate(tt);
            if (pos.magnitude > 600 || float.IsNaN(pos.x + pos.y + pos.z)) // handle outliers
            {
                return positions[0]; // return last known position from buffer
            }
            predictedPoints.Add(pos);
        }
        
        // our points are evenly distributed in time, so we can linearly interpolate between them
        // we have 4 points, 3 sections
        var maxDelta = 0.04f;
        var delta = float.PositiveInfinity;
        var left = 0f;
        var right = 1f;
        var normalizedTime = 1f;

        // if we are unlucky and cannot reach good delta, just limit iterations count
        var maxIterations = 20;
        var iterations = 0;
        
        var interpolation = new EvenSectionsInterpolation(predictedPoints);
        while (Mathf.Abs(delta) > maxDelta && iterations < maxIterations)
        {
            normalizedTime = (right + left) / 2f;
            
            var targetPos = interpolation.LerpPos(normalizedTime);
            
            var scaledTime = normalizedTime * timeForward;
            var ourMaxLen = scaledTime * reachingAcceleration * scaledTime;
            var ourLen = (targetPos - startPoint).magnitude;
            delta = ourMaxLen - ourLen;
            if (delta > 0f)
            {
                right = (right + left) / 2f;
            }
            else if (delta < 0f)
            {
                left = (right + left) / 2f;
            }
            iterations++;
        }
        if (iterations == maxIterations)
            print($"bad delta {delta}");
        else
            print($"good delta {delta}");

        return interpolation.LerpPos(normalizedTime);
    }

    private class EvenSectionsInterpolation
    {
        private readonly List<Vector3> points;
        private readonly float timeInterval;

        public EvenSectionsInterpolation(List<Vector3> p)
        {
            points = p;
            if (points.Count == 1)
            {
                throw new ArgumentException("Even sections interpolation requires at least 2 points");
            }
            timeInterval = 1f / (points.Count - 1);
        }
        
        public Vector3 LerpPos(float t)
        {
            var segmentNum = (int)(t / timeInterval);
            var localT = t % timeInterval;
            var left = points[segmentNum];
            var right = points[segmentNum + 1];
            return Vector3.Lerp(left, right, localT);
        }
        
        public float LerpLen(float t)
        {
            var segmentNum = (int)(t / timeInterval);
            var localT = t % timeInterval;
            var left = points[segmentNum];
            var right = points[segmentNum + 1];
            var pointOnSegment = Vector3.Lerp(left, right, localT);
            var cumulativeLength = (pointOnSegment - left).magnitude;
            for (var i = 0; i < segmentNum + 1; i++)
            {
                left = points[segmentNum];
                right = points[segmentNum + 1];
                cumulativeLength += (right - left).magnitude;
            }

            return cumulativeLength;
        }
    }



    private void Sample(ref List<Vector3> positions, ref List<float> times, int numSamples)
    {
        if (numSamples > positions.Count)
            return;
        var count = positions.Count;
        var timeWindow = 0.35f; // seconds
        for (var i = times.Count - 1; i >= 0; i--)
        {
            if (times[i] > 1 - timeWindow) continue;
            count = i;
            break;
        }
        
        var k = positions.Count - count;
        var step = k / numSamples;
        for (var i = 0; i < numSamples - 1; i++)
        {
            var elem = i * step + count;
            positions[i] = positions[elem];
            times[i] = times[elem];
        }

        positions[numSamples - 1] = positions.Last();
        times[numSamples - 1] = times.Last();
        positions = positions.GetRange(0, numSamples);
        times = times.GetRange(0, numSamples);
    }

    private void Normalize(ref List<float> times)
    {
        var firstTime = times[0];
        for (var i = 0; i < times.Count; i++)
        {
            times[i] -= firstTime;
            if (times[i] < 0)
                Debug.LogError("АХТУНГ ТУТ ВРЕМЯ ОТРИЦАТЕЛЬНОЕ");
        }
        var lastTime = times.Last();
        for (var i = 0; i < times.Count; i++)
        {
            times[i] /= lastTime;
        }
    }

    private void Smooth(ref List<Vector3> points, ref List<float> times)
    {
        if (points.Count != times.Count)
            throw new ArgumentException("Lists must be of same size");
        
        if (smoothingWindow > points.Count || smoothingWindow > times.Count || smoothingWindow == 0)
            return;
        points.Reverse(); //Im too lazy to reimplement reversed smoothing window
        times.Reverse();
        var firstPoint = points[0];
        var secondPoint = points[1];
        var firstTime = times[0];
        var secondTime = times[1];
        for (var step = 0; step <= points.Count - smoothingWindow; step++)
        {
            var sumVector = Vector3.zero;
            var sumTime = 0f;
            for (var i = 0; i < smoothingWindow; i++)
            {
                sumTime += times[step + i];
                sumVector += points[step + i];
            }
            times[step] = sumTime / smoothingWindow;
            points[step] = sumVector / smoothingWindow;
        }
        points.RemoveRange(points.Count - smoothingWindow, smoothingWindow);
        times.RemoveRange(times.Count - smoothingWindow, smoothingWindow);
        points.Reverse();
        times.Reverse();
        points.Add(secondPoint);
        times.Add(secondTime);
        points.Add(firstPoint);
        times.Add(firstTime);
    }

    public float CoarseSpeed()
    {
        var pos = buffer.TakePositions(2);
        if (pos.Count != 2)
            return 0f;

        return Mathf.Abs((pos[0] - pos[1]).magnitude) / buffer.interval;
    }
    
//
//    private IEnumerator Extrapolate()
//    {
//        var tolerance = 0.0005f;
//        var solved = false;
//        var pointReducer = 0;
//        lineRenderer.positionCount = 0;
//        for (float tApprox = 0; tApprox < 1; tApprox += 0.0005f)
//        {
//            var newPos = extrapolation.Extrapolate(tApprox + t);
//            if (!solved)
//            {
//                weapon.transform.rotation = Quaternion.LookRotation(newPos - weapon.tip.position);
//                var dist = Vector3.Distance(newPos, weapon.tip.position);
//                var timeToReach = dist / bulletSpeed;
//                if (Math.Abs(timeToReach - tApprox) < tolerance)
//                {
//                    if (parabolicWeapon)
//                        newPos -= (timeToReach * timeToReach) * Physics.gravity / 2;
//                    weapon.transform.rotation = Quaternion.LookRotation(newPos - weapon.tip.position);
//                    observableTarget = null;
//                    //yield return null;
//                    var b = weapon.Shoot();
//                    var lr = b.GetComponent<LineRenderer>();
//                    lr.positionCount = 2;
//                    lr.SetPosition(0, b.position);
//                    lr.SetPosition(1, newPos);
//                    solved = true;
//                }
//            }
//
//            pointReducer++;
//            if (pointReducer != 20) continue;
//            pointReducer = 0;
//            lineRenderer.positionCount++;
//            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
//        }
//        
//        
//        yield return new WaitForSeconds(0.75f);
//        lineRenderer.positionCount = 0;
//        extrapolating = false;
//    }
//    
}
