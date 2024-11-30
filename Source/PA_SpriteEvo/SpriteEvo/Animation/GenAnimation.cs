using SpriteEvo.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace SpriteEvo
{
    public static class GenAnimation
    {
        public static bool currentlyGenerating = false;
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        public static Material Spine_SkeletonGraphic => AssetLoadManager.SkeletonGraphic;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => AssetManager.ObjectDatabase;
        
        /// <summary>从AnimationDef中读取动画属性作为参数结构体返回</summary>
        public static AnimationParams GetSkeletonParams(this AnimationDef def, bool loop = true)
        {
            AnimationParams @params = default;
            @params.name = def.defName;
            @params.color = def.props.color;
            @params.slotSettings = def.props.slotSettings;
            @params.skin = def.props.skin;
            @params.defaultAnimation = def.props.idleAnimationName;
            @params.rotation = def.props.rotation;
            @params.scale = def.props.scale;
            @params.timeScale = def.props.timeScale;
            @params.position = def.props.position;
            @params.loop = loop;
            return @params;
        }
        ///<summary>
        ///根据AnimationDef信息创建并初始化一个没有额外附加脚本SkeletonAnimation素体实例，默认在场景Layer的第2层
        ///<para>是本框架最重要的功能之一，实现了填入XML信息就可在指定地图位置生成Spine动画</para>
        ///</summary>
        public static GameObject CreateAnimationInstance(this AnimationDef def, int layer = 2, bool loop = true, bool DontDestroyOnLoad = false, string tag = null)
        {
            GameObject obj = null;
            AnimationParams @params = GetSkeletonParams(def, loop);
            if (def.version == "3.8")
            {
                obj = Spine38Lib.CreateAnimationSafe(def, @params, layer, active: true, DontDestroyOnLoad);
            }
            else if (def.version == "4.1")
            {
                obj = Spine41Lib.CreateAnimationSafe(def, @params, layer, active: true, DontDestroyOnLoad);
            }
            if (tag != null)
                obj.tag = tag;
            return obj;
        }
        ///<summary>(具有全局唯一性地)初始化创建一个SkeletonAnimation实例对象后返回该运行时实例</summary>
        public static GameObject CreateGlobalAnimationInstance(this AnimationDef def, int layer = 2, bool Isloop = true, bool DontDestroyOnLoad = true, string tag = null)
        {
            if (def == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = CreateAnimationInstance(def, layer, Isloop, DontDestroyOnLoad, tag);
            DynamicObjectDatabase.Add(def.defName, instance);
            return instance;
        }
        ///<summary>创建一个用于输出RenderTexture的SkeletonAnimation实例对象并进行初始化, 默认在场景Layer 5层进行渲染隔离且不可见
        ///<para>是本框架最重要的功能之一，实现了填入XML信息就可在指定屏幕位置生成Spine动画</para>
        ///</summary>
        public static GameObject Create_AnimationTextureInstance(this AnimationDef def, Vector3 pos, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = false, string tag = null, string camtag = null)
        {
            if (def == null) return null;
            GameObject instance = CreateAnimationInstance(def, layer, loop, DontDestroyOnLoad, tag);
            instance.AddRenderCameraToSkeletonAnimation(def.props.uioffset, width, height, camtag);
            instance.transform.position = pos;
            return instance;
        }
        ///<summary>(在当前游戏GC内) 初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例
        /// <para>需要一个key用于在游戏内记录当前动画实例, 默认启用的动画名称 是否循环播放. 该实例强制禁用DontDestroyOnLoad功能.</para>
        /// </summary>
        public static GameObject Create_GameOnlyAnimationTextureInstance(this AnimationDef def, object key, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = false, string tag = null, string camtag = null)
        {
            if (def == null) return null;
            GameObject obj = GC_ThingDocument.TryGetRecord(key);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            Vector3 pos = new(UnityEngine.Random.Range(-99f, 99f), UnityEngine.Random.Range(-99f, 99f), UnityEngine.Random.Range(-99f, 99f));
            GameObject instance = Create_AnimationTextureInstance(def, pos, width, height, layer, loop, DontDestroyOnLoad, tag, camtag);
            GC_ThingDocument.Add(key, instance);
            return instance;
        }
        ///<summary>(具有全局唯一性地)初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例</summary>
        public static GameObject Create_GlobalAnimationTextureInstance(this AnimationDef def, Vector3 pos, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = true, string tag = null, string camtag = null)
        {
            if (def == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = Create_AnimationTextureInstance(def, pos, layer, width, height, loop, DontDestroyOnLoad, tag, camtag);
            DynamicObjectDatabase.Add(def.defName, instance);
            return instance;
        }
        public static GameObject AddRenderCameraToSkeletonAnimation(this GameObject instance, Vector3 uioffset, int width = 1024, int height = 1024, string camtag = null)
        {
            //添加Camera
            GameObject myGO = new("RenderCamera", new Type[] { typeof(Camera) });
            Camera cam = myGO.GetComponent<Camera>();
            myGO.transform.SetParent(instance.transform);
            myGO.transform.localRotation = Quaternion.identity;
            myGO.transform.localPosition = uioffset; // X:0, Y:10, Z:-15
            //设置清除标志
            cam.clearFlags = CameraClearFlags.Color;
            //剔除遮罩: UI层
            cam.cullingMask = 1 << 5;
            cam.fieldOfView = 40f;
            cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
            cam.useOcclusionCulling = false;
            cam.renderingPath = RenderingPath.Forward;
            //cam.nearClipPlane = 0.3f;
            //cam.farClipPlane = 10f;
            cam.depth = Current.Camera.depth - 1f;
            cam.targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32, 0);
            if (camtag != null)
            {
                myGO.tag = camtag;
            }
            return myGO;
        }
        //<summary>[Pending]在Canvas上渲染Spine动画 </summary>
        [Obsolete]
        public static void Create_CanvasInstance(this AnimationDef def, Pawn pawn, bool loop = true)
        {
            /*
            Vector3 offset = new Vector3(def.props.offset.x, 0, pack.def.props.offset.y);
            Vector3 scale = new Vector3(pack.def.props.scale.x * 0.1f, 1f, pack.def.props.scale.y * 0.1f);
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Error("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return;
            }
            GameObject myGO = new GameObject
            {
                name = "myGO"
            };
            Canvas myCanvas = myGO.AddComponent<Canvas>();
            Transform parent = myCanvas.transform;
            parent.position += offset;
            parent.localScale = scale;

            Material SkeletonGraphic_alpha = Spine_SkeletonGraphic;
            if (!pack.useStraightAlpha)
            {
                SkeletonGraphic_alpha.SetFloat("_StraightAlphaInput", 0);
                SkeletonGraphic_alpha.DisableKeyword("_STRAIGHT_ALPHA_INPUT");
                Log.Message("_STRAIGHT_ALPHA_INPUT OFF");
            }
            Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
            Spine38.Unity.SkeletonGraphic graphic = Spine38.Unity.SkeletonGraphic.NewSkeletonGraphicGameObject(skeleton, parent, SkeletonGraphic_alpha);

            graphic.allowMultipleCanvasRenderers = true;
            graphic.gameObject.layer = 5;
            //graphic.rectTransform.position += offset;
            //graphic.rectTransform.localScale = scale;
            graphic.rectTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
            graphic.rectTransform.position = pawn.DrawPos + Vector3.up; ;
            graphic.AnimationState.SetAnimation(0, "Idle", loop);
            graphic.Initialize(overwrite: false);
            graphic.gameObject.SetActive(false);
            DynamicObjectDatabase.Add(pack.def.defName, graphic.gameObject);
            */
        }
        public static void DisableProbe(GameObject obj) 
        {
            var MeshRenderer = obj.GetComponent<MeshRenderer>();
            if (MeshRenderer != null) 
            {
                MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }
        }
        public static void Destory_SpineAnimationModel(string name)
        {
            GameObject obj = DynamicObjectDatabase.TryGetValue(name);
            if (obj != null)
            {
                DynamicObjectDatabase.Remove(name);
                GameObject.Destroy(obj);
            }
        }
    }
}
