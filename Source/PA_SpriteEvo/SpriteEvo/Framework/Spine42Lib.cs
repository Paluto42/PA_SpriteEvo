using Spine42;
using Spine42.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace SpriteEvo
{
    ///<summary>Spine4.2版本方法库</summary>
    public static class Spine42Lib
    {
        public static SkeletonDataAsset GetSkeletonDataFrom(AnimationDef animationDef)
        {
            AssetLoader loader = animationDef.mainAsset.Load<AssetLoader>() ??
                throw new NullReferenceException($"SpriteEvo. Main Asset Not Found In {animationDef.defName}");
            return loader.GetSkeletonDataAssetVer42();
        }

        //检查是否合并Skeleton 这个功能打算删了
        public static SkeletonDataAsset EnsureInitializedSkeletonData(AnimationDef animationDef)
        {
            if (animationDef == null) return null;
            SkeletonDataAsset skeletonDataAsset;
            if (animationDef.attachments.NullOrEmpty())
            {
                skeletonDataAsset = GetSkeletonDataFrom(animationDef);
            }
            else
            {
                Log.Error("暂不支持Spine4.2骨架合并");
                skeletonDataAsset = null;
            }
            if (skeletonDataAsset == null) return null;
            skeletonDataAsset.name = animationDef.defName + "_SkeletonData.asset";
            return skeletonDataAsset;
        }

        public static GameObject NewSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.2" || animationDef.mainAsset == null) return null;

            AnimationParams @params = animationDef.GetSkeletonParams(loop);//获取def属性

            SkeletonDataAsset skeletonDataAsset = EnsureInitializedSkeletonData(animationDef);
            //skeletonDataAsset.GetAnimationStateData().defaultMix = 0.177f;
            skeletonDataAsset.FixRenderQueueInternal(animationDef.props.renderQueue);
            //单个Skeleton
            SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            GameObject baseObj = animation.gameObject;
            baseObj.SetActive(false);

            baseObj.name = animationDef.defName;
            //baseObj.layer = layer;
            baseObj.DisableProbe();//关闭反射器
            baseObj.SetTransform(@params.position, @params.rotation, @params.scale);

            animation.Skeleton.SetSkin(@params.skin);//设置默认皮肤
            if (@params.skeletonColor != null)
            {
                animation.SetSkeletonColor(@params.skeletonColor.Value);
            }
            if (@params.slotSettings != null)
            {
                animation.SetSlotColor(@params.slotSettings);//设置默认颜色
            }
            baseObj.AddScriptsFrom(animationDef.scripts);
            baseObj.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(baseObj);
            return baseObj;
        }

        public static GameObject NewSkeletonGraphic(AnimationDef animationDef, Material materialProperySource, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.2" || animationDef.mainAsset == null) return null;

            AnimationParams @params = animationDef.GetSkeletonParams(loop);//获取def属性

            SkeletonDataAsset skeletonDataAsset = EnsureInitializedSkeletonData(animationDef);
            //单个Skeleton
            GameObject baseObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
            Canvas compCanvas = baseObj.GetComponent<Canvas>();
            compCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            SkeletonGraphic graphic = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, baseObj.transform, materialProperySource);
            GameObject parentObj = graphic.gameObject;
            parentObj.SetActive(false);

            parentObj.name = animationDef.defName;
            parentObj.layer = layer;
            parentObj.SetTransform(@params.position, @params.rotation, @params.scale); //设置RectTransform属性

            graphic.allowMultipleCanvasRenderers = true; //不开这个会导致多页材质的模型变成碎片
            graphic.Skeleton.SetSkin(@params.skin); //设置默认皮肤
            if (@params.skeletonColor != null)
            {
                graphic.SetSkeletonColor(@params.skeletonColor.Value);
            }
            if (@params.slotSettings != null)
            {
                graphic.SetSlotColor(@params.slotSettings);//设置默认颜色
            }
            graphic.InitializeAnimation(@params.defaultAnimation, @params.timeScale, @params.loop); //设置动画属性

            parentObj.AddScriptsFrom(animationDef.scripts);
            parentObj.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(baseObj.gameObject);
            return baseObj.gameObject;
        }

        public static void InitializeAnimation(this SkeletonAnimation instance, string defaultAnimation, float timeScale, bool loop = true)
        {
            instance.AnimationName = defaultAnimation;
            instance.loop = loop;
            instance.timeScale = timeScale;
            //instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
        }
        public static void InitializeAnimation(this SkeletonGraphic instance, string defaultAnimation, float timeScale, bool loop = true)
        {
            instance.startingAnimation = defaultAnimation;
            instance.startingLoop = loop;
            instance.timeScale = timeScale;
            //instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
        }

        public static void DoFlipX(this ISkeletonComponent instance, bool IsFlip)
        {
            float x = IsFlip ? -1f : 1f;
            instance.Skeleton.ScaleX = x;
        }

        public static void InitializeSkeletonData(this SkeletonDataAsset asset, float defaultMix, float defaultScale)
        {
            asset.GetAnimationStateData().DefaultMix = defaultMix;
            asset.scale = defaultScale;
        }

        public static void FixRenderQueueInternal(this SkeletonDataAsset asset, int renderQueue = 3000)
        {
            AtlasAssetBase[] atlasAssets = asset.atlasAssets;
            foreach (var atlasAsset in atlasAssets)
            {
                atlasAsset.PrimaryMaterial.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// 在运行时对动画实例进行颜色修改，可以多次覆盖
        /// </summary>
        public static void SetSkeletonColor(this ISkeletonComponent instance, Color32 color)
        {
            instance.Skeleton.SetColor(color);
        }
        public static void SetSlotColor(this ISkeletonComponent instance, List<SlotSetting> slotSettings)
        {
            foreach (SlotSetting s in slotSettings)
            {
                Slot slot = instance.Skeleton.FindSlot(s.slot);
                slot?.SetColor(s.color);
            }
        }

        public static void SetCustomMix(this SkeletonDataAsset skeletonDataAsset, List<CustomMixSetting> mixSettings)
        {
            if (mixSettings.Count == 0) return;
            AnimationStateData animationStateData = skeletonDataAsset.GetAnimationStateData();
            foreach (var mix in mixSettings)
            {
                animationStateData.SetMix(mix.fromAnimation, mix.toAnimation, mix.duration);
            }
        }

        /*public static TrackEntry AppendTrackRequest(this AnimationState animationState, int index, string animation, bool loop, float delay, bool reset = false)
        {
            return reset ? animationState.SetAnimation(index, animation, loop) : animationState.AddAnimation(index, animation, loop, delay);
        }*/

        /// <summary>
        /// 对Spine运行时骨架实例更新Skin.会重置骨骼和Slots的材质以及丢失之前的动画状态
        /// </summary>
        public static void UpdateSkin(this ISkeletonComponent instance, string newskin, bool forceResetBones = true)
        {
            instance.Skeleton.SetSkin(newskin);
            if (forceResetBones)
                instance.Skeleton.SetBonesToSetupPose();
            instance.Skeleton.SetSlotsToSetupPose();
        }
        /// <summary>
        /// 重新加载SkeletonData并重置骨架实例所有状态,包括AnimationState 
        /// <para>仅在复用单个SkeletonRenderer实例时再考虑使用<paramref name="newAsset"/>重新加载新的Skeleton</para>
        /// </summary>
        public static void ReloadSkeletonFrom(this SkeletonAnimation instance, string skin = "default", SkeletonDataAsset newAsset = null)
        {
            if (newAsset != null && newAsset.IsLoaded)
                instance.skeletonDataAsset = newAsset;
            instance.initialSkinName = skin;
            instance.Initialize(overwrite: true);
        }

        public static void ReloadSkeletonFrom(this SkeletonGraphic instance, string skin = "default", SkeletonDataAsset newAsset = null)
        {
            if (newAsset != null && newAsset.IsLoaded)
                instance.skeletonDataAsset = newAsset;
            instance.initialSkinName = skin;
            instance.Initialize(overwrite: true);
        }
    }
}
