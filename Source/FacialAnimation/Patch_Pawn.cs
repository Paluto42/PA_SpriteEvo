using AK_DLL;
using HarmonyLib;
using SpriteEvo;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using AnimationDef = SpriteEvo.AnimationDef;

namespace PA_SpriteEvo.FacialAnimation
{

    [HarmonyPatch(typeof(Pawn), "SpawnSetup", new Type[] { typeof(Map), typeof(bool) })]
    public class Patch_Pawn
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Dead || __instance.GetDoc() == null) return;
            Log.Message("SpawnSetup");

            if (!GC_AnimationController.instance.registedPawns.Contains(__instance))
            {
                //Patch_PawnRenderer.registeredPawns.Add(__instance);
                GC_AnimationController.instance.registedPawns.Add(__instance);
                Log.Message("缓存了 " + __instance.Label);
            }
        }
    }

    /*[HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt", new Type[] { typeof(Vector3), typeof(Rot4?), typeof(bool) })]
    public class Patch_PawnRenderer
    {
        public static HashSet<Pawn> registeredPawns = new HashSet<Pawn>();

        [HarmonyPostfix]
        public static void Postfix(Pawn ___pawn)
        {
            if (Current.ProgramState != ProgramState.Playing) return;
            if (!registeredPawns.Contains(___pawn)) return;

            if (ObjectManager.CurrentObjectTrackers.TryGetValue(___pawn, out AnimationTracker res)) return;

            Log.Message("可以创建Spine");
            string defName = "Chang_An_Test";
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);

            ProgramStateFlags flag = (ProgramStateFlags)0;
            flag |= (ProgramStateFlags)ProgramState.Playing;

            GameObject obj = SkeletonAnimationUtility.InstantiateSpine(def, ___pawn, allowProgramStates: flag);
            obj.transform.position = ___pawn.DrawPos;
            obj.SetActive(true);
        }
    }*/
}
