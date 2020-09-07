using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableWeapon : CollectableItem
{
    [SerializeField] [Range(0, 100)]        protected int   _rounds = 15;

    public int  Rounds    { get => _rounds;
        set => _rounds = value;
    }
}
