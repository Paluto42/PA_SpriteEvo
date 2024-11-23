using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        //May be Null
        public static TextAsset AppendToAtlasText(this TextAsset child, TextAsset parent)
        {
            if (parent == null || child == null) return null;
            string origincontent = parent.text;
            string toappend = child.text;
            string result = origincontent + "\n\n" + toappend;
            TextAsset newatlas = new TextAsset(result)
            {
                name = parent.name
            };
            return newatlas;
        }
        //May be Null
        public static Texture2D[] AppendToTextureArray(this Texture2D[] child, Texture2D[] parent)
        {
            if (parent == null || child == null) return null;
            Texture2D[] mergedTexArray = new Texture2D[parent.Length + child.Length];
            for (int i = 0; i < parent.Length; i++)
            {
                mergedTexArray[i] = parent[i];
            }
            for (int i = 0; i < child.Length; i++)
            {
                mergedTexArray[parent.Length + i] = child[i];
            }
            return mergedTexArray;
        }
    }
}
