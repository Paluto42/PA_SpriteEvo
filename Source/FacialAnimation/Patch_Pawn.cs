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
            if (__instance.Dead || __instance.GetDoc() == null) return;

            string defName = "SE_ChangAn";
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);
            GameObject instance = SkeletonAnimationUtility.InstantiateSpine(def, __instance);
            instance.SetActive(true);
        }
    }
}
