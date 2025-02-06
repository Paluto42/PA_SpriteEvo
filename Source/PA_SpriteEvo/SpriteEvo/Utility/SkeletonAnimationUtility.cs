using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [Flags]
    public enum ProgramStateFlags : byte
    {
        Entry = 1,
        MapInitializing = 2,
        Playing = 4,
    }

    public static class SkeletonAnimationUtility
    {
        public static bool currentlyGenerating = false;
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => AssetManager.GlobalObjectDatabase;

        ///<summary>
        ///根据AnimationDef信息创建并初始化一个没有额外附加脚本SkeletonAnimation素体实例，默认在场景Layer的第2层
        ///<para>是本框架最重要的功能之一，实现了填入XML信息就可在指定地图位置生成Spine动画</para>
        ///</summary>
        /*public static GameObject CreateAnimationInstance(AnimationDef def, int layer = 2, bool loop = true, bool DontDestroyOnLoad = false)
        {
            GameObject obj = null;
            //AnimationParams @params = GetSkeletonParams(def, loop);
            if (def.version == "3.8")
            {
                obj = Spine38Lib.CreateSkeletonAnimation(def, layer, loop, active: true, DontDestroyOnLoad);
            }
            else if (def.version == "4.1")
            {
                obj = Spine41Lib.CreateSkeletonAnimation(def, layer, loop, active: true, DontDestroyOnLoad);
            }
            return obj;
        }*/
        //可副本 生命周期仅限每局游戏内,加载新游戏就会被清理 没有key怎么获取自己想吧 写新方法可以用
        /*public static GameObject InstantiateInGame(AnimationDef def, bool loop = true, bool active = true)
        {
            return InstantiateInGame(def, layer: 2, loop, active);
        }
        public static GameObject InstantiateInGame(AnimationDef def, int layer = 2, bool loop = true, bool active = true)
        {
            if (Current.ProgramState != ProgramState.Playing) return null;
            return Instantiate(def, layer, loop, active, DontDestroyOnLoad: false);
        }*/
        //可副本,每局游戏内 一个key绑定一个运行时实例 不可传入NULL作为Key
        /*public static GameObject InstantiateInGameOnly(AnimationDef def, object key)
        {
            return InstantiateSpine(def, key, layer: 2, loop: true, active: true);
        }*/
        public static GameObject InstantiateSpineByDefname(string defname, string key = null, int layer = 2, bool loop = true, bool active = true, bool docuSaved = true, List<ProgramState> programStates = null)
        {
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defname);
            if (def == null)
            {
                Log.Error($"[PA] 无{defname}的AnimationDef");
                return null;
            }
            key ??= defname;
            ProgramStateFlags flag = (ProgramStateFlags)0;
            if (programStates == null) flag |= (ProgramStateFlags)ProgramState.Playing;
            else
            {
                foreach(ProgramState stat in programStates)
                {
                    flag |= (ProgramStateFlags)stat;
                }
            }

            return InstantiateSpine(def, key, layer, loop, active, docuSaved, flag);
        }

        public static GameObject InstantiateSpine(AnimationDef def, object key, int layer = 2, bool loop = true, bool active = true, bool docuSaved = true, ProgramStateFlags allowProgramStates = ProgramStateFlags.Playing)
        {
            if (((ProgramStateFlags)Current.ProgramState & allowProgramStates) == 0) return null; //游戏状况不允许
            //if (Current.ProgramState != ProgramState.Playing) return null;
            if (key == null) return null; //任何情况不允许空key
            if (docuSaved && GC_AnimationDocument.ObjectDataBase.TryGetValue(key, out GameObject res))
            {
                //Log.Warning("SpriteEvo. Duplicate Call :  Animation Instance \"" + def.defName + "\" corresponding to the key \"" + key + "\" Existed in Hierarchy");
                return res;
            }
            GameObject instance = Instantiate(def, layer, loop, active, DontDestroyOnLoad: false);
            if (instance != null && docuSaved)
            {
                GC_AnimationDocument.TryAdd(key, instance);
            }
            return instance;
        }
        //因为开一个新档会清掉所有物件，所以全局使用要DontDestroyOnLoad 基本上不考虑对它做删除操作和额外管理. 可副本
        /*public static GameObject InstantiateGlobal(AnimationDef def, int layer = 5, bool loop = true, bool active = true)
        {
            return Instantiate(def, layer, loop, active, DontDestroyOnLoad: true);
        }
        //可副本,全局一个key绑定一个运行时实例 
        public static GameObject InstantiateGlobalOnly(AnimationDef def, object key, int layer = 5, bool loop = true, bool active = true)
        {
            if (key == null) return null;
            if (AssetManager.GlobalObjectDatabase.TryGetValue(key) != null)
            {
                Log.Warning("SpriteEvo. Duplicate Call :  Animation Instance \"" + def.defName + "\" corresponding to the key \"" + key + "\" Existed in Hierarchy");
                return null;
            }
            GameObject instance = Instantiate(def, layer, loop, active, DontDestroyOnLoad: true);
            if (instance != null)
                AssetManager.GlobalObjectDatabase.Add(key, instance);
            return instance;
        }*/
        /// <summary>接受所有可选参数的主方法</summary>
        public static GameObject Instantiate(AnimationDef def, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (def == null) return null;
            GameObject instance = null;
            if (def.version == "3.8")
            {
                instance = Spine38Lib.NewSkeletonAnimation(def, layer, loop, active, DontDestroyOnLoad);
            }
            else if (def.version == "4.1")
            {
                instance = Spine41Lib.NewSkeletonAnimation(def, layer, loop, active, DontDestroyOnLoad);
            }
            if (instance == null)
            {
                return null;
            }
            if (def.props.OnIMGUI)
            {
                instance.AddRenderCameraToSkeletonAnimation(def.props.uioffset, (int)def.props.uiDrawSize.x, (int)def.props.uiDrawSize.y);
            }
            return instance;
        }
        public static void SetPosition(this GameObject instance, Vector3 pos)
        {
            if (instance == null) return;
            instance.transform.position = pos;
        }
        public static Texture AnimationTexture(this GameObject instance)
        {
            if (instance == null) return null;
            return instance.RenderCamera()?.targetTexture;
        }
        public static void DrawAnimation(Texture aTex, Rect screenRect)
        {
            if (aTex == null) return;
            Graphics.DrawTexture(screenRect, aTex);
        }
        public static Camera RenderCamera(this GameObject instance)
        {
            return instance?.GetComponentInChildren<Camera>();
        }
        public static void ResizeRenderTexture(this Camera renderCam, int width, int height)
        {
            if (renderCam == null) return;
            renderCam.targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32, 0);
        }
        ///<summary>(具有全局唯一性地)初始化创建一个SkeletonAnimation实例对象后返回该运行时实例</summary>
        /*public static GameObject CreateGlobalAnimationInstance(this AnimationDef def, int layer = 2, bool Isloop = true, bool DontDestroyOnLoad = true)
        {
            if (def == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = CreateAnimationInstance(def, layer, Isloop, DontDestroyOnLoad);
            DynamicObjectDatabase.Add(def.defName, instance);
            return instance;
        }*/
        ///<summary>创建一个用于输出RenderTexture的SkeletonAnimation实例对象并进行初始化, 默认在场景Layer 5层进行渲染隔离且不可见
        ///<para>是本框架最重要的功能之一，实现了填入XML信息就可在指定屏幕位置生成Spine动画</para>
        ///</summary>
        /*public static GameObject Create_AnimationTextureInstance(this AnimationDef def, Vector3 pos, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = false)
        {
            if (def == null) return null;
            GameObject instance = CreateAnimationInstance(def, layer, loop, DontDestroyOnLoad);
            instance.AddRenderCameraToSkeletonAnimation(def.props.uioffset, width, height);
            instance.transform.position = pos;
            return instance;
        }*/
        ///<summary>(在当前游戏GC内) 初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例
        /// <para>需要一个key用于在游戏内记录当前动画实例, 默认启用的动画名称 是否循环播放. 该实例强制禁用DontDestroyOnLoad功能.</para>
        /// </summary>
        /*public static GameObject Create_GameOnlyAnimationTextureInstance(this AnimationDef def, Vector3 pos, object key, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = false)
        {
            if (def == null) return null;
            GameObject obj = GC_AnimationDocument.TryGet(key);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = Create_AnimationTextureInstance(def, pos, width, height, layer, loop, DontDestroyOnLoad);
            GC_AnimationDocument.TryAdd(key, instance);
            return instance;
        }*/
        ///<summary>(具有全局唯一性地)初始化创建一个用于输出RenderTexture的SkeletonAnimation实例对象后返回该运行时实例</summary>
        /*public static GameObject Create_GlobalAnimationTextureInstance(this AnimationDef def, Vector3 pos, int width = 1024, int height = 1024, int layer = 5, bool loop = true, bool DontDestroyOnLoad = true)
        {
            if (def == null) return null;
            GameObject obj = DynamicObjectDatabase.TryGetValue(def.defName);
            if (obj != null)
            {
                Log.Warning("[PA]. Duplicate Call :  Animation Instance  \"" + def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject instance = Create_AnimationTextureInstance(def, pos, layer, width, height, loop, DontDestroyOnLoad);
            DynamicObjectDatabase.Add(def.defName, instance);
            return instance;
        }*/
        public static Camera AddRenderCameraToSkeletonAnimation(this GameObject instance, Vector3 uioffset, int width = 1024, int height = 1024)
        {
            //添加Camera
            if (instance == null) return null;
            instance.layer = 5;
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
            return cam;
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
