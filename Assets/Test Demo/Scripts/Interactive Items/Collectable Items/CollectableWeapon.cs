using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableWeapon : CollectableItem
{
    [SerializeField] [Range(0, 100)]        protected int   _rounds = 15;

    public int  Rounds    { get => _rounds;
        set => _rounds = value;
    }

    public override Vector3 GetAttachPosition()
    {
        return new Vector3(0, 0, 0.0575f);
    }

    public override Quaternion GetAttachRotation()
    {
        return Quaternion.Euler(19, 90, 180);
    }
}
