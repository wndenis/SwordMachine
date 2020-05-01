﻿using System;
using System.Collections;
using System.Collections.Generic;
 using System.Globalization;
 using RootMotion;
 using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class ScratchMaker : MonoBehaviour
{
    public float minPointDelay = 0.025f;
    public float minPointDist = 0.015f;
    public int maxPoints = 50;
    public int maxScratches = 20;
    public Material lineMaterial;
    public LayerMask LineLayers;
    public ParticleSystem ScratchEffect;
    public AudioClip[] ScratchSounds;
    public Transform[] RayCastDirections;
    [Space(3)] 
    public TipPositionsBuffer buffer;

    private AudioSource audioSource;
    private ParticleSystem.EmissionModule emission;
    
    private float lastPointTime = 0;
    private Transform ScratchesRoot;

    private LinkedList<LineRenderer> lineRenderers;
    private AnimationCurve curve;

    private bool tipReleased = true;
    
    // limits for speed measuring
    private float lastKnownSpeed;
    private float lastInvocationTime;
    private float invocationInterval;
    //======================================
    private DemoGUIMessage msg;
    void Start()
    {
        msg = GetComponent<DemoGUIMessage>();
        audioSource = GetComponent<AudioSource>();
        lineRenderers = new LinkedList<LineRenderer>();
        ScratchEffect.Stop();
//        curve = new AnimationCurve(new Keyframe(0, 0.002f), new Keyframe(0.3f, 0.2f), new Keyframe(1, 0.3f));
        curve = new AnimationCurve(new Keyframe(0, 0.35f), new Keyframe(0.3f, 0.5f), new Keyframe(1, 0.6f));
        ScratchesRoot = new GameObject().transform;
        ScratchesRoot.parent = null;
        invocationInterval = Time.fixedDeltaTime * 1.1f;
        lastPointTime = Time.time - minPointDelay;
//        StartCoroutine(FixScratches());
    }

//    private IEnumerator FixScratches()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(0.5f);
//            if (!((transform.localPosition - defaultPosition).magnitude > 0.065f)) continue;
//            var rb = GetComponent<Rigidbody>();
//            rb.velocity = Vector3.zero;
//            rb.angularVelocity = Vector3.zero;
//            transform.localPosition = defaultPosition;
//            rb.velocity = Vector3.zero;
//            rb.angularVelocity = Vector3.zero;
//            print("Жопа спасена");
//        }
//    }

    private void OnCollisionEnter(Collision other)
    {
        if (SkipPoint(other))
            return;
        tipReleased = false;
        AddNewLineRenderer(other).SetPosition(0, ExtractPoint(other));
        lastPointTime = Time.time;
    }

    private void OnCollisionStay(Collision other)
    {
        if (SkipPoint(other))
            return;
        if (lineRenderers.Count == 0 || tipReleased){ //TODO: seems to be a hotfix
            OnCollisionEnter(other);
            return;
        }
        var lineRenderer = lineRenderers.Last.Value;
        var point = ExtractPoint(other);
        if (Vector3.Distance(point, lineRenderer.GetPosition(lineRenderer.positionCount - 1)) > minPointDist)
        {
            var positionCount = lineRenderer.positionCount;
            if (positionCount == 2)
                PlayScratchFX();
            if (positionCount > maxPoints)
            {
                AddNewLineRenderer(other).SetPosition(0, point);
                return;
            }

            positionCount += 1;
            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPosition(positionCount - 1, point);
            lastPointTime = Time.time;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (InteractableLayer(other.gameObject.layer))
            StopScratchFX();
        tipReleased = true;
    }

    private void PlayScratchFX()
    {
        ScratchEffect.Play(true);
        if (!audioSource.isPlaying)
            audioSource.pitch = Random.Range(0.85f, 1.0f);
        if (ScratchSounds.Length > 0)
            audioSource.PlayOneShot(ScratchSounds[Random.Range(0, ScratchSounds.Length)]);
    }

    private void StopScratchFX()
    {
        ScratchEffect.Stop(true);
    }
    
    private LineRenderer AddNewLineRenderer(Collision other)
    {
        if (lineRenderers.Count >= maxScratches)
        {
            var tmp = lineRenderers.First.Value;
            tmp.positionCount = 1;
            var tmpTransform = tmp.transform;
            tmpTransform.rotation = Quaternion.Euler(0.001f, 0.001f, 0.001f);
            tmpTransform.forward = -ExtractNormal(other);
            //Destroy(ScratchesRoot.GetChild(0).gameObject);
            lineRenderers.RemoveFirst();
            lineRenderers.AddLast(tmp);
            return lineRenderers.Last.Value;

        }

        var child = new GameObject();
        child.transform.parent = ScratchesRoot;
        child.transform.rotation = Quaternion.Euler(0.001f, 0.001f, 0.001f);
        child.transform.forward = -ExtractNormal(other);

        var lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.widthCurve = curve;
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        lineRenderer.widthMultiplier = 0.7f;
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        lineRenderer.alignment = LineAlignment.TransformZ;
//        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.material = lineMaterial;
//        lineRenderer.startColor = new Color(0.14f, 0.14f, 0.14f);
//        lineRenderer.endColor = new Color(0.14f, 0.14f, 0.14f);
        lineRenderer.generateLightingData = true;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.lightProbeUsage = LightProbeUsage.Off;
        lineRenderer.reflectionProbeUsage = ReflectionProbeUsage.Simple;
        lineRenderer.numCornerVertices = 3;
        lineRenderer.textureMode = LineTextureMode.DistributePerSegment;


        lineRenderer.positionCount = 1;
        lineRenderers.AddLast(lineRenderer);
        return lineRenderer;
    }

    #region Utils
    private float CalculateMedianSpeed()
    {
        if (Time.time - lastInvocationTime < invocationInterval)
        {
//            print($"Last Known Speed: {lastKnownSpeed}");
            return lastKnownSpeed;
        }
            
        lastInvocationTime = Time.time;
        var speeds = new List<float>();
        var timeOffset = buffer.GetElementsCountInTimeWindow(0.215f);
        var bPoints = buffer.TakePositions(timeOffset);
        var bTimes = buffer.TakeTimes(timeOffset);
        if (bPoints.Count < 3)
        {
            lastInvocationTime = Time.time;
            lastKnownSpeed = 0f;
//            print($"Less than 3, speed: {lastKnownSpeed}");
            return lastKnownSpeed;
        }
            
        var count = bPoints.Count;

        for (var i = 0; i < bPoints.Count - 1; i++)
        {
            var dt = bTimes[i + 1] - bTimes[i];
            speeds.Add(Vector3.Distance(bPoints[i + 1], bPoints[i]) / dt);
        }
            
        
        speeds.Sort();
        var speed = 0f;
        if (count % 2 == 0)
            speed = (speeds[count / 2 - 1] + speeds[count / 2]) / 2;
        else
            speed = speeds[count / 2];
        lastKnownSpeed = speed;
//        print($"Speed: {lastKnownSpeed}");
        return speed;
    }
    private Vector3 ExtractPoint(Collision other)
    {
        var contact = other.GetContact(0);
        return contact.point + contact.normal * 0.0001f;
    }
    private Vector3 ExtractNormal(Collision other)
    {
        return other.GetContact(0).normal.normalized;
    }
    private bool InTiming()
    {
        return Time.time - lastPointTime > minPointDelay;
    }
    private bool InteractableLayer(int layer)
    {
        return LineLayers == (LineLayers | (1 << layer));
    }

    private bool SkipPoint(Collision other)
    {
        var medianSpeed = CalculateMedianSpeed();
        msg.text = medianSpeed.ToString(CultureInfo.InvariantCulture);
        var timing = InTiming();
        var conditions = InteractableLayer(other.gameObject.layer) && medianSpeed > 7.3f;
        var zeroScratches = lineRenderers.Count == 0;
        var t1 = zeroScratches && conditions;
        var t2 = conditions && timing;
        return !(t1 || t2);
    }
    #endregion
    
    //todo raycast вместо коллизии
}
