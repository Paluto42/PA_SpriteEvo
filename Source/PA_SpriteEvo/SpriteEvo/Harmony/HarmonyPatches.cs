using HarmonyLib;
using SpriteEvo.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace SpriteEvo
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

    //用来加载全部文件的Prefix方法
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
    //用来清除

    //应该合并进AK_DLL的招募流程中，不需要去每帧判断。
    /*[HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class PatchWindowOnGUI
    {
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
            //TrySetDynModel(doc, pawn);
            TrySetPawnAnimation(pawn);
        }
        private static void TrySetDynModel(OperatorDocument doc, Pawn pawn)
        {
            List<SpineAssetDef> list = DefDatabase<SpineAssetDef>.AllDefsListForReading;
            string defName = "AK_Spine_Dusk_dynillust_Nian";
            foreach (SpineAssetDef def in list)
            {
                string[] parts = def.defName.Split('_');
                if (doc.operatorID.EndsWith(parts[2]))
                {
                    defName = def.defName;
                    break;
                }
            }
            AnimationDef animationdef = DefDatabase<AnimationDef>.AllDefsListForReading.Find(a => a.defName == defName);
            //SkeletonLoader pack = AssetManager.spine38_Database?.TryGetValue(defName); ;
            if (animationdef == null)
            {
                Log.Error("[PA]. SpineAssetPack " + animationdef + "Not Found");
                return;
            }
            GameObject obj = GC_ThingDocument.TryGetRecord(animationdef.defName);
            //GameObject obj = AssetManager.ObjectDatabase.TryGetValue(pack.def.defName);
            if (obj == null)
            {
                //Add
                animationdef.Create_GameOnlyAnimationTextureInstance(animationdef.defName);
                //pack.Create_GlobalAnimationTextureInstance();
            }
            else
            {
                //obj.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                obj?.SetActive(!pawn.Downed);
                try
                {
                    GUI.DrawTexture(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 500, 500), obj.GetComponentInChildren<Camera>().targetTexture, scaleMode: ScaleMode.ScaleToFit);
                }
                catch (Exception){
                    throw;
                }
            }
        }
        private static void TrySetPawnAnimation(Pawn pawn)
        {
            var test = DefDatabase<AnimationDef>.AllDefsListForReading.FirstOrDefault(pd => pd.defName == "Chang_An_Test");
            if (test == null)
            {
                Log.Error("PawnKindSpriteDef Not Found");
                return;
            }
            GameObject obj = GC_ThingDocument.TryGetRecord(pawn);
            //GameObject obj = AssetManager.ThingObjectDatabase.TryGetValue(pawn);
            if (obj == null)
            {
                //AnimationGenerator.CreatePawnAnimationModel(pawn, test);
                AnimationGenerator.MergeAnimation(pawn, test);
            }
            if (obj != null)
            {
                obj?.SetActive(!pawn.Downed || !pawn.Dead);
            }
        }*/
}
