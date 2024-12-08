using Spine38;
using Spine38.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace SpriteEvo
{
    ///<summary>Spine3.8版本方法库</summary>
    public static class Spine38Lib
    {
        /*public static Animation38 GenAnimation38(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool hasCam = false, bool DontDestroyOnLoad = false) 
        {
            if (animationDef == null || animationDef.version != "3.8" || animationDef.mainAsset == null) return null;
            if (animationDef.attachments.NullOrEmpty())
            {
                SkeletonLoader loader = animationDef.mainAsset.TryGetSpineAsset();
                if (loader == null)
                {
                    Log.Error("PA.SpriteEvo" + animationDef.defName + " Main Asset Not Found");
                    return null;
                }
                if (loader.def.asset.version != "3.8")
                {
                    Log.Error("PA.SpriteEvo" + animationDef.defName + " Wrong AnimationDef Version");
                    return null;
                }
                SkeletonDataAsset skeleton = loader.Create_SkeletonDataAsset38();
                skeleton.name = animationDef.defName + "_SkeletonData.asset";
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                AnimationParams @params = GenAnimation.GetSkeletonParams(animationDef, loop);
                animation.InitAnimation(@params, layer, active, DontDestroyOnLoad);
                animation.ApplyColor(@params.color, @params.slotSettings);
                if (!hasCam)
                {
                    return new Animation38(animation);
                }
                else
                {
                    return new Animation38(animation, animationDef.props.uioffset, animationDef.props.uiDrawSize);
                }
            }
            else
            {
                Log.Error("暂不支持Spine3.8骨架合并");
            }
            return null;
        }*/
        public static GameObject CreateSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "3.8" || animationDef.mainAsset == null) return null;
            if (animationDef.attachments.NullOrEmpty())
            {
                SkeletonLoader loader = animationDef.mainAsset.TryGetSpineAsset();
                if (loader == null)
                {
                    Log.Error("SpriteEvo." + animationDef.defName + " Main Asset Not Found");
                    return null;
                }
                if (loader.def.asset.version != "3.8")
                {
                    Log.Error("SpriteEvo." + animationDef.defName + " Wrong AnimationDef Version");
                    return null;
                }
                SkeletonDataAsset skeleton = loader.Create_SkeletonDataAsset38();
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
            else
            {
                Log.Error("暂不支持Spine3.8骨架合并");
            }
            return null;
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
        /// <summary>
        /// 重新加载SkeletonData并重置骨架实例所有状态,包括AnimationState 
        /// <para>仅在复用单个SkeletonRenderer实例时再考虑使用<paramref name="newAsset"/>重新加载新的Skeleton</para>
        /// </summary>
        public static void ReloadSkeleton(this SkeletonAnimation instance, SkeletonDataAsset newAsset = null, string skin = "default")
        {
            instance.skeletonDataAsset = newAsset;
            instance.initialSkinName = skin;
            instance.Initialize(overwrite: true);
        }
        //通用，禁用渲染器反射
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
