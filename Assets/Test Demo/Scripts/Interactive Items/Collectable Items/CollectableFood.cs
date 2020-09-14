using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CollectableFood : CollectableItem
{
    public override Vector3 GetAttachPosition()
    {
        return new Vector3(0.0317f, -0.0298f, 0.0106f);
    }

    public override Quaternion GetAttachRotation()
    {
        return Quaternion.Euler(-73.414f, 90f, 90f);
    }
}
