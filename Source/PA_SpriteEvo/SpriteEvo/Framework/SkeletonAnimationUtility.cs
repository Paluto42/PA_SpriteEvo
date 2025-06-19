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
        public static Shader Spine_Skeleton => AssetLoadService.Spine_Skeleton;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => ObjectManager.NeverDestoryObjects;

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
                Log.Error($"[SpriteEvo] 无{defname}的AnimationDef");
                return null;
            }
            key ??= defname;
            ProgramStateFlags flag = (ProgramStateFlags)0;
            if (programStates == null) flag |= (ProgramStateFlags)ProgramState.Playing;
            else
            {
                foreach (ProgramState stat in programStates)
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
            if (key == null)
            {
                throw new NullReferenceException("SpriteEvo. Tried to Invoke Instantiate with Null Foreign Key"); //任何情况不允许空key
            }
            if (docuSaved && ObjectManager.CurrentObjectTrackers.TryGetValue(key, out AnimationTracker res))
            {
                res.instanceInt.SetActive(true);
                //Log.Warning("SpriteEvo. Duplicate Call :  Animation Instance \"" + def.defName + "\" corresponding to the key \"" + key + "\" Existed in Hierarchy");
                return res.instanceInt;
            }
            GameObject instance = Instantiate(def, layer, loop, active, DontDestroyOnLoad: false);
            if (instance != null && docuSaved)
            {
                ObjectManager.TryAddToCurrentGame(instance, key);
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
            if (def == null)
            {
                throw new NullReferenceException("SpriteEvo. Tried to Invoke Instantiate SkeletonAnimation with Null AnimationDef");
            }
            GameObject instance = null;
            if (def.version == "3.8")
            {
                instance = Spine38Lib.NewSkeletonAnimation(def, layer, loop, active, DontDestroyOnLoad);
            }
            else if (def.version == "4.1")
            {
                instance = Spine41Lib.NewSkeletonAnimation(def, layer, loop, active, DontDestroyOnLoad);
            }
            else if (def.version == "4.2")
            {
                instance = Spine42Lib.NewSkeletonAnimation(def, layer, loop, active, DontDestroyOnLoad);
            }
            return instance;
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
