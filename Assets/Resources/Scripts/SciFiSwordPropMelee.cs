using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class SciFiSwordPropMelee : RootMotion.Demos.PropMelee
{
    [Space(5)]
    public SciFiSwordAnimation anim;
    public Sword sword;
    protected override void OnPickUp(PropRoot propRoot)
    {
        anim.Open();
        base.OnPickUp(propRoot);
    }

    protected override void OnDrop()
    {
        anim.Close();
        base.OnDrop();
    }
}
