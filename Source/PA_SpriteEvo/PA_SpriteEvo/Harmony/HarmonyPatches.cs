using AK_DLL;
using HarmonyLib;
using PA_SpriteEvo.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    [StaticConstructorOnStartup]
    internal class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony Instance = new Harmony("paluto22.SpineFramework.patch");
            Instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    /*
    [HarmonyPatch(typeof(Spine38.Unity.SpineAtlasAsset), "CreateRuntimeInstance", new Type[] { typeof(TextAsset), typeof(Texture2D[]), typeof(Material), typeof(bool) } )]
    public class Patch_SpineAtlasAsset
    {
        [HarmonyPrefix]
        public static bool Prefix_CreateAtlas(ref Material materialPropertySource) 
        {
            if (!SpineFramework_Tool.Is_StraightAlphaTexture)
            {
                materialPropertySource.EnableKeyword("_STRAIGHT_ALPHA_INPUT");
                materialPropertySource.SetFloat("_StraightAlphaInput", 1);
            }
            return true;
        }
    }*/

    [HarmonyPatch(typeof(UIRoot_Entry), "DoMainMenu")]
    public class Patch_UIRoot
    {
        [HarmonyPrefix]
        public static bool Prefix(UIRoot_Entry __instance)
        {
            if (!AssetLoadManager.AllAssetsLoaded)
            {
                AssetLoadManager.ResloveAllAssetBundle();
                AssetLoadManager.AllAssetsLoaded = true;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class PatchWindowOnGUI
    {
        private static void TrySetDynModel(OperatorDocument doc, Pawn pawn) 
        {
            List<SpinePackDef> list = DefDatabase<SpinePackDef>.AllDefsListForReading;
            string defName = "AK_Spine_Dusk_dynillust_Nian";
            foreach (SpinePackDef def in list)
            {
                string[] parts = def.defName.Split('_');
                if (doc.operatorID.EndsWith(parts[2]))
                {
                    defName = def.defName;
                    break;
                }
            }
            SpineAssetPack pack = AssetManager.spine38_Database?.TryGetValue(defName); ;
            if (pack == null)
            {
                Log.Error("[PA]. SpineAssetPack " + defName + "Not Found");
                return;
            }
            //GameObject obj = GameObject.Find(pack.def.defName);
            GameObject obj = AssetManager.ObjectDatabase.TryGetValue(pack.def.defName);
            if (obj == null)
            {
                pack.Create_AnimationTextureInstance();
            }
            if (obj != null)
            {
                //obj.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                obj?.SetActive(pawn.Drafted || pawn.Downed);
                try
                {
                    GUI.DrawTexture(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 500, 500), AssetManager.ObjectDatabase[defName].GetComponentInChildren<Camera>().targetTexture, scaleMode: ScaleMode.ScaleToFit);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return;
            }
        }
        private static void TrySetPawnAnimation(OperatorDocument doc, Pawn pawn) 
        {
            PawnKindSpriteDef test = DefDatabase<PawnKindSpriteDef>.AllDefsListForReading.FirstOrDefault(pd => pd.defName == "Chang_An");
            if (test == null) 
            {
                Log.Error("PawnKindSpriteDef Not Found");
                return;
            }
            GameObject obj = AssetManager.ThingObjectDatabase.TryGetValue(pawn);
            if (obj == null)
            {
                AnimationGenerator.CreatePawnAnimationModel(pawn, test);
            }
            if (obj != null)
            {
                obj?.SetActive(pawn.Drafted || pawn.Downed);
            }
        }
        [HarmonyPrefix]
        public static void Prefix_DrawBottomLeftPortrait()
        {
            if (!AK_ModSettings.displayBottomLeftPortrait || Find.World == null || Find.CurrentMap == null || Find.Selector == null || !Find.Selector.AnyPawnSelected || Find.Selector.SelectedPawns.Count == 0)
            {
                return;
            }
            Pawn pawn = Find.Selector.SelectedPawns.First();
            if (pawn == null) return;
            OperatorDocument doc = pawn.GetDoc();
            if (doc == null) return;
            if (!AssetLoadManager.AllAssetsLoaded) return;
            TrySetDynModel(doc, pawn);
            TrySetPawnAnimation(doc, pawn);
        }
    }
}
