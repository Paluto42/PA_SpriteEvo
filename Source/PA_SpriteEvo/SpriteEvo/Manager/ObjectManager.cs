using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //打算整合进GC里
    [StaticConstructorOnStartup]
    public static class ObjectManager
    {
        //设置为DontDestroyOnLoad的GameObject才能在这里引用。
        public static Dictionary<object, GameObject> NeverDestoryObjects = new();
        //String会自带内存驻留，用string就是字典 用Thing就是弱表
        public static ConditionalWeakTable<object, AnimationTracker> CurrentObjectTrackers => GC_AnimationDocument.instance.animationTrackerDocument;

        public static void TryAddToCurrentGame(GameObject val, object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Forbidden to Add Object to GameComponent in UIEntry");

            CurrentObjectTrackers.TryGetValue(key, out AnimationTracker res);
            if (res != null){
                Log.Error("SpriteEvo. Error while Adding new Value: The same Foreign Key already exists in Current Game");
                return;
            }
            else{
                var comps = val.GetComponentsInChildren<ScriptBase>(includeInactive: true);
                //给脚本加上 外键引用? 这很符合数据库原理
                if (!comps.NullOrEmpty())
                {
                    foreach (var comp in comps)
                    {
                        comp.referenceKey = key;
                    }
                }
                AnimationTracker tracker = new(val);
                CurrentObjectTrackers.Add(key, tracker);
            }
        }

        public static AnimationTracker TryGetFromCurrentGame(object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Forbidden to Get Object from GameComponent in UIEntry");

            CurrentObjectTrackers.TryGetValue(key, out AnimationTracker res);
            return res;
        }

        public static void TryAddPermanent(object key, GameObject value)
        {
            if (NeverDestoryObjects.ContainsKey(key)) {
                Log.Error("SpriteEvo. Error while Adding new Object: The same Foreign Key already exists in Scene"); 
                return; 
            }
            else{
                NeverDestoryObjects.Add(key, value);
            }
        }

        //May be Null
        public static GameObject TryGetPermanent(object key)
        {
            if (NeverDestoryObjects.ContainsKey(key)) {
                return NeverDestoryObjects[key]; 
            }
            else {
                return null;
            }
        }
    }
}
