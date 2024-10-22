using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    /*Root模型结构:
     *   Fx_Root 为根节点 是空物件，只负责更新坐标
     *   {
     *     Fx_Head 为第一层节点 是空物件 负责切换显示方位,使用HeadControllerComp组件控制下面的节点实现头部动画
     *     {
     *       South Head, North Head, East Head为第二层节点 是头部的SkeletonAnimation附件物体
     *       {
     *         //Attachments
     *         Front Hair, Back Hair, EyeBow, LeftEye, RightEye, Mouth等面部部件挂在下面,为第三层节点  
     *         
     *     Fx_Body
     *     {
     *     
     *     Fx_Extra
     *     {
     */
    public static class AnimationGenerator
    {
        public static Dictionary<string, SpineAssetPack> Spine38_DB => AssetManager.spine38_Database;
        public static Dictionary<string, SpineAssetPack> Spine41_DB => AssetManager.spine41_Database;
        private static Dictionary<string, GameObject> Object_DB => AssetManager.ObjectDatabase;

        public static void SetAnimation() 
        {
        }
        private static GameObject Create_FxRoot(Vector3 rotation, string name = null) 
        {
            GameObject root = new GameObject
            {
                name = "FX_Root" + name
            };
            root.transform.rotation = Quaternion.Euler(rotation);
            root.SetActive(false);
            return root;
        }
        private static void SetFxRootAtThing(this GameObject fx_root, Thing t) 
        {
            if (fx_root == null) return;
            FxRootComp rootcomp = fx_root.AddComponent<FxRootComp>();
            rootcomp.User = t;
        }
        private static void SetPawnFxHead(this GameObject fx_root, HeadPartsDef headDef, Thing t) 
        {
            if (!(t is Pawn p)) return;
            if (headDef == null) return;
            if (headDef.head == null) return;
            if (headDef.rotation == null) return;

            GameObject fx_head = fx_root.AddEmptyChild("Fx_Head");
            FxHeadComp headcomp = fx_head.AddComponent<FxHeadComp>();
            //Childrens
            GameObject south = fx_head.AddEmptyChild("South Head");
            GameObject north = fx_head.AddEmptyChild("North Head");
            GameObject east = fx_head.AddEmptyChild("East Head");
            //设置子物件
            headcomp.SetChildren(s: south, n: north, e: east);

            if (headDef.version == "3.8")
            {
                var db = Spine38_DB;

                south.SetSouthHead(db, headDef);
                north.SetNorthHead(db, headDef);
                east.SetEastHead(db, headDef);
                if (headDef.rotation == Rot4.South)
                {
                    fx_root.SetSouthHead(db, headDef);
                }
                else if (headDef.rotation == Rot4.North) 
                {
                    fx_root.SetNorthHead(db, headDef);
                }
            }
        }
        private static void AddFacialAttachment(this GameObject fx_root, string defName, Dictionary<string, SpineAssetPack> db) 
        {
            SpineAssetPack compPack = db.TryGetValue(defName);
            if (compPack == null) return;
            GameObject attachment = AssetExtensions.Create_AnimationInstance(compPack);
            attachment.transform.SetParent(fx_root.transform);
            attachment.transform.localPosition = Vector3.zero;
        }
        private static void SetSouthHead(this GameObject fx_root, Dictionary<string, SpineAssetPack> db, HeadPartsDef headDef, int layer = 0) 
        {
            SpineAssetPack head_Pack = db.TryGetValue(headDef.head.defName);
            //
            GameObject head = AssetExtensions.Create_AnimationInstance(head_Pack, false);
            head.transform.SetParent(fx_root.transform);
            if (headDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(headDef.frontHair.defName);
                GameObject frontHair = AssetExtensions.Create_AnimationInstance(frontHair_Pack, false);
                frontHair.GetComponent<MeshRenderer>().sortingOrder = layer;
                frontHair.transform.SetParent(head.transform);
            }
            if (headDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(headDef.backHair.defName);
                GameObject backHair = AssetExtensions.Create_AnimationInstance(backHair_Pack, false);
                backHair.transform.SetParent(head.transform);
            }
            if (headDef.eyeBow != null)
            {
                SpineAssetPack eyeBow_Pack = db.TryGetValue(headDef.eyeBow.defName);
                GameObject eyeBow = AssetExtensions.Create_AnimationInstance(eyeBow_Pack, false);
                eyeBow.transform.SetParent(head.transform);
            }
            if (headDef.leftEye != null)
            {
                SpineAssetPack leftEye_Pack = db.TryGetValue(headDef.leftEye.defName);
                GameObject leftEye = AssetExtensions.Create_AnimationInstance(leftEye_Pack, false);
                leftEye.transform.SetParent(head.transform);
            }
            if (headDef.rightEye != null)
            {
                SpineAssetPack rightEye_Pack = db.TryGetValue(headDef.rightEye.defName);
                GameObject rightEye = AssetExtensions.Create_AnimationInstance(rightEye_Pack, false);
                rightEye.transform.SetParent(head.transform);
            }
            if (headDef.mouth != null)
            {
                SpineAssetPack mouth_Pack = db.TryGetValue(headDef.mouth.defName);
                GameObject mouth = AssetExtensions.Create_AnimationInstance(mouth_Pack, false);
                mouth.transform.SetParent(head.transform);
            }
        }
        private static void SetEastHead(this GameObject fx_root, Dictionary<string, SpineAssetPack> db, HeadPartsDef headDef)
        {

        }
        private static void SetNorthHead(this GameObject fx_root, Dictionary<string, SpineAssetPack> db, HeadPartsDef headDef)
        {
            SpineAssetPack head_Pack = db.TryGetValue(headDef.head.defName);
            GameObject head = AssetExtensions.Create_AnimationInstance(head_Pack, false);
            head.transform.SetParent(fx_root.transform);
            if (headDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(headDef.frontHair.defName);
                GameObject frontHair = AssetExtensions.Create_AnimationInstance(frontHair_Pack, false);
                frontHair.transform.SetParent(head.transform);
            }
            if (headDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(headDef.backHair.defName);
                GameObject backHair = AssetExtensions.Create_AnimationInstance(backHair_Pack, false);
                backHair.transform.SetParent(head.transform);
            }
        }
    }
}
