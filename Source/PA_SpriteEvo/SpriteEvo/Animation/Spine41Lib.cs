using Spine41.Unity;
using SpriteEvo.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace SpriteEvo
{
    //为了省略不同版本的Spine类又臭又长的前缀 进行了分版本控制
    //返回NULL记得报个错不然只有空引用异常抛出啥也不知道
    public static class Spine41Lib
    {
        public static GameObject CreateAnimationSafe(AnimationDef animationDef, AnimationParams @params, int layer = 2, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.1" || animationDef.mainAsset == null) return null;
            //单个动画
            if (animationDef.attachments.NullOrEmpty())
            {
                SkeletonLoader loader = animationDef.mainAsset.TryGetSpineAsset();
                if (loader == null)
                {
                    Log.Error("PA.SpriteEvo" + animationDef.defName + " Main Asset Not Found");
                    return null;
                }
                if (loader.def.asset.version != "4.1")
                {
                    Log.Error("PA.SpriteEvo" + animationDef.defName + " Wrong AnimationDef Version");
                    return null;
                }
                SkeletonDataAsset skeleton = loader.Create_SkeletonDataAsset41();
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.InitAnimation(@params, layer, DontDestroyOnLoad);
                animation.gameObject.ApplyColor(@params.color, @params.slotSettings);
                return animation.gameObject;
            }
            //合并的动画
            else
            {
                SpineTexAsset parent = animationDef.mainAsset.FindSpineTexAsset();
                if (parent == null)
                {
                    Log.Error("PA.SpriteEvo" + animationDef.defName + " Main Asset Not Found.");
                    return null;
                }
                SpineTexAsset[] attachments = new SpineTexAsset[animationDef.attachments.Count];
                for (int i = 0; i < attachments.Length; i++)
                {
                    attachments[i] = animationDef.attachments[i].FindSpineTexAsset();
                    if (attachments[i] == null) 
                    {
                        Log.Error("PA.SpriteEvo" + animationDef.defName + "Failed Applying Attachments.");
                        return null;
                    }
                }
                SkeletonDataAsset skeleton = JsonMerger.MergeSkeletonFromJSONs(parent, attachments);
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.InitAnimation(@params, layer, DontDestroyOnLoad);
                animation.gameObject.ApplyColor(@params.color, @params.slotSettings);
                return animation.gameObject;
            }
        }
        /// <summary>
        /// 在实例层面对动画进行颜色修改，可以多次覆盖
        /// </summary>
        public static void ApplyColor(this GameObject instance, Color color, List<SlotSettings> slotSettings)
        {
            if (instance == null) return;
            ISkeletonComponent skeletonComponent = instance.GetComponent<ISkeletonComponent>();
            if (skeletonComponent != null)
            {
                Spine41.Skeleton skeleton = skeletonComponent.Skeleton;
                SkeletonExtensions.SetColor(skeleton, color);
                foreach (SlotSettings s in slotSettings)
                {
                    Spine41.Slot slot = skeleton.FindSlot(s.slot);
                    slot?.SetColor(color);
                }
            }
        }
        //初始化动画
        public static void InitAnimation(this SkeletonAnimation instance, AnimationParams @params, int layer = 2, bool activeSelf = true, bool DontDestroyOnLoad = false)
        {
            if (instance == null) { return; }
            //Initilize
            Vector3 scale = new(@params.scale.x, @params.scale.y, 1f);
            instance.gameObject.name = "Spine_" + @params.name;
            instance.gameObject.layer = layer;
            instance.transform.rotation = Quaternion.Euler(@params.rotation);
            //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
            instance.transform.localScale = scale;
            instance.skeleton.SetSkin(@params.skin);
            //TrackEntry 
            instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
            instance.Initialize(overwrite: false);
            instance.gameObject.SetActive(value: activeSelf);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
            instance.transform.position = @params.position;
            DisableProbe(instance.gameObject);
        }
        //重新加载动画
        public static void ReloadAnimation(this SkeletonAnimation instance)
        {
        }
        public static void DisableProbe(GameObject obj)
        {
            var MeshRenderer = obj.GetComponent<MeshRenderer>();
            if (MeshRenderer != null)
            {
                MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }
        }

    }
}
