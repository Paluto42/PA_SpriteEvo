using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpriteEvo
{
    public static class ObjectUtility
    {
        public static GameObject AddEmptyChild(this GameObject parent, string name)
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

        public static void SetTransform(this GameObject obj, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            if (obj == null) return;
            //@object.transform.position = @params.position;
            obj.transform.localPosition = pos;
            obj.transform.rotation = Quaternion.Euler(rot);
            obj.transform.localScale = scale;
        }

        public static void AddScriptsFrom(this GameObject obj, List<ScriptProperties> props)
        {
            if (props == null) return;
            foreach (var cmp in props)
            {
                if (cmp?.scriptClass == null) continue;
                if (typeof(ScriptBase).IsAssignableFrom(cmp?.scriptClass))
                {
                    Component comp = obj.AddComponent(cmp.scriptClass);
                    if (comp is ScriptBase script)
                        script.props = cmp;
                }
            }
        }

        //通用，禁用渲染器反射(MeshRender)
        public static void DisableProbe(this GameObject obj)
        {
            MeshRenderer MeshRenderer = obj.GetComponent<MeshRenderer>();
            if (MeshRenderer == null) return;
            MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
            MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }
    }
}
