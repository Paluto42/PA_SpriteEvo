using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    /*Root模型结构:
     *   Fx_Root 为根节点 是空物件，只负责更新坐标和切换旋转方向
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
        private static Dictionary<string, SpineAssetPack> Spine38_DB => AssetManager.spine38_Database;
        private static Dictionary<string, SpineAssetPack> Spine41_DB => AssetManager.spine41_Database;
        private static Dictionary<string, GameObject> Object_DB => AssetManager.ObjectDatabase;

        public static void SetAnimationModel(Thing t, string name = null) 
        {
            Vector3 rot = new Vector3(90f, 0f, 0f);
            GameObject baseroot = Create_FxRootBase(rot, name);
            GameObject fxroot = baseroot.SetFxRootAtThing(t);
            Dictionary<string, SpineAssetPack> db;
            /*if (t is Pawn p)
            {
                SpineAssetPack s = db.TryGetValue(apr.n0);
                fxroot.GetComponent<FxRootComp>().SouthChild.AddFxHead();
                fxroot.GetComponent<FxRootComp>().NorthChild.AddFxHead();
                fxroot.GetComponent<FxRootComp>().EastChild.AddFxHead();
            }*/
        }
        private static GameObject Create_FxRootBase(Vector3 rotation, string name = null) 
        {
            GameObject root = new GameObject
            {
                name = "FX_Root" + name
            };
            root.transform.rotation = Quaternion.Euler(rotation);
            root.SetActive(false);
            return root;
        }
        private static GameObject SetFxRootAtThing(this GameObject fx_root, Thing t) 
        {
            if (fx_root == null) return null;
            if (t == null) return fx_root;
            //设置子物件
            FxRootComp rootcomp = fx_root.AddComponent<FxRootComp>();
            rootcomp.SouthChild = fx_root.AddEmptyChild("South");
            rootcomp.NorthChild = fx_root.AddEmptyChild("North");
            rootcomp.EastChild = fx_root.AddEmptyChild("East");
            rootcomp.User = t;
            return fx_root;
        }
        private static GameObject AddFxHead(this GameObject rotchild, SpineAssetPack headpack) 
        {
            if (headpack == null) return null;
            GameObject fxhead = AssetExtensions.CreateAnimationInstance(headpack, false);
            fxhead.GetComponent<MeshRenderer>().sortingOrder = 1;
            fxhead.AddComponent<FxHeadComp>();
            fxhead.SetParentSafely(rotchild);
            return fxhead;
        }
        private static void AddFacialAttachment(this GameObject fxhead, SpineAssetPack pack, int layer = 0) 
        {
            if (pack == null) return;
            GameObject attachment = pack.CreateAnimationInstance(loop: false);
            attachment.GetComponent<MeshRenderer>().sortingOrder = layer;
            attachment.transform.SetParent(fxhead.transform);
            attachment.transform.localPosition = Vector3.zero;
            attachment.transform.localRotation = Quaternion.identity;
        }
        private static void SetSouthHead(this GameObject fx_head, SpineAssetPack pack, HeadPartsDef headDef) 
        {
            if (fx_head == null) return;
            if (pack == null) return;
            Dictionary<string, SpineAssetPack> db;
            if (headDef.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            if (headDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(headDef.frontHair.defName);
                fx_head.AddFacialAttachment(frontHair_Pack, layer: 2);
            }
            if (headDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(headDef.backHair.defName);
                fx_head.AddFacialAttachment(backHair_Pack, layer: 0);
            }
            if (headDef.eyeBow != null)
            {
                SpineAssetPack eyeBow_Pack = db.TryGetValue(headDef.eyeBow.defName);
                fx_head.AddFacialAttachment(eyeBow_Pack, layer: 3);
            }
            if (headDef.leftEye != null)
            {
                SpineAssetPack leftEye_Pack = db.TryGetValue(headDef.leftEye.defName);
                fx_head.AddFacialAttachment(leftEye_Pack, layer: 2);
            }
            if (headDef.rightEye != null)
            {
                SpineAssetPack rightEye_Pack = db.TryGetValue(headDef.rightEye.defName);
                fx_head.AddFacialAttachment(rightEye_Pack, layer: 2);
            }
            if (headDef.mouth != null)
            {
                SpineAssetPack mouth_Pack = db.TryGetValue(headDef.mouth.defName);
                fx_head.AddFacialAttachment(mouth_Pack, layer: 2);
            }
        }
        private static void SetEastHead(this GameObject fx_root, Dictionary<string, SpineAssetPack> db, HeadPartsDef headDef)
        {

        }
        private static void SetNorthHead(this GameObject fx_root, Dictionary<string, SpineAssetPack> db, HeadPartsDef headDef)
        {
            SpineAssetPack head_Pack = db.TryGetValue(headDef.head.defName);
            GameObject head = AssetExtensions.CreateAnimationInstance(head_Pack, false);
            head.transform.SetParent(fx_root.transform);
            if (headDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(headDef.frontHair.defName);
                GameObject frontHair = AssetExtensions.CreateAnimationInstance(frontHair_Pack, false);
                frontHair.transform.SetParent(head.transform);
            }
            if (headDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(headDef.backHair.defName);
                GameObject backHair = AssetExtensions.CreateAnimationInstance(backHair_Pack, false);
                backHair.transform.SetParent(head.transform);
            }
        }
    }
}
