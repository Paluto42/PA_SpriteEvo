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
        public static Dictionary<object, GameObject> NeverDestoryObjectDatabase = new();
        //String会自带内存驻留，用string就是字典 用Thing就是弱表
        public static ConditionalWeakTable<object, GameObject> CurrentGameObjectDataBase => GC_AnimationDocument.instance.ObjectDataBase;

        public static void TryAddToCurrentGame(GameObject val, object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Failed to Get GameComponent in UIEntry");

            CurrentGameObjectDataBase.TryGetValue(key, out GameObject obj);
            if (obj != null){
                Log.Error("SpriteEvo. Error while Adding new Value: The same Key already exists in Current Game");
                return;
            }
            else{
                CurrentGameObjectDataBase.Add(key, val);
            }
        }

        public static GameObject TryGetFromCurrentGame(object key)
        {
            if (Current.ProgramState == ProgramState.Entry)
                throw new InvalidOperationException("SpriteEvo. Failed to Get GameComponent in UIEntry");

            CurrentGameObjectDataBase.TryGetValue(key, out GameObject val);
            return val;
        }

        public static void TryAddPermanent(object key, GameObject value)
        {
            if (NeverDestoryObjectDatabase.ContainsKey(key)) {
                Log.Error("SpriteEvo. Error while Adding new Value: The same Key already exists in Scene"); 
                return; 
            }
            else{
                NeverDestoryObjectDatabase.Add(key, value);
            }
        }

        //May be Null
        public static GameObject TryGetPermanent(object key)
        {
            if (NeverDestoryObjectDatabase.ContainsKey(key)) {
                return NeverDestoryObjectDatabase[key]; 
            }
            else {
                return null;
            }
        }
    }
}
