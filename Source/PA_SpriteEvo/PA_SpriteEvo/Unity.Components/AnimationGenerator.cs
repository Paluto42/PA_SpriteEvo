using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo.Unity
{
    /*Root模型结构:
     *   根节点: Fx_Root ，只负责更新坐标和切换旋转方向
     *   {
     *     //别问我为什么头和身体分开渲染，泰南也干了
     *     第一层节点: Fx_Head  负责切换显示方位,使用HeadControllerComp组件控制下面的节点实现头部动画
     *     {
     *       South Head, North Head, East Head为第二层节点 是头部的SkeletonAnimation附件物体
     *       {
     *         //Facial Attachments
     *         Front Hair, Back Hair, EyeBow, LeftEye, RightEye, Mouth等面部部件挂在下面,为第三层节点  
     *         
     *     第一层节点: Fx_Body
     *     {
     *     
     *     第一层节点: Fx_Extra
     *     {
     */
    public static class AnimationGenerator
    {
        private static Dictionary<string, SpineAssetPack> Spine38_DB => AssetManager.spine38_Database;
        private static Dictionary<string, SpineAssetPack> Spine41_DB => AssetManager.spine41_Database;
        private static Dictionary<Thing, GameObject> ThingObject_DB => AssetManager.ThingObjectDatabase;

        private static List<PawnKindSpriteDef> PawnKindDB => DefDatabase<PawnKindSpriteDef>.AllDefsListForReading;

        public static void CreatePawnAnimationModel(Thing t, PawnKindSpriteDef test, string name = null) 
        {
            GameObject obj = ThingObject_DB.TryGetValue(t);
            if (obj != null) return;
            Vector3 rot = new Vector3(90f, 0f, 0f);
            GameObject baseroot = Create_FxRootBase(rot, name);
            baseroot.SetFxRootAtThing(t);
            if (t is Pawn p)
            {
                if (test == null) return;
                GameObject fxhead = baseroot.AddFxHead();
                if (test.head.south != null)
                {
                    GameObject south_head = fxhead.SetSouthHead(test);
                }
                /*
                if (test.head.east != null)
                {
                    fxhead.SetEastHead(test);
                }
                if (test.head.north != null)
                {
                    fxhead.SetNorthHead(test);
                }
                if (test.head.west != null)
                {
                    fxhead.SetWestHead(test);
                }
                */
                //fxroot.AddFxBody();
            }
            ThingObject_DB.Add(t, baseroot);
        }
        private static GameObject Create_FxRootBase(Vector3 rotation, string name = null) 
        {
            GameObject root = new GameObject
            {
                name = "FXRoot" + name
            };
            root.transform.rotation = Quaternion.Euler(rotation);
            root.SetActive(false);
            return root;
        }
        private static void SetFxRootAtThing(this GameObject fx_root, Thing t) 
        {
            if (fx_root == null || t == null) return;
            //设置子物件
            FxRootComp rootcomp = fx_root.AddComponent<FxRootComp>();
            rootcomp.User = t;
            return;
        }
        private static GameObject AddFxHead(this GameObject fx_root) 
        {
            if (fx_root == null) return null;
            if (fx_root.GetComponent<FxRootComp>()?.User is Pawn) 
            {
                GameObject fxhead = fx_root.AddEmptyChild("FxHead");
                FxHeadComp headcontroller = fxhead.AddComponent<FxHeadComp>();
                fx_root.GetComponent<FxRootComp>().FxHeadController = headcontroller;
                return fxhead;
            }
            return null;
        }
        private static GameObject AddFxBody(this GameObject fx_root)
        {
            if (fx_root == null) return null;
            if (fx_root.GetComponent<FxRootComp>()?.User is Pawn)
            {
                GameObject fxbody = fx_root.AddEmptyChild("FxBody");
                fxbody.AddComponent<FxBodyComp>();
                return fxbody;
            }
            return null;
        }
        private static GameObject AddFacialAttachment(this GameObject head, SpineAssetPack pack, int layer = 0, bool loop = false) 
        {
            if (head == null) return null;
            if (pack == null) return null;
            GameObject attachment = pack.CreateAnimationInstance(loop);
            attachment.GetComponent<MeshRenderer>().sortingOrder = layer;
            attachment.SetParentSafely(head);
            return attachment;
        }
        private static GameObject SetSouthHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1) 
        {
            if (fx_head == null) return null;
            FacialParts southheadDef = def.head.south;
            if (southheadDef == null || southheadDef.head == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(southheadDef.head.defName);
            if (head_pack == null) 
            {
                Log.Error("southheadDef.head找不到!");
                return null;
            }
            GameObject south_head = head_pack?.CreateAnimationInstance(loop: false);
            south_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            south_head.SetParentSafely(fx_head);
            FacialControllerComp fcc = south_head.AddComponent<FacialControllerComp>();
            fx_head.GetComponent<FxHeadComp>().SouthChild = south_head;
            if (southheadDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(southheadDef.frontHair.defName);
                fcc.FrontHair = south_head.AddFacialAttachment(frontHair_Pack, layer: 2);
            }
            if (southheadDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(southheadDef.backHair.defName);
                fcc.BackHair = south_head.AddFacialAttachment(backHair_Pack, layer: 0);
            }
            if (southheadDef.eyeBow != null)
            {
                SpineAssetPack eyeBow_Pack = db.TryGetValue(southheadDef.eyeBow.defName);
                fcc.Eyebow = south_head.AddFacialAttachment(eyeBow_Pack, layer: 3);
            }
            if (southheadDef.leftEye != null)
            {
                SpineAssetPack leftEye_Pack = db.TryGetValue(southheadDef.leftEye.defName);
                fcc.LeftEye = south_head.AddFacialAttachment(leftEye_Pack, layer: 2, loop: true);
            }
            if (southheadDef.rightEye != null)
            {
                SpineAssetPack rightEye_Pack = db.TryGetValue(southheadDef.rightEye.defName);
                fcc.RightEye = south_head.AddFacialAttachment(rightEye_Pack, layer: 2, loop: true);
            }
            if (southheadDef.mouth != null)
            {
                SpineAssetPack mouth_Pack = db.TryGetValue(southheadDef.mouth.defName);
                fcc.Mouth = south_head.AddFacialAttachment(mouth_Pack, layer: 2);
            }
            return south_head;
        }
        private static GameObject SetEastHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            FacialParts eastheadDef = def.head.east;
            if (eastheadDef == null || eastheadDef.head == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(eastheadDef.head.defName);
            GameObject east_head = head_pack.CreateAnimationInstance(loop: false);
            east_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            east_head.SetParentSafely(fx_head);
            if (eastheadDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(eastheadDef.frontHair.defName);
                fx_head.AddFacialAttachment(frontHair_Pack, layer: 2);
            }
            if (eastheadDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(eastheadDef.backHair.defName);
                fx_head.AddFacialAttachment(backHair_Pack, layer: 0);
            }
            if (eastheadDef.eyeBow != null)
            {
                SpineAssetPack eyeBow_Pack = db.TryGetValue(eastheadDef.eyeBow.defName);
                fx_head.AddFacialAttachment(eyeBow_Pack, layer: 3);
            }
            if (eastheadDef.rightEye != null)
            {
                SpineAssetPack rightEye_Pack = db.TryGetValue(eastheadDef.rightEye.defName);
                fx_head.AddFacialAttachment(rightEye_Pack, layer: 2);
            }
            if (eastheadDef.mouth != null)
            {
                SpineAssetPack mouth_Pack = db.TryGetValue(eastheadDef.mouth.defName);
                fx_head.AddFacialAttachment(mouth_Pack, layer: 2);
            }
            return east_head;
        }
        private static GameObject SetNorthHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            FacialParts northheadDef = def.head.north;
            if (northheadDef == null || northheadDef.head == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_Pack = db.TryGetValue(northheadDef.head.defName);
            GameObject north_head = AssetExtensions.CreateAnimationInstance(head_Pack, false);
            north_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            north_head.SetParentSafely(fx_head);
            if (northheadDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(northheadDef.frontHair.defName);
                fx_head.AddFacialAttachment(frontHair_Pack, layer: 2);
            }
            if (northheadDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(northheadDef.backHair.defName);
                fx_head.AddFacialAttachment(backHair_Pack, layer: 0);
            }
            return north_head;
        }
        private static GameObject SetWestHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            FacialParts westheadDef = def.head.east;
            if (westheadDef == null || westheadDef.head == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(westheadDef.head.defName);
            GameObject west_head = head_pack.CreateAnimationInstance(loop: false);
            west_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            west_head.SetParentSafely(fx_head);
            if (westheadDef.frontHair != null)
            {
                SpineAssetPack frontHair_Pack = db.TryGetValue(westheadDef.frontHair.defName);
                fx_head.AddFacialAttachment(frontHair_Pack, layer: 2);
            }
            if (westheadDef.backHair != null)
            {
                SpineAssetPack backHair_Pack = db.TryGetValue(westheadDef.backHair.defName);
                fx_head.AddFacialAttachment(backHair_Pack, layer: 0);
            }
            if (westheadDef.eyeBow != null)
            {
                SpineAssetPack eyeBow_Pack = db.TryGetValue(westheadDef.eyeBow.defName);
                fx_head.AddFacialAttachment(eyeBow_Pack, layer: 3);
            }
            if (westheadDef.leftEye != null)
            {
                SpineAssetPack rightEye_Pack = db.TryGetValue(westheadDef.rightEye.defName);
                fx_head.AddFacialAttachment(rightEye_Pack, layer: 2);
            }
            if (westheadDef.mouth != null)
            {
                SpineAssetPack mouth_Pack = db.TryGetValue(westheadDef.mouth.defName);
                fx_head.AddFacialAttachment(mouth_Pack, layer: 2);
            }
            return west_head;
        }
    }
}
