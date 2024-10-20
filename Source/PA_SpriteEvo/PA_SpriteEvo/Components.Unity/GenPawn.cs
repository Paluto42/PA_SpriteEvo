using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    public static class GenPawn
    {
        private static Dictionary<string, GameObject> DynamicObjectDatabase => AssetManager.ObjectDatabase;
        internal static GameObject Create_FxRoot(string name) 
        {
            GameObject root = new GameObject
            {
                name = name
            };
            root.SetActive(false);
            return root;
        }
        internal static GameObject Set_FxRoot(Pawn p) 
        {
            GameObject Fx_Root = new GameObject("Fx_Root");
            Fx_Root.AddComponent<UniqueID_Thing>().UID = p;
            return Fx_Root;
        }
        internal static void Set_FxHead(HeadPartsDef headDef, Pawn p) 
        {
            if (headDef == null) return;
            if (headDef.head == null) return;
            if (headDef.version == "3.8")
            {
                if (headDef.rotation == Rot4.South)
                {
                    SpineAssetPack head_Pack = AssetManager.spine38_Database.TryGetValue(headDef.head.defName);
                    SpineAssetPack frontHair_Pack = AssetManager.spine38_Database.TryGetValue(headDef.frontHair.defName);
                    SpineAssetPack backHair_Pack = AssetManager.spine38_Database.TryGetValue(headDef.backHair.defName);
                    SpineAssetPack eyeBow_Pack = AssetManager.spine38_Database.TryGetValue(headDef.eyeBow.defName);
                    SpineAssetPack leftEye_Pack = AssetManager.spine38_Database.TryGetValue(headDef.leftEye.defName);
                    SpineAssetPack rightEye_Pack = AssetManager.spine38_Database.TryGetValue(headDef.rightEye.defName);
                    SpineAssetPack mouth_Pack = AssetManager.spine38_Database.TryGetValue(headDef.mouth.defName);
                    //
                    GameObject Fx_Root = Set_FxRoot(p);
                    GameObject head = AssetExtensions.Create_AnimationInstance(head_Pack);
                    head.transform.SetParent(Fx_Root.transform);
                    //
                    GameObject frontHair = AssetExtensions.Create_AnimationInstance(frontHair_Pack);
                    frontHair.transform.SetParent(head.transform);
                    GameObject backHair = AssetExtensions.Create_AnimationInstance(backHair_Pack);
                    backHair.transform.SetParent(head.transform);
                    GameObject eyeBow = AssetExtensions.Create_AnimationInstance(eyeBow_Pack);
                    eyeBow.transform.SetParent(head.transform);
                    GameObject leftEye = AssetExtensions.Create_AnimationInstance(leftEye_Pack);
                    leftEye.transform.SetParent(head.transform);
                    GameObject rightEye = AssetExtensions.Create_AnimationInstance(rightEye_Pack);
                    rightEye.transform.SetParent(head.transform);
                    GameObject mouth = AssetExtensions.Create_AnimationInstance(mouth_Pack);
                    mouth.transform.SetParent(head.transform);
                }
            }
        }
    }
}
