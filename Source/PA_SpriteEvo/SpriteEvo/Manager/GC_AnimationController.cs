using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public class GC_AnimationController : GameComponent
    {
        public static GC_AnimationController instance;

        public HashSet<Pawn> registedPawns = new();

        public Dictionary<Pawn, AnimationTracker> pawnTrackerDB = new();

        public ConditionalWeakTable<object, AnimationTracker> TrackerDataBase => GC_AnimationDocument.instance.TrackerDataBase;

        public GC_AnimationController(Game game)
        {
            instance= this;
        }

        public override void GameComponentTick()
        {
            if (registedPawns.Count == 0) return; 
            foreach (var pawn in registedPawns)
            {
                if (!pawnTrackerDB.TryGetValue(pawn, out AnimationTracker tracker))
                {
                    //下个tick出结果
                    if (TrackerDataBase.TryGetValue(pawn, out AnimationTracker res))
                    {
                        Cache(pawn, res);
                    }
                    else
                    {
                        Instantiate(pawn);
                    }
                }
                tracker?.Tick();
            }
        }

        //test
        private void Instantiate(Pawn pawn, string defName = "Chang_An_Test") 
        {
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);
            if (def == null) return;
            ProgramStateFlags flag = (ProgramStateFlags)0;
            flag |= (ProgramStateFlags)ProgramState.Playing;
            GameObject obj = SkeletonAnimationUtility.InstantiateSpine(def, pawn, allowProgramStates: flag);
            obj.transform.position = pawn.DrawPos + Vector3.up;//debug
            obj.SetActive(true);
        }

        private void Cache(Pawn pawn, AnimationTracker tracker) 
        {
            if (pawnTrackerDB.ContainsKey(pawn)) return;
            pawnTrackerDB.Add(pawn, tracker);
        }
    }
}
