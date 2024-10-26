using System.Collections.Generic;
using Verse;
using UnityEngine;
using Spine38.Unity;
using Spine41.Unity;
using System;

namespace PA_SpriteEvo
{
    public static class AssetExtensions
    {
        public static bool Is_StraightAlphaTexture = false;
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        public static Material Spine_SkeletonGraphic => AssetLoadManager.SkeletonGraphic;
        public static Dictionary<string, GameObject> DynamicObjectDatabase => AssetManager.ObjectDatabase;

        ///<summary>获取一个模型实例的定位点Bone</summary>
        public static Spine38.Bone RootBone(this Spine38.Unity.SkeletonAnimation instance)
        {
            return instance.skeleton.RootBone;
        }
        public static Spine41.Bone RootBone(this Spine41.Unity.SkeletonAnimation instance)
        {
            return instance.skeleton.RootBone;
        }
        ///<summary>获取一个模型实例里的Bone (可能为Null)</summary>
        public static Spine38.Bone GetBone(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            return instance.skeleton.FindBone(name);
        }
        public static Spine41.Bone GetBone(this Spine41.Unity.SkeletonAnimation instance, string name)
        {
            return instance.skeleton.FindBone(name);
        }
        ///<summary>获取一个模型实例里的Bone的实际坐标 (不推荐在动画运动时使用)</summary>
        public static Vector3 GetBonePositon(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            Spine38.Bone bone = instance.skeleton.FindBone(name);
            if (bone == null)
            {
                return Vector3.zero;
            }
            return bone.GetWorldPosition(instance.transform);
        }
        public static Vector3 GetBonePositon(this Spine41.Unity.SkeletonAnimation instance, string name)
        {
            Spine41.Bone bone = instance.skeleton.FindBone(name);
            if (bone == null)
            {
                return Vector3.zero;
            }
            return bone.GetWorldPosition(instance.transform);
        }
        ///<summary>访问SkeletonAnimation实例的SkeletonDataAsset 获取指定名称的Spine动画数据对象</summary>
        public static Spine38.Animation GetAnimation(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            return instance?.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(name);
        }
        ///<summary>访问SkeletonAnimation实例的SkeletonDataAsset 返回包含该骨骼文件的全部动画对象的列表</summary>
        public static List<Spine38.Animation> GetAllAnimations(this Spine38.Unity.SkeletonAnimation instance)
        {
            Spine38.ExposedList<Spine38.Animation> animations = instance?.SkeletonDataAsset.GetSkeletonData(false).Animations;
            List<Spine38.Animation> IList = new List<Spine38.Animation>();
            for (int i = 0, n = animations.Count; i < n; i++)
            {
                Spine38.Animation animation = animations.Items[i];
                IList.Add(animation);
            }
            return IList;
        }
        ///<summary>通过SpineAssetPack实例 来创建SkeletonData Asset</summary>
        public static Spine38.Unity.SkeletonDataAsset Create_SkeletonDataAsset38(this SpineAssetPack pack)
        {
            if (pack == null)
            {
                Log.Error("SpineAssetPack 为空引用");
                return null;
            }
            Spine38.Unity.SkeletonDataAsset skeleton;
            if (pack.useMaterial)
            {
                Spine38.Unity.SpineAtlasAsset atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(pack.atlasData, pack.materials, initialize: true);
                skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(pack.skeletonData, atlas, initialize: true);
            }
            else
            {
                //Is_StraightAlphaTexture = pack.useStraightAlpha;
                Spine38.Unity.SpineAtlasAsset atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(pack.atlasData, pack.textures, pack.shader, initialize: true);
                skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(pack.skeletonData, atlas, initialize: true);
                //Is_StraightAlphaTexture = false;
            }
            return skeleton;
        }
        public static Spine41.Unity.SkeletonDataAsset Create_SkeletonDataAsset41(this SpineAssetPack pack)
        {
            if (pack == null)
            {
                Log.Error("SpineAssetPack 为空引用");
                return null;
            }
            Spine41.Unity.SkeletonDataAsset skeleton;
            if (pack.useMaterial)
            {
                Spine41.Unity.SpineAtlasAsset atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(pack.atlasData, pack.materials, initialize: true);
                skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(pack.skeletonData, atlas, initialize: true);
            }
            else
            {
                //Is_StraightAlphaTexture = pack.useStraightAlpha;
                Spine41.Unity.SpineAtlasAsset atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(pack.atlasData, pack.textures, pack.shader, initialize: true);
                skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(pack.skeletonData, atlas, initialize: true);
                //Is_StraightAlphaTexture = false;
            }
            return skeleton;
        }
       
        ///<summary>创建一个用于输出RenderTexture的SkeletonAnimation实例对象并进行初始化</summary>
        internal static void Create_AnimationTextureInstance(this SpineAssetPack pack, bool loop = true)
        {
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Error("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return;
            }
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
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin); ;
                //TrackEntry 
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                //添加Camera
                GameObject myGO = new GameObject("RenderCamera38" + "_" + pack.def.defName, new Type[] { typeof(Camera) });
                Camera cam = myGO.GetComponent<Camera>();
                myGO.transform.SetParent(animation.transform);
                myGO.transform.localRotation = Quaternion.identity;
                myGO.transform.localPosition = pack.def.uioffset; // X:0, Y:10, Z:-15
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
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
            }
            else if (version == "4.1")
            {
                Spine41.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset41();
                Spine41.Unity.SkeletonAnimation animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.gameObject.name = pack.def.defName;
                animation.gameObject.layer = 5;
                animation.transform.rotation = Quaternion.Euler(pack.def.rotation);
                animation.transform.localScale = scale;
                animation.skeleton.SetSkin(pack.def.props.skin); ;
                //TrackEntry 
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                //添加Camera
                GameObject myGO = new GameObject("RenderCamera41" + "_" + pack.def.defName, new Type[] { typeof(Camera) });
                Camera cam = myGO.GetComponent<Camera>();
                myGO.transform.SetParent(animation.transform);
                myGO.transform.localRotation = Quaternion.identity;
                myGO.transform.localPosition = new Vector3(0f, 10f, -30f); // X:0, Y:10, Z:-15
                //Layer: 5
                cam.cullingMask = 1 << 5;
                cam.fieldOfView = 40f;
                cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
                cam.useOcclusionCulling = false;
                cam.renderingPath = RenderingPath.Forward;
                //cam.nearClipPlane = 0.3f;
                //cam.farClipPlane = 10f;
                cam.depth = Current.Camera.depth - 1f;
                cam.targetTexture = new RenderTexture(1000, 1000, 32, RenderTextureFormat.ARGB32, 0);
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
            }
        }
        ///<summary>创建一个SkeletonAnimation实例对象并进行初始化</summary>
        ///
        internal static GameObject CreateAnimationInstance (this SpineAssetPack pack, bool loop = true)
        {
            if (pack == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Error("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
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
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
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
                animation.AnimationState.SetAnimation(0, pack.def.props.idleAnimationName, loop);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: false);
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
                DynamicObjectDatabase.Add(pack.def.defName, animation.gameObject);
                return animation.gameObject;
            }
            return null;
        }
        [Obsolete]
        internal static void Create_CanvasInstance(this SpineAssetPack pack, Pawn pawn, bool loop = true)
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
        }
        internal static void Destory_SpineAnimationModel(string name)
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
