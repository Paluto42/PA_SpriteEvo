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
        public static GameObject CreateSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.1" || animationDef.mainAsset == null) return null;
            //单个动画
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
                SkeletonDataAsset skeleton = loader.Create_SkeletonDataAsset41();
                skeleton.name = animationDef.defName + "_SkeletonData.asset";
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                animation.gameObject.SetActive(false);
                var cmp = animationDef.scriptProperties;
                if (cmp?.scriptClass != null)
                {
                    if (typeof(CompatibleMonoBehaviour).IsAssignableFrom(cmp?.scriptClass))
                    {
                        Component comp = animation.gameObject.AddComponent(cmp.scriptClass);
                        if (comp is CompatibleMonoBehaviour cm)
                            cm.props = cmp;
                    }
                }
                //Initilize
                AnimationParams @params = GenAnimation.GetSkeletonParams(animationDef, loop);
                animation.InitAnimation(@params, layer, active, DontDestroyOnLoad);
                animation.ApplyColor(@params.color, @params.slotSettings);
                return animation.gameObject;
            }
            //合并的动画
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
                SkeletonDataAsset skeleton = JsonMerger.MergeSkeletonFromJSONs(parent, attachments);
                skeleton.name = animationDef.defName + "_SkeletonData.asset";
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                AnimationParams @params = GenAnimation.GetSkeletonParams(animationDef, loop);
                animation.InitAnimation(@params, layer, active, DontDestroyOnLoad);
                animation.ApplyColor(@params.color, @params.slotSettings);
                return animation.gameObject;
            }
        }
        /// <summary>
        /// 在实例层面对动画进行颜色修改，可以多次覆盖
        /// </summary>
        public static void ApplyColor(this ISkeletonComponent instance, Color color, List<SlotSettings> slotSettings)
        {
            if (instance == null) return;
            ISkeletonComponent skeletonComponent = instance;
            if (skeletonComponent != null)
            {
                Skeleton skeleton = skeletonComponent.Skeleton;
                SkeletonExtensions.SetColor(skeleton, color);
                foreach (SlotSettings s in slotSettings)
                {
                    Slot slot = skeleton.FindSlot(s.slot);
                    slot?.SetColor(color);
                }
            }
        }
        //初始化动画
        public static void InitAnimation(this SkeletonAnimation instance, AnimationParams @params, int layer = 2, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (instance == null) { return; }
            //Initilize
            Vector3 scale = new(@params.scale.x, @params.scale.y, 1f);
            instance.gameObject.name = @params.name;
            instance.gameObject.layer = layer;
            instance.transform.rotation = Quaternion.Euler(@params.rotation);
            instance.transform.localScale = scale;
            instance.timeScale = @params.timeScale;
            instance.skeleton.SetSkin(@params.skin);
            //TrackEntry 
            instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
            instance.gameObject.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
            instance.transform.position = @params.position;
            DisableProbe(instance.gameObject);
        }
        /// <summary>
        /// 对Spine运行时骨架实例更新Skin.会重置骨骼和Slots的材质以及丢失之前的动画状态
        /// <para>如果切换的两个skin之间骨骼是一样的 <paramref name="resetBones"/>应设为false</para>
        /// </summary>
        public static void UpdateSkin(this SkeletonAnimation animated, string newskin, bool resetBones = true)
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
            if (MeshRenderer != null)
            {
                MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }
        }

    }
}
