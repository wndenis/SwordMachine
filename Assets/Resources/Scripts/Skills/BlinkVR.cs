using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Valve.VR;

public class BlinkVR : MonoBehaviour
{
    private SteamVR_Action_Boolean blinkAction = SteamVR_Actions.default_Dash;
    public Transform player;
    public Transform origin;
    [Space]
    public float travelDuration = 0.5f;
    public float range = 15;
    public AnimationCurve movementCurve;

    [Space] 
    public AnimationCurve FXCurve;
    public AudioClip audioClip;
    public GameObject pointer;
    public GameObject appearParts;

    private GameObject clonePointer;
    private RaycastHit lastRaycastHit;
    private Vector3 point;

    private float blinkCooldown = 0;
    private float blinkCooldownTotal = 1.5f;
    
    private PostProcessVolume postProcessing;


    private void Start()
    {
        postProcessing = GetComponent<PostProcessVolume>();
    }

    private void GetPoint()
    {
        if (Physics.Raycast(origin.position, origin.forward, out lastRaycastHit, range))
        {
            point = lastRaycastHit.point + lastRaycastHit.normal * 0.005f;
        }
        else
        {
            point = origin.position + origin.forward * range;
            //return null;
        }
    }

    private void TeleportToLookAt()
    {
        blinkCooldown = blinkCooldownTotal;
        Destroy(clonePointer);
        StartCoroutine(VisualEffects(player.position, point));
    }

    private void ShowPoint()
    {
        GetPoint();
        if (clonePointer == null)
        {
            clonePointer = Instantiate(pointer, point, pointer.transform.rotation) as GameObject;
        }
        clonePointer.transform.position = point;
    }

    void Update()
    {
        if (blinkCooldown <= 0)
        {
            if (blinkAction.state)
                ShowPoint();
            else if (blinkAction.stateUp)
                TeleportToLookAt();
        }
    }

    private void FixedUpdate()
    {
        if (blinkCooldown > 0)
            blinkCooldown -= Time.deltaTime;
    }

    private IEnumerator VisualEffects(Vector3 from, Vector3 to)
    {
        var t = 0f;
        if (audioClip != null)
            AudioSource.PlayClipAtPoint(audioClip, to, 1.1f);

        while (t <= travelDuration)
        {
            t += Time.deltaTime;
            var time = t / travelDuration;
            var sample = movementCurve.Evaluate(time);
            player.position = Vector3.Lerp(from, to, sample);
            postProcessing.weight = FXCurve.Evaluate(time);
            yield return null;
        }

        postProcessing.weight = 0f;
        Instantiate(appearParts, point, Quaternion.identity);
        // ppc.motionBlur.frameBlending = oldFrameBlending;
        // ppc.motionBlur.shutterAngle = oldShutterAngle;
        // ppc.vignette.intensity = oldVignetteIntensity;
        // ppc.vignette.smoothness = oldVignetteSmooth;
        //GetComponent<Collider>().enabled = true;
    }
}
