using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SlowmoSwordCam : MonoBehaviour
{
    public PostProcessVolume processVolume;
    public Transform target;
    
    
    [Space]
    public float defaultFov;
    public float minimalFov;
    
    [Space]
    public float defaultTimescale = 1f;
    public float minimalTimescale = 0.25f;
    
    
    private Camera cam;
    [Space]
    public float distance = 2.0f;
    public float xSpeed = 20.0f;
    public float ySpeed = 20.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float distanceMin = 10f;
    public float distanceMax = 10f;
    public float smoothTime = 2f;


    private bool slowmo;

    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;
    
    private bool m_inputCaptured;
    
    void OnValidate() {
        if(Application.isPlaying)
            enabled = true;
    }

    void CaptureInput() {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
        m_inputCaptured = true;
    }

    void ReleaseInput() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_inputCaptured = false;
    }

    void OnApplicationFocus(bool focus) {
        if(m_inputCaptured && !focus)
            ReleaseInput();
    }
    

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        cam = GetComponent<Camera>();
        defaultFov = cam.fieldOfView;
    }

    void LateUpdate()
    {
        if(!m_inputCaptured) {
            if(Input.GetMouseButtonDown(0)) 
                CaptureInput();
            else if(Input.GetMouseButtonDown(1))
                CaptureInput();
        }

        if(!m_inputCaptured || !target)
            return;
        
        if(m_inputCaptured)
            if(Input.GetKeyDown(KeyCode.Escape))
                ReleaseInput();

        velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
        velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;

        rotationYAxis += velocityX;
        rotationXAxis -= velocityY;
        rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        Quaternion fromRotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        Quaternion rotation = toRotation;

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        // RaycastHit hit;
//            if (Physics.Linecast(target.position, transform.position, out hit))
//            {
//            distance -= hit.distance;
//            }
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        
        transform.position = Vector3.Lerp(transform.position, position, 0.2f);
        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);


        if (Input.GetMouseButtonDown(0))
        {
            slowmo = !slowmo;
            StopAllCoroutines();
            if (slowmo)
            {
                StartCoroutine(ChangeTimeScale(minimalTimescale, minimalFov, 1f));
            }
            else
            {
                StartCoroutine(ChangeTimeScale(defaultTimescale, defaultFov, 0f));
            }
        }
    }

    private IEnumerator ChangeTimeScale(float timeScale, float fieldOfView, float effects)
    {
        var duration = 0.65f;
        for (var t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fieldOfView, t / duration);
            Time.timeScale = Mathf.Lerp(Time.timeScale, timeScale, t / duration);
            processVolume.weight = Mathf.Lerp(processVolume.weight, effects, t / duration);
            yield return null;
        }
        cam.fieldOfView = fieldOfView;
        Time.timeScale = timeScale;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

