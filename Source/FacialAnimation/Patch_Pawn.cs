using AK_DLL;
using HarmonyLib;
using SpriteEvo;
using System;
using Verse;

namespace PA_SpriteEvo.FacialAnimation
{

    [HarmonyPatch(typeof(Pawn), "SpawnSetup", new Type[] { typeof(Map), typeof(bool) })]
    public class Patch_Pawn
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Dead) return;
            if (__instance.GetDoc() == null) return; //真正的逻辑
            Log.Message("SpawnSetup");

            if (GC_AnimationDocument.instance.Contains(__instance) == false)
            {
                GC_AnimationDocument.instance.Register(__instance);
                Log.Message("缓存了 " + __instance.Label);
            }
        }
    }
}
