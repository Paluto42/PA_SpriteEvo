using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AK_DLL;
using HarmonyLib;
using SpriteEvo;
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
            Log.Message("SpawnSetup");
            if (__instance.Dead || __instance.GetDoc() == null) return;

            Log.Message("可以创建Spine");

            string defName = "Chang_An_Test";
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);

            ProgramStateFlags flag = (ProgramStateFlags)0;
            flag |= (ProgramStateFlags)ProgramState.Playing;

            GameObject obj = SkeletonAnimationUtility.InstantiateSpine(def, __instance, allowProgramStates: flag);
            obj.transform.position = __instance.DrawPos;
            obj.SetActive(true);
        }
    }
}
