using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public static class SkeletonGraphicUtility
    {
        public static Shader Spine_SkeletonGraphic => AssetLoadManager.Spine_SkeletonGraphic;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => AssetManager.GlobalObjectDatabase;

        public static Material EnsureInitializedMaterialProperySource(Shader shader, bool usePMA = false) 
        {
            if (shader == null) return null;
            Material material = new(shader);
            if (usePMA)
            {
                material.EnableKeyword("_STRAIGHT_ALPHA_INPUT");
                material.SetFloat("_StraightAlphaInput", 1);
            }
            return material;
        }

        ///<summary>[Pending] 在Canvas上渲染Spine动画 </summary>
        [Obsolete]
        public static GameObject Instantiate(AnimationDef def, Pawn pawn, bool loop = true)
        {
            /*
            Vector3 offset = new Vector3(def.props.offset.x, 0, pack.def.props.offset.y);
            Vector3 scale = new Vector3(pack.def.props.scale.x * 0.1f, 1f, pack.def.props.scale.y * 0.1f);
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Error("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject myGO = new GameObject
            {
                name = "myGO"
            };
            Canvas myCanvas = myGO.AddComponent<Canvas>();
            Transform parent = myCanvas.transform;
            parent.position += offset;
            parent.localScale = scale;

            Material SkeletonGraphic_alpha = Spine_SkeletonGraphic;
            if (!pack.useStraightAlpha)
            {
                SkeletonGraphic_alpha.SetFloat("_StraightAlphaInput", 0);
                SkeletonGraphic_alpha.DisableKeyword("_STRAIGHT_ALPHA_INPUT");
                Log.Message("_STRAIGHT_ALPHA_INPUT OFF");
            }
            Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
            Spine38.Unity.SkeletonGraphic graphic = Spine38.Unity.SkeletonGraphic.NewSkeletonGraphicGameObject(skeleton, parent, SkeletonGraphic_alpha);

            graphic.allowMultipleCanvasRenderers = true;
            graphic.gameObject.layer = 5;
            //graphic.rectTransform.position += offset;
            //graphic.rectTransform.localScale = scale;
            graphic.rectTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
            graphic.rectTransform.position = pawn.DrawPos + Vector3.up; ;
            graphic.AnimationState.SetAnimation(0, "Idle", loop);
            graphic.Initialize(overwrite: false);
            graphic.gameObject.SetActive(false);
            DynamicObjectDatabase.Add(pack.def.defName, graphic.gameObject);
            */
            return null;
        }
    }
}
