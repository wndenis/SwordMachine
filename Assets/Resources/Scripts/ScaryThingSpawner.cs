using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;

public class ScaryThingSpawner : MonoBehaviour
{
    public Camera targetCamera;
    public Transform forward;

    public GameObject[] poses;
    private Renderer[] renderers;
    
    private int currentPoseInd;
    // Start is called before the first frame update
    void Start()
    {
        renderers = new Renderer[poses.Length];
        for (var i = 0; i < poses.Length; i++)
            renderers[i] = poses[i].GetComponentInChildren<SkinnedMeshRenderer>();
        StartCoroutine(Cycle());
    }

    private Vector3 ConditionsSpawn()
    {
        var res = Vector3.zero;
        if (!Physics.Raycast(targetCamera.transform.position-forward.forward * 0.5f, -forward.forward, 3f))
            if (Physics.Raycast(targetCamera.transform.position - forward.forward * 0.9f, Vector3.down, out var raycastHit,
                3f))
                res = raycastHit.point;
        return res;
    }

    private void SetPosesActive(bool val)
    {
        foreach (var elem in poses)
        {
            elem.SetActive(val);
        }
    }

    private IEnumerator Cycle()
    {
        var canSpawn = false;
        var spawnPoint = Vector3.zero;
        SetPosesActive(false);
        while (true)
        {
            while (!canSpawn)
            {
                yield return new WaitForSeconds(0.1f);
                spawnPoint = ConditionsSpawn();
                if (spawnPoint != Vector3.zero)
                    canSpawn = true;
            }
            canSpawn = false;
            
            transform.position = spawnPoint;
            transform.LookAt(targetCamera.transform);
            var rot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
            currentPoseInd = Random.Range(0, poses.Length);
            
            poses[currentPoseInd].SetActive(true);
            yield return null;
            var initialVisible = renderers[currentPoseInd].isVisible;

            var visibleDuration = Random.Range(2f, 5f);
            var invisibleDuration = Random.Range(2f, 15f);

            if (initialVisible)
            {
                poses[currentPoseInd].SetActive(false);
                visibleDuration = 0f;
                invisibleDuration = 1.25f;
            }
                

            if (Random.Range(0f, 1f) < 0.25f)
                visibleDuration /= 8f;
            
            var t = 0f;
            while (t < visibleDuration)
            {
                if (!renderers[currentPoseInd].isVisible && 
                    Vector3.Distance(transform.position, targetCamera.transform.position) > 5f)
                    break;
                yield return new WaitForSecondsRealtime(0.1f);
                t += 0.1f;
            }
            
            while (renderers[currentPoseInd].isVisible)
                yield return null;
            SetPosesActive(false);
            
            if (Random.Range(0f, 1f) < 0.2f)
                invisibleDuration /= 5f;
            yield return new WaitForSeconds(invisibleDuration);
        }
    }
}
