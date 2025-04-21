using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public static class ObjectManager
    {
        //设置为DontDestroyOnLoad的GameObject才能在这里引用。
        public static Dictionary<object, GameObject> NeverDestoryObjects = new();
        //String会自带内存驻留，用string就是字典 用Thing就是弱表
        public static ConditionalWeakTable<object, GameObject> CurrentGameObjects => GC_AnimationDocument.instance.ObjectDataBase;

        public static void TryAddToCurrentGame(GameObject val, object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Forbidden to Add Object to GameComponent in UIEntry");

            CurrentGameObjects.TryGetValue(key, out GameObject obj);
            if (obj != null){
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
                CurrentGameObjects.Add(key, val);
            }
        }

        public static GameObject TryGetFromCurrentGame(object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Forbidden to Get Object from GameComponent in UIEntry");

            CurrentGameObjects.TryGetValue(key, out GameObject val);
            return val;
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
