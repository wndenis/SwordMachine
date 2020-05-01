using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public Transform root;

    public Transform roroot;

    public Transform rend;

    public float Force = 20f;

    public GameObject Explosion;

    private List<Rigidbody> rbs;

    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        rbs = new List<Rigidbody>();
        Invoke("Kill", 5f);
        Invoke("Dismember", 6f);
        Invoke("Boom", 7f);
    }

    void Kill()
    {
        var puppet = GetComponentInChildren<PuppetMaster>();
        puppet.pinPow = 0;
        puppet.mappingWeight = 10f;
        rend.parent = null;
    }

    void Dismember()
    {
        var joints = GetComponentsInChildren<Joint>();
        var parts = root.GetComponentsInChildren<Transform>();

        foreach (var j in joints)
        {
            j.connectedBody = null;
            j.transform.parent = null;
            Destroy(j.gameObject);
        }
        pos = root.transform.position;
        root.parent = null;
        root.gameObject.AddComponent<BoxCollider>().size = Vector3.one * 0.3f;
        root.gameObject.AddComponent<Rigidbody>().AddExplosionForce(100, pos, 10);
        

        foreach (var part in parts)
        {
            part.parent = null;
            var rb = part.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = part.gameObject.AddComponent<Rigidbody>();
            rbs.Add(rb);
            part.gameObject.AddComponent<BoxCollider>().size = Vector3.one * 0.3f;
        }
    }

    void Boom()
    {
        Destroy(Instantiate(Explosion, root.position, root.rotation), 2);
        foreach (var rb in rbs)
        {
            print("OBJ");
            rb.AddExplosionForce(Force, pos, 10, 3, ForceMode.Impulse);
        }
        Destroy(roroot.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
