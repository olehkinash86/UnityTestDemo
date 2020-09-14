using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CollectableAmmo : CollectableItem
{
    public override Vector3 GetAttachPosition()
    {
        return new Vector3(-0.011f, -0.0328f, 0.0233f);
    }

    public override Quaternion GetAttachRotation()
    {
        return Quaternion.Euler(-1.325f, 95.443f, 56.133f);
    }

}