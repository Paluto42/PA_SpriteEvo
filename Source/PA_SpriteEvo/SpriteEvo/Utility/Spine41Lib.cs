using Spine41;
using Spine41.Unity;
using SpriteEvo.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace SpriteEvo
{
    ///<summary>Spine4.1版本方法库</summary>
    public static class Spine41Lib
    {
        public static GameObject NewSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.1" || animationDef.mainAsset == null) return null;
            SkeletonDataAsset skeletonDataAsset = null;
            //单个Skeleton
            if (animationDef.attachments.NullOrEmpty())
            {
                SkeletonLoader loader = animationDef.mainAsset.TryGetSpineAsset();
                if (loader == null)
                {
                    Log.Error("SpriteEvo." + animationDef.defName + " Main Asset Not Found");
                    return null;
                }
                if (loader.def.asset.version != "4.1")
                {
                    Log.Error("SpriteEvo." + animationDef.defName + " Wrong AnimationDef Version");
                    return null;
                }
                skeletonDataAsset = loader.SkeletonDataAsset41();
                if (skeletonDataAsset == null) return null;
                skeletonDataAsset.name = animationDef.defName + "_SkeletonData.asset";
            }
            //合并Skeleton
            else
            {
                SpineTexAsset parent = animationDef.mainAsset.FindSpineTexAsset();
                if (parent == null)
                {
                    Log.Error("SpriteEvo." + animationDef.defName + " Main Asset Not Found.");
                    return null;
                }
                SpineTexAsset[] attachments = new SpineTexAsset[animationDef.attachments.Count];
                for (int i = 0; i < attachments.Length; i++)
                {
                    attachments[i] = animationDef.attachments[i].FindSpineTexAsset();
                    if (attachments[i] == null) 
                    {
                        Log.Error("SpriteEvo." + animationDef.defName + "Failed Applying Attachments.");
                        return null;
                    }
                }
                skeletonDataAsset = JsonMerger.MergeSkeletonFromJSONs(parent, attachments);
                if (skeletonDataAsset == null) return null;
                skeletonDataAsset.name = animationDef.defName + "_SkeletonData.asset";
            }
            SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            animation.gameObject.name = animationDef.defName;
            animation.gameObject.layer = layer;
            animation.gameObject.SetActive(false);
            DisableProbe(animation.gameObject);
            AnimationParams @params = PA_Helper.GetSkeletonParams(animationDef, loop);
            InitializeTransform(animation.gameObject, @params);
            animation.Skeleton.SetSkin(@params.skin);
            animation.Skeleton.ApplyColor(@params.color, @params.slotSettings);
            InitializeAnimation(animation, @params);
            InitializeMonoBehaviour(animation.gameObject, animationDef.scriptProperties);
            animation.gameObject.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
            return animation.gameObject;
        }
        public static void InitializeMonoBehaviour(GameObject @object, List<CompatibleMonoBehaviourProperties> props)
        {
            if (props == null) return;
            foreach (var cmp in props)
            {
                if (cmp?.scriptClass == null) continue;
                if (typeof(CompatibleMonoBehaviour).IsAssignableFrom(cmp?.scriptClass))
                {
                    Component comp = @object.AddComponent(cmp.scriptClass);
                    if (comp is CompatibleMonoBehaviour cm)
                        cm.props = cmp;
                }
            }
        }
        /// <summary>
        /// 在实例层面对动画进行颜色修改，可以多次覆盖
        /// </summary>
        public static void ApplyColor(this Skeleton skeleton, Color color, List<SlotSettings> slotSettings)
        {
            if (skeleton == null) return;
            skeleton.SetColor(color);
            foreach (SlotSettings s in slotSettings)
            {
                Slot slot = skeleton.FindSlot(s.slot);
                slot?.SetColor(color);
            }
        }
        public static void InitializeTransform(this GameObject @object, AnimationParams @params)
        {
            if (@object == null) return;
            Vector3 scale = new(@params.scale.x, @params.scale.y, 1f);
            @object.transform.position = @params.position;
            @object.transform.rotation = Quaternion.Euler(@params.rotation);
            @object.transform.localScale = scale;
        }
        public static void InitializeAnimation(this SkeletonAnimation instance, AnimationParams @params)
        {
            if (instance == null) return;
            instance.loop = @params.loop;
            instance.timeScale = @params.timeScale;
            instance.AnimationName = @params.defaultAnimation;
            //instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
        }
        /// <summary>
        /// 对Spine运行时骨架实例更新Skin.会重置骨骼和Slots的材质以及丢失之前的动画状态
        /// <para>如果切换的两个skin之间骨骼是一样的 <paramref name="resetBones"/>应设为false</para>
        /// </summary>
        public static void UpdateSkin(this ISkeletonComponent animated, string newskin, bool resetBones = true)
        {
            if (animated.SkeletonDataAsset == null)
                return;
            animated.Skeleton.SetSkin(newskin);
            if (resetBones)
                animated.Skeleton.SetBonesToSetupPose();
            animated.Skeleton.SetSlotsToSetupPose();
        }
        //重新读取SkeletonData并重置骨架实例所有状态，包括AnimationState.如果替换了SkeletonDataAsset会重新加载SkeletonData.
        public static void ReloadSkeleton(this SkeletonAnimation instance, string skin = "default", SkeletonDataAsset newAsset = null)
        {
            if (newAsset != null && newAsset.IsLoaded)
            {
                instance.skeletonDataAsset = newAsset;
            }
            instance.initialSkinName = skin;
            instance.Initialize(overwrite: true);
        }
        public static void DisableProbe(GameObject obj)
        {
            MeshRenderer MeshRenderer = obj.GetComponent<MeshRenderer>();
            if (MeshRenderer == null) return;
            MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
            MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }
    }
}
