using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>用来缓存动画信息的GC,在游戏中才可以使用</summary>
    public class GC_AnimationManager : GameComponent
    {
        private bool isRegisterUpdated = false;

        public static GC_AnimationManager instance;

        //key没有其他强引用时，这里面的key的键值对会在gc时被自动移除。但是gc不是实时的。
        //public ConditionalWeakTable<object, GameObject> ObjectDataBase = new();
        public ConditionalWeakTable<object, AnimationTracker> animationTrackerDocument = new();

        // 使用动画的小人的缓存
        private readonly HashSet<Pawn> registedPawns = new();
        public Dictionary<Pawn, AnimationTracker> pawnTrackerDocument = new();

        public GC_AnimationManager(Game game)
        {
            instance = this;
        }

        public override void GameComponentTick()
        {
            if (isRegisterUpdated == false) return;

            foreach (var pawn in registedPawns)
            {
                if (pawnTrackerDocument.ContainsKey(pawn) == false)
                {
                    if (animationTrackerDocument.TryGetValue(pawn, out AnimationTracker res))
                    {
                        CacheToDocument(pawn, res);
                    }
                    else
                    {
                        Instantiate(pawn);
                    }
                }
                //tracker?.Tick();
            }
            isRegisterUpdated = false;
        }

        public bool Contains(Pawn pawn)
        {
            return registedPawns.Contains(pawn);
        }

        //必须通过这里来刷新状态
        public void RegisterForPawn(Pawn pawn)
        {
            if (pawn == null) return;
            registedPawns.Add(pawn);
            isRegisterUpdated = true;
        }

        private void CacheToDocument(Pawn pawn, AnimationTracker tracker)
        {
            if (pawnTrackerDocument.ContainsKey(pawn)) return;
            pawnTrackerDocument.Add(pawn, tracker);
        }

        //test
        private void Instantiate(Pawn pawn, string defName = "Chang_An_Test")
        {
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);
            if (def == null) return;
            #region
            ProgramStateFlags flag = (ProgramStateFlags)0;
            flag |= (ProgramStateFlags)ProgramState.Playing;
            #endregion
            GameObject obj = SkeletonAnimationUtility.InstantiateSpine(def, pawn, allowProgramStates: flag);
            obj.transform.position = pawn.DrawPos + Vector3.up;//debug
            obj.SetActive(true);
        }
    }
}
