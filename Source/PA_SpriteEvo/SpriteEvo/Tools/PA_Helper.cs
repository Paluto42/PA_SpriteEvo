using UnityEngine;

namespace SpriteEvo
{
    internal static class PA_Helper
    {
        internal static void SetScaleX(this Spine41.Skeleton skeleton, float x)
        {
            if (x == 1f || x == -1f)
            skeleton.ScaleX = x;
        }
        internal static GameObject AddEmptyChild(this GameObject parent, string name)
        {
            GameObject instance = new GameObject(name);
            instance.transform.SetParent(parent.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            return instance;
        }
        internal static void SetParentSafely(this GameObject child, GameObject parent) 
        {
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
        }
        internal static void DestoryInNextFrame(this GameObject obj) 
        {
            if (obj != null) 
            {
                GameObject.Destroy(obj);
            }
        }

    }
}
