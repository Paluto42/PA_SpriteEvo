using Spine41;
using Spine41.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace SpriteEvo
{
    ///<summary>Spine4.1版本方法库</summary>
    public static class Spine41Lib
    {
        public static SkeletonDataAsset GetSkeletonDataFrom(AnimationDef animationDef)
        {
            AssetLoader loader = animationDef.mainAsset.Load<AssetLoader>() ??
                throw new NullReferenceException($"SpriteEvo. Main Asset Not Found In {animationDef.defName}");
            return loader.GetSkeletonDataAssetVer41();
        }

        //检查是否合并Skeleton 这个功能打算删了
        /*private static SkeletonDataAsset GetMergeSkeletonDataFrom(AnimationDef animationDef)
        {
            Asset_Tex parent = animationDef.mainAsset.TryGetAsset<Asset_Tex>();
            if (parent == null)
            {
                Log.Error($"SpriteEvo.{animationDef.defName} Main Asset Not Found.");
                return null;
            }
            Asset_Tex[] attachments = new Asset_Tex[animationDef.attachments.Count];
            for (int i = 0; i < attachments.Length; i++)
            {
                attachments[i] = animationDef.attachments[i].TryGetAsset<Asset_Tex>();
                if (attachments[i] == null)
                {
                    Log.Error($"SpriteEvo.{animationDef.defName} Failed Applying Attachments.");
                    return null;
                }
            }
            return JsonMerger.MergeSkeletonFromJSONs41(parent, attachments);
        }*/

        public static SkeletonDataAsset EnsureInitializedSkeletonData(AnimationDef animationDef) 
        {
            if (animationDef == null) return null;
            SkeletonDataAsset skeletonDataAsset;
            if (animationDef.attachments.NullOrEmpty()){
                skeletonDataAsset = GetSkeletonDataFrom(animationDef);
            }
            //合并Skeleton
            else{
                Log.Error("暂不支持Spine4.2骨架合并");
                skeletonDataAsset = null;
                //skeletonDataAsset = GetMergeSkeletonDataFrom(animationDef);
            }
            if (skeletonDataAsset == null) return null;
            skeletonDataAsset.name = animationDef.defName + "_SkeletonData.asset";
            return skeletonDataAsset;
        }

        public static GameObject NewSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.1" || animationDef.mainAsset == null) return null;

            AnimationParams @params = animationDef.GetSkeletonParams(loop);//获取def属性

            SkeletonDataAsset skeletonDataAsset = EnsureInitializedSkeletonData(animationDef);
            UnityExtension.FixMeshRenderQueue<AtlasAssetBase, SkeletonDataAsset>(skeletonDataAsset, animationDef.props.renderQueue);
            //单个Skeleton
            SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            GameObject baseObj = animation.gameObject;
            baseObj.SetActive(false);

            baseObj.name = animationDef.defName;
            //baseObj.layer = layer;
            baseObj.DisableProbe();//关闭反射器
            baseObj.SetTransform(@params.position, @params.rotation, @params.scale);

            animation.Skeleton.SetSkin(@params.skin);//设置默认皮肤
            animation.SetColor(@params.color, @params.slotSettings);//设置默认颜色
            animation.InitializeAnimation(@params.defaultAnimation, @params.timeScale, @params.loop);

            baseObj.AddScriptsFrom(animationDef.scripts);
            baseObj.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(baseObj);
            return baseObj;
        }

        public static GameObject NewSkeletonGraphic(AnimationDef animationDef, Material materialProperySource, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "4.1" || animationDef.mainAsset == null) return null;

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
            graphic.SetColor(@params.color, @params.slotSettings); //设置默认颜色
            graphic.InitializeAnimation(@params.defaultAnimation, @params.timeScale, @params.loop); //设置动画属性

            parentObj.AddScriptsFrom(animationDef.scripts);
            parentObj.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(baseObj.gameObject);
            return baseObj.gameObject;
        }

        /*public static Vector3 GetBonePositon(this ISkeletonComponent instance, Transform transform, string name)
        {
            Bone bone = instance.Skeleton.FindBone(name);
            return bone.GetWorldPosition(transform);
        }*/

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

        /// <summary>
        /// 在实例层面对动画进行颜色修改，可以多次覆盖
        /// </summary>
        public static void SetColor(this ISkeletonComponent instance, Color color, List<SlotSettings> slotSettings)
        {
            instance.Skeleton.SetColor(color);
            foreach (SlotSettings s in slotSettings)
            {
                Slot slot = instance.Skeleton.FindSlot(s.slot);
                slot?.SetColor(color);
            }
        }

        public static void SetTransparency(this ISkeletonComponent instance, float alpha = 0f)
        {
            instance.Skeleton.a = alpha;
        }

        //直接对着动画组件用就行
        public static TrackEntry SetAnimation(this IAnimationStateComponent instance, int trackIndex, string name, bool loop = true)
        {
            return instance.AnimationState.SetAnimation(trackIndex, name, loop);
        }

        /// <summary>
        /// 对Spine运行时骨架实例更新Skin.会重置骨骼和Slots的材质以及丢失之前的动画状态
        /// <para>如果切换的两个skin之间骨骼是一样的 <paramref name="forceResetBones"/>应设为false</para>
        /// </summary>
        public static void UpdateSkin(this ISkeletonComponent animated, string newskin, bool forceResetBones = true)
        {
            animated.Skeleton.SetSkin(newskin);
            if (forceResetBones)
                animated.Skeleton.SetBonesToSetupPose();
            animated.Skeleton.SetSlotsToSetupPose();
        }

        //重新读取SkeletonData并重置骨架实例所有状态，包括AnimationState.如果替换了SkeletonDataAsset会重新加载SkeletonData.
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
