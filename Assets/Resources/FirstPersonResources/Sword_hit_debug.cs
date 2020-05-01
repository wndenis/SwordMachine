using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_hit_debug : MonoBehaviour
{
    private Animation anim;
    private const int maxAttacks = 2;
    private int currentAttack = 0;

    private string[] names;
    // Start is called before the first frame update
    void Start()
    {
        names = new string[] {"SwordAttack_DEBUG", "SwordAttack2_DEBUG"};
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (anim.isPlaying)
                return;
            currentAttack += 1;
            currentAttack %= maxAttacks;
            anim.PlayQueued(names[currentAttack]);
        }
    }
}
