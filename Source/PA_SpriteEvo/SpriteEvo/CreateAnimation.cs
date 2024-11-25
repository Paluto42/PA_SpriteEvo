using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public static class CreateAnimation
    {
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        public static Material Spine_SkeletonGraphic => AssetLoadManager.SkeletonGraphic;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => AssetManager.ObjectDatabase;
        public struct GenAnimationRequest
        {
            public string name;
            public int layer;
            public Vector3 rotation;
            public Vector3 scale;
            public string defaultskin;
            public string defaultAnimation;
        }

        ///<summary>(在当前游戏GC内) 在指定坐标位置初始化创建一个SkeletonAnimation实例对象后返回该运行时实例
        /// <para>需要一个key用于在游戏内记录当前动画实例, 默认启用的动画名称 是否循环播放. 该实例强制禁用DontDestroyOnLoad功能.</para>
        /// <para>懒人模式 填好XML 给出初始动画名称和坐标 即可.</para>
        /// </summary>
        public static GameObject CreateAnimationInstanceAtPos(this SkeletonLoader pack, Vector3 pos, string InitAnimation = null, bool visiable = true, bool loop = true, bool DontDestroyOnLoad = false)
        {
            if (pack == null) return null;
            Vector3 offset = new Vector3(pack.def.offset.x, 0, pack.def.offset.y);
            Vector3 scale = new Vector3(pack.def.scale.x * 0.1f, pack.def.scale.y * 0.1f, 1f);
            string version = pack.def.props.version;
            if (version == "3.8")
            {
                Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
                Spine38.Unity.SkeletonAnimation animation = Spine38.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = pack.def.defName;
                animation.gameObject.layer = 5;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin);
                //TrackEntry 
                if (InitAnimation == null)
                    InitAnimation = pack.def.props.idleAnimationName;
                animation.AnimationState.SetAnimation(0, InitAnimation, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: visiable);
                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                //DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
                animation.transform.position = pos;
                return animation.gameObject;
            }
            else if (version == "4.1")
            {
                Spine41.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset41();
                Spine41.Unity.SkeletonAnimation animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = "Spine_" + pack.def.defName;
                animation.gameObject.layer = 2;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                animation.transform.localScale = scale;
                //newObject.skeleton.SetSkin();
                //TrackEntry 
                if (InitAnimation == null)
                    InitAnimation = pack.def.props.idleAnimationName;
                animation.AnimationState.SetAnimation(0, InitAnimation, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                return animation.gameObject;
            }
            return null;
        }
        ///<summary>(在当前游戏GC内) 初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例
        /// <para>需要一个key用于在游戏内记录当前动画实例, 默认启用的动画名称 是否循环播放. 该实例强制禁用DontDestroyOnLoad功能.</para>
        /// </summary>
        public static GameObject Create_GameOnlyAnimationTextureInstance(this SkeletonLoader pack, object key, string InitAnimation = null, bool loop = true)
        {
            if (pack == null) return null;
            GameObject obj = GC_GameObjectManager.TryGetRecord(key);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = Create_AnimationTextureInstance(pack, InitAnimation, loop);
            GC_GameObjectManager.Add(key, instance);
            return instance;
        }
        ///<summary>(具有全局唯一性地)初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例</summary>
        public static GameObject Create_GlobalAnimationTextureInstance(this SkeletonLoader pack, string InitAnimation = null, bool loop = true, bool DontDestroyOnLoad = true)
        {
            if (pack == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = Create_AnimationTextureInstance(pack, InitAnimation, loop, DontDestroyOnLoad);
            DynamicObjectDatabase.Add(pack.def.defName, instance);
            return instance;
        }
        ///<summary>创建一个用于输出RenderTexture的SkeletonAnimation实例对象并进行初始化</summary>
        public static GameObject Create_AnimationTextureInstance(this SkeletonLoader pack, string animationName = null, bool loop = true, bool DontDestroyOnLoad = false)
        {
            if (pack == null) return null;
            Vector3 scale = new Vector3(pack.def.scale.x * 0.1f, pack.def.scale.y * 0.1f, 1f);
            string version = pack.def.props.version;
            if (version == "3.8")
            {

                Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
                if (skeleton == null) { return null; }
                Spine38.Unity.SkeletonAnimation animation = Spine38.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = pack.def.defName;
                animation.gameObject.layer = 5;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin); ;
                //TrackEntry 
                if (animationName == null)
                    animationName = pack.def.props.idleAnimationName;
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                //添加Camera
                animation.gameObject.AddCameraToSkeletonAnimation("RenderCamera38" + "_" + pack.def.defName, pack.def.uioffset);

                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                //DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
                return animation.gameObject;
            }
            else if (version == "4.1")
            {
                Spine41.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset41();
                if (skeleton == null) { return null; }
                Spine41.Unity.SkeletonAnimation animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = pack.def.defName;
                animation.gameObject.layer = 5;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin); ;
                //TrackEntry 
                if (animationName == null)
                    animationName = pack.def.props.idleAnimationName;
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                //添加Camera
                Vector3 offset = new(0f, 10f, -30f);
                animation.gameObject.AddCameraToSkeletonAnimation("RenderCamera41" + "_" + pack.def.defName, offset);

                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                //DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
                return animation.gameObject;
            }
            return null;
        }
        ///<summary>(具有全局唯一性地)初始化创建一个SkeletonAnimation实例对象后返回该运行时实例</summary>
        public static GameObject CreateGlobalAnimationInstance(this SkeletonLoader pack, string animationName = null, bool visiable = false, bool Isloop = true, bool DontDestroyOnLoad = true)
        {
            if (pack == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = CreateAnimationInstance(pack, animationName, visiable, Isloop, DontDestroyOnLoad);
            DynamicObjectDatabase.Add(pack.def.defName, instance);
            return instance;
        }
        ///<summary>创建一个SkeletonAnimation实例对象并进行初始化</summary>
        public static GameObject CreateAnimationInstance(this SkeletonLoader pack, string animationName = null, bool visiable = false, bool Isloop = true, bool DontDestroyOnLoad = false)
        {
            if (pack == null) return null;
            Vector3 offset = new Vector3(pack.def.offset.x, 0, pack.def.offset.y);
            Vector3 scale = new Vector3(pack.def.scale.x * 0.1f, pack.def.scale.y * 0.1f, 1f);
            string version = pack.def.props.version;
            if (version == "3.8")
            {
                Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
                Spine38.Unity.SkeletonAnimation animation = Spine38.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = pack.def.defName;
                animation.gameObject.layer = 5;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin);
                //TrackEntry 
                if (animationName == null)
                    animationName = pack.def.props.idleAnimationName;
                animation.AnimationState.SetAnimation(0, animationName, Isloop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: visiable);
                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                //DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
                return animation.gameObject;
            }
            else if (version == "4.1")
            {
                Spine41.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset41();
                Spine41.Unity.SkeletonAnimation animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = "Spine_" + pack.def.defName;
                animation.gameObject.layer = 2;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                animation.transform.localScale = scale;
                //newObject.skeleton.SetSkin();
                //TrackEntry 
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, Isloop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                if (DontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                return animation.gameObject;
            }
            return null;
        }
        public static GameObject AddCameraToSkeletonAnimation(this GameObject instance, string name, Vector3 offset)
        {
            //添加Camera
            GameObject myGO = new(name, new Type[] { typeof(Camera) });
            Camera cam = myGO.GetComponent<Camera>();
            myGO.transform.SetParent(instance.transform);
            myGO.transform.localRotation = Quaternion.identity;
            myGO.transform.localPosition = offset; // X:0, Y:10, Z:-15
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
            cam.targetTexture = new RenderTexture(1024, 1024, 32, RenderTextureFormat.ARGB32, 0);
            return myGO;
        }
        [Obsolete]
        public static void Create_CanvasInstance(this SkeletonLoader pack, Pawn pawn, bool loop = true)
        {
            Vector3 offset = new Vector3(pack.def.offset.x, 0, pack.def.offset.y);
            Vector3 scale = new Vector3(pack.def.scale.x * 0.1f, 1f, pack.def.scale.y * 0.1f);
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
            /*if (!pack.useStraightAlpha)
            {
                SkeletonGraphic_alpha.SetFloat("_StraightAlphaInput", 0);
                SkeletonGraphic_alpha.DisableKeyword("_STRAIGHT_ALPHA_INPUT");
                Log.Message("_STRAIGHT_ALPHA_INPUT OFF");
            }*/
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
