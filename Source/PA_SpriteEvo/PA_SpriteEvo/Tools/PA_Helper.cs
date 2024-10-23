using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PA_SpriteEvo
{
    internal static class PA_Helper
    {
        internal static GameObject AddEmptyChild(this GameObject parent, string name)
        {
            GameObject instance = new GameObject(name);
            instance.transform.SetParent(parent.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            return instance;
        }
        internal static void SetParentSafely(this GameObject child, GameObject parent) 
        {
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
        }
    }
}
