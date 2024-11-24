using SpriteEvo.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace SpriteEvo.Unity
{
    /*Root模型结构:
     *   根节点: Fx_Root ，只负责更新坐标
     *   {
     *     //别问我为什么头和身体分开渲染，泰南也干了
     *     第一层节点: Fx_Head  负责切换旋转方向,使用HeadControllerComp组件控制下面的节点实现头部动画
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
    //由于Sorting Layer的鬼畜，不得不使用单独的Camera直接拿到贴图渲染。
    public static class AnimationGenerator
    {
        private static Dictionary<string, SpineAssetPack> Spine38_DB => AssetManager.spine38_Database;
        private static Dictionary<string, SpineAssetPack> Spine41_DB => AssetManager.spine41_Database;
        //private static Dictionary<Thing, GameObject> ThingObject_DB => AssetManager.ThingObjectDatabase;

        private static List<PawnKindSpriteDef> PawnKindDB => DefDatabase<PawnKindSpriteDef>.AllDefsListForReading;

        public static void MergeAnimation(Thing t, PawnKindSpriteDef test, string name = null)
        {
            GameObject obj = GC_GameObjectManager.TryGetRecord(t);
            if (obj != null) return;
            Vector3 rot = new(90f, 0f, 0f);
            GameObject baseroot = Create_FxRootBase(rot, name);
            baseroot.SetFxRootAtThing(t);
            if (t is Pawn p)
            {
                if (test == null) return;
                //
                var packhead = test.head.south.parent.FindSpineAssetPack();
                //var packhair = test.head.south.attachments[0].attachment.FindSpineAssetPack();
                //var newSkeleton = JsonMerger.MergeSkeletonFromJSON(packhead, packhair);
                /*for (int i = 1; i < test.head.south.attachments.Count; i++)
                {
                }*/
                List<SpineAssetPack> packs = new List<SpineAssetPack>();
                foreach (var item in test.head.south.attachments)
                {
                    packs.Add(item.attachment.FindSpineAssetPack());
                }
                var newSkeleton = JsonMerger.MergeSkeletonFromJSONs(packhead, packs);

                var animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(newSkeleton);
                //Initilize
                animation.gameObject.name = "Spine_" + packhead.def.defName;
                animation.gameObject.layer = 2;
                animation.transform.rotation = Quaternion.Euler(packhead.def.rotation);
                //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
                animation.transform.localScale = new Vector3(packhead.def.scale.x * 0.1f, packhead.def.scale.y * 0.1f, 1f);
                //newObject.skeleton.SetSkin();
                //TrackEntry 
                animation.AnimationState.SetAnimation(0, packhead.def.props.idleAnimationName, true);
                animation.Initialize(overwrite: false);
                animation.gameObject.SetActive(value: true);
                animation.gameObject.SetParentSafely(baseroot);
            }
            GC_GameObjectManager.Add(t, baseroot);
        }
        public static void CreatePawnAnimationModel(Thing t, PawnKindSpriteDef test, string name = null) 
        {
            GameObject obj = GC_GameObjectManager.TryGetRecord(t);
            //GameObject obj = ThingObject_DB.TryGetValue(t);
            if (obj != null) return;
            Vector3 rot = new Vector3(90f, 0f, 0f);
            GameObject baseroot = Create_FxRootBase(rot, name);
            baseroot.SetFxRootAtThing(t);
            if (t is Pawn p)
            {
                if (test == null) return;
                string version = test.version;
                GameObject fxhead = baseroot.AddFxHead();

                test.head.south?.SetHeadRotateForFxhead(fxhead, Rot4.South, version);

                test.head.east?.SetHeadRotateForFxhead(fxhead, Rot4.East, version);

                test.head.north?.SetHeadRotateForFxhead(fxhead, Rot4.North, version);

                test.head.west?.SetHeadRotateForFxhead(fxhead, Rot4.West, version);

                GameObject fxbody = baseroot.AddFxBody();
                test.body.south?.SetBodyRotateForFxBody(fxbody, Rot4.South, version);

                test.body.east?.SetBodyRotateForFxBody(fxbody, Rot4.East, version);

                test.body.north?.SetBodyRotateForFxBody(fxbody, Rot4.North, version);

                test.body.west?.SetBodyRotateForFxBody(fxbody, Rot4.West, version);
            }
            GC_GameObjectManager.Add(t, baseroot);
            //ThingObject_DB.Add(t, baseroot);
        }
        private static GameObject Create_FxRootBase(Vector3 rotation, string name = null) 
        {
            GameObject root = new GameObject
            {
                name = name + "FXRoot"
            };
            root.transform.rotation = Quaternion.Euler(rotation);
            root.SetActive(false);
            return root;
        }
        private static void SetFxRootAtThing(this GameObject fx_root, Thing t) 
        {
            if (fx_root == null || t == null) return;
            //设置子物件
            FxRootWorker rootcomp = fx_root.AddComponent<FxRootWorker>();
            rootcomp.User = t;
            return;
        }
        private static GameObject AddFxHead(this GameObject fx_root) 
        {
            if (fx_root == null) return null;
            if (fx_root.GetComponent<FxRootWorker>()?.User is Pawn) 
            {
                GameObject fxhead = fx_root.AddEmptyChild("FxHead");
                FxHeadWorker headcontroller = fxhead.AddComponent<FxHeadWorker>();
                fx_root.GetComponent<FxRootWorker>().FxHeadController = headcontroller;
                return fxhead;
            }
            return null;
        }
        private static GameObject AddFxBody(this GameObject fx_root)
        {
            if (fx_root == null) return null;
            if (fx_root.GetComponent<FxRootWorker>()?.User is Pawn)
            {
                GameObject fxbody = fx_root.AddEmptyChild("FxBody");
                FxBodyWorker bodycontroller = fxbody.AddComponent<FxBodyWorker>();
                fx_root.GetComponent<FxRootWorker>().FxBodyController = bodycontroller;
                return fxbody;
            }
            return null;
        }
        private static GameObject AddAttachment(this GameObject parent, SpineAssetPack pack, int layer = 0, bool loop = false) 
        {
            if (parent == null) return null;
            if (pack == null) return null;
            GameObject attachment = pack.CreateAnimationInstance(Isloop: loop);
            attachment.GetComponent<MeshRenderer>().sortingOrder = layer;
            attachment.SetParentSafely(parent);
            return attachment;
        }
        private static void RecordFacialAttachmentToComp(this GameObject fx, FacialControllWorker fcc, AttachmentTag tag) 
        {
            int s = (int)tag;
            switch (s) 
            {
                case 1:
                    fcc.FrontHair = fx;
                    break;
                case 2:
                    fcc.BackHair = fx;
                    break;
                case 3:
                    fcc.Eyebow = fx;
                    break;
                case 4:
                    fcc.LeftEye = fx;
                    break;
                case 5:
                    fcc.RightEye = fx;
                    break;
                case 6:
                    fcc.Mouth = fx;
                    break;
                default:
                    break;
            }
        }
        private static void RecordBodyAttachmentToComp(this GameObject fx, BodyControllWorker bcc, AttachmentTag tag)
        {
            int s = (int)tag;
            switch (s)
            {
                case 7:
                    bcc.FrontClothes = fx;
                    break;
                case 8:
                    bcc.BackClothes = fx;
                    break;
                default:
                    break;
            }
        }
        private static GameObject SetHeadRotateForFxhead(this ParentWithAttachment pwa, GameObject fxhead, Rot4 rot, string version = "4.1", int headlayer = 1)
        {
            if (pwa.parent == null || fxhead == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(pwa.parent.defName);
            if (head_pack == null)
            {
                Log.Error("southheadDef.head找不到!");
                return null;
            }
            GameObject headrotate = head_pack?.CreateAnimationInstance(Isloop: false);
            headrotate.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            headrotate.SetParentSafely(fxhead);
            FacialControllWorker fcc = headrotate.AddComponent<FacialControllWorker>();
            fcc.rot = rot;
            switch (rot.AsInt)
            {
                case 0:
                    fxhead.GetComponent<FxHeadWorker>().NorthChild = headrotate;
                    break;
                case 1:
                    fxhead.GetComponent<FxHeadWorker>().EastChild = headrotate;
                    break;
                case 2:
                    fxhead.GetComponent<FxHeadWorker>().SouthChild = headrotate;
                    break;
                case 3:
                    fxhead.GetComponent<FxHeadWorker>().WestChild = headrotate;
                    break;
                default:
                    break;
            }
            foreach (Attachment a in pwa.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = headrotate.AddAttachment(pack, layer: a.layer);
                    attach.RecordFacialAttachmentToComp(fcc, a.tag);
                }
            }
            return headrotate;
        }
        /*private static GameObject SetSouthHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1) 
        {
            if (fx_head == null) return null;
            var southheadDef = def.head.south;
            if (southheadDef == null || southheadDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(southheadDef.parent.defName);
            if (head_pack == null) 
            {
                Log.Error("southheadDef.head找不到!");
                return null;
            }
            GameObject south_head = head_pack?.CreateAnimationInstance(Isloop: false);
            south_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            south_head.SetParentSafely(fx_head);
            FacialControllerComp fcc = south_head.AddComponent<FacialControllerComp>();
            fcc.rot = Rot4.South;
            fx_head.GetComponent<FxHeadComp>().SetSouthChild(south_head);
            foreach (Attachment a in southheadDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = south_head.AddAttachment(pack, layer: a.layer);
                    attach.RecordFacialAttachmentToComp(fcc, a.tag);
                }
            }
            return south_head;
        }*/

        /*private static GameObject SetSouthBody(this GameObject fx_body, PawnKindSpriteDef def, int bodylayer = 1)
        {
            if (fx_body == null) return null;
            ParentWithAttachment southbodyDef = def.body.south;
            if (southbodyDef == null || southbodyDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack body_pack = db.TryGetValue(southbodyDef.parent.defName);
            if (body_pack == null)
            {
                Log.Error("southbodyDef.body找不到!");
                return null;
            }
            GameObject south_body = AssetExtensions.CreateAnimationInstance(body_pack, Isloop: false);
            south_body.GetComponent<MeshRenderer>().sortingOrder = bodylayer;
            south_body.SetParentSafely(fx_body);
            BodyControllerComp bcc = south_body.AddComponent<BodyControllerComp>();
            bcc.rot = Rot4.South;
            fx_body.GetComponent<FxBodyComp>().SetSouthChild(south_body);
            foreach (Attachment a in southbodyDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = south_body.AddAttachment(pack, layer: a.layer);
                    attach.RecordBodyAttachmentToComp(bcc, a.tag);
                }
            }
            return south_body;
        }*/

        private static GameObject SetBodyRotateForFxBody(this ParentWithAttachment pwa, GameObject fxbody, Rot4 rot, string version = "4.1", int bodylayer = 1)
        {
            if (pwa.parent == null || fxbody == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack body_pack = db.TryGetValue(pwa.parent.defName);
            if (body_pack == null)
            {
                Log.Error("southbodyDef.body找不到!");
                return null;
            }
            GameObject bodyrotate = AssetExtensions.CreateAnimationInstance(body_pack, Isloop: false);
            bodyrotate.GetComponent<MeshRenderer>().sortingOrder = bodylayer;
            bodyrotate.SetParentSafely(fxbody);
            BodyControllWorker bcc = bodyrotate.AddComponent<BodyControllWorker>();
            bcc.rot = rot;
            switch (rot.AsInt)
            {
                case 0:
                    fxbody.GetComponent<FxBodyWorker>().NorthChild = bodyrotate;
                    break;
                case 1:
                    fxbody.GetComponent<FxBodyWorker>().EastChild = bodyrotate;
                    break;
                case 2:
                    fxbody.GetComponent<FxBodyWorker>().SouthChild = bodyrotate;
                    break;
                case 3:
                    fxbody.GetComponent<FxBodyWorker>().WestChild = bodyrotate;
                    break;
                default:
                    break;
            }
            foreach (Attachment a in pwa.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = bodyrotate.AddAttachment(pack, layer: a.layer);
                    attach.RecordBodyAttachmentToComp(bcc, a.tag);
                }
            }
            return bodyrotate;
        }
        /*private static GameObject SetEastHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            var eastheadDef = def.head.east;
            if (eastheadDef == null || eastheadDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(eastheadDef.parent.defName);
            if (head_pack == null)
            {
                Log.Error("eastheadDef.head找不到!");
                return null;
            }
            GameObject east_head = head_pack.CreateAnimationInstance(Isloop: false);
            east_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            east_head.SetParentSafely(fx_head);
            FacialControllerComp fcc = east_head.AddComponent<FacialControllerComp>();
            fcc.rot = Rot4.East;
            fx_head.GetComponent<FxHeadComp>().SetEastChild(east_head);
            foreach (Attachment a in eastheadDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = east_head.AddAttachment(pack, layer: a.layer);
                    attach.RecordFacialAttachmentToComp(fcc, a.tag);
                }
            }
            return east_head;
        }*/
        /*private static GameObject SetEastBody(this GameObject fx_body, PawnKindSpriteDef def, int bodylayer = 1)
        {
            if (fx_body == null) return null;
            ParentWithAttachment eastbodyDef = def.body.east;
            if (eastbodyDef == null || eastbodyDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack body_pack = db.TryGetValue(eastbodyDef.parent.defName);
            if (body_pack == null)
            {
                Log.Error("southbodyDef.body找不到!");
                return null;
            }
            GameObject east_body = AssetExtensions.CreateAnimationInstance(body_pack, Isloop: false);
            east_body.GetComponent<MeshRenderer>().sortingOrder = bodylayer;
            east_body.SetParentSafely(fx_body);
            BodyControllerComp bcc = east_body.AddComponent<BodyControllerComp>();
            bcc.rot = Rot4.South;
            fx_body.GetComponent<FxBodyComp>().SetEastChild(east_body);
            foreach (Attachment a in eastbodyDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = east_body.AddAttachment(pack, layer: a.layer);
                    attach.RecordBodyAttachmentToComp(bcc, a.tag);
                }
            }
            return east_body;
        }*/
        /*private static GameObject SetNorthHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            var northheadDef = def.head.north;
            if (northheadDef == null || northheadDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(northheadDef.parent.defName);
            if (head_pack == null)
            {
                Log.Error("northheadDef.head找不到!");
                return null;
            }
            GameObject north_head = AssetExtensions.CreateAnimationInstance(head_pack, Isloop: false);
            north_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            north_head.SetParentSafely(fx_head);
            FacialControllerComp fcc = north_head.AddComponent<FacialControllerComp>();
            //fcc.rot = Rot4.North;
            fx_head.GetComponent<FxHeadComp>().SetNorthChild(north_head);
            foreach (Attachment a in northheadDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = north_head.AddAttachment(pack, layer: a.layer);
                    attach.RecordFacialAttachmentToComp(fcc, a.tag);
                }
            }
            return north_head;
        }*/
        /*private static GameObject SetNorthBody(this GameObject fx_body, PawnKindSpriteDef def, int bodylayer = 1)
        {
            if (fx_body == null) return null;
            ParentWithAttachment northbodyDef = def.body.north;
            if (northbodyDef == null || northbodyDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack body_pack = db.TryGetValue(northbodyDef.parent.defName);
            if (body_pack == null)
            {
                Log.Error("southbodyDef.body找不到!");
                return null;
            }
            GameObject north_body = AssetExtensions.CreateAnimationInstance(body_pack, Isloop: false);
            north_body.GetComponent<MeshRenderer>().sortingOrder = bodylayer;
            north_body.SetParentSafely(fx_body);
            BodyControllerComp bcc = north_body.AddComponent<BodyControllerComp>();
            bcc.rot = Rot4.South;
            fx_body.GetComponent<FxBodyComp>().SetNorthChild(north_body);
            foreach (Attachment a in northbodyDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = north_body.AddAttachment(pack, layer: a.layer);
                    attach.RecordBodyAttachmentToComp(bcc, a.tag);
                }
            }
            return north_body;
        }*/
        /*private static GameObject SetWestHead(this GameObject fx_head, PawnKindSpriteDef def, int headlayer = 1)
        {
            if (fx_head == null) return null;
            var westheadDef = def.head.east;
            if (westheadDef == null || westheadDef.parent == null) return null;
            Dictionary<string, SpineAssetPack> db;
            if (def.version == "4.1") db = Spine41_DB;
            else db = Spine38_DB;
            SpineAssetPack head_pack = db.TryGetValue(westheadDef.parent.defName);
            if (head_pack == null)
            {
                Log.Error("northheadDef.head找不到!");
                return null;
            }
            GameObject west_head = head_pack.CreateAnimationInstance(Isloop: false);
            west_head.GetComponent<MeshRenderer>().sortingOrder = headlayer;
            west_head.SetParentSafely(fx_head);
            FacialControllerComp fcc = west_head.AddComponent<FacialControllerComp>();
            fcc.rot = Rot4.West;
            fx_head.GetComponent<FxHeadComp>().SetNorthChild(west_head);
            foreach (Attachment a in westheadDef.attachments)
            {
                if (a.attachment != null)
                {
                    SpineAssetPack pack = db.TryGetValue(a.attachment.defName);
                    GameObject attach = west_head.AddAttachment(pack, layer: a.layer);
                    attach.RecordFacialAttachmentToComp(fcc, a.tag);
                }
            }
            return west_head;
        }*/

    }
}
