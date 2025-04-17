using Spine38;
using Spine38.Unity;
using SpriteEvo.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
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
        private static SkeletonDataAsset GetSkeletonDataFrom(AnimationDef animationDef)
        {
            SkeletonLoader loader = animationDef.mainAsset.TryGetAsset<SkeletonLoader>();
            if (loader == null)
            {
                Log.Error($"SpriteEvo.{animationDef.defName} Main Asset Not Found");
                return null;
            }
            if (loader.def.asset.version != "3.8")
            {
                Log.Error($"SpriteEvo.{animationDef.defName} Wrong AnimationDef Version");
                return null;
            }
            return loader.SkeletonDataAsset38();
        }

        public static SkeletonDataAsset EnsureInitializedSkeletonData(AnimationDef animationDef)
        {
            if (animationDef == null) return null;
            SkeletonDataAsset skeletonDataAsset;
            if (animationDef.attachments.NullOrEmpty())
            {
                skeletonDataAsset = GetSkeletonDataFrom(animationDef);
            }
            //合并Skeleton 暂时不考虑做3.8的合并
            else
            {
                Log.Error("暂不支持Spine3.8骨架合并");
                skeletonDataAsset = null;
            }
            if (skeletonDataAsset == null) return null;
            skeletonDataAsset.name = animationDef.defName + "_SkeletonData.asset";
            return skeletonDataAsset;
        }

        public static GameObject NewSkeletonAnimation(AnimationDef animationDef, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "3.8" || animationDef.mainAsset == null) return null;

            SkeletonDataAsset skeletonDataAsset = EnsureInitializedSkeletonData(animationDef);
            //单个Skeleton
            SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            animation.gameObject.name = animationDef.defName;
            animation.gameObject.layer = layer;
            animation.gameObject.SetActive(false);
            DisableProbe(animation.gameObject);//关闭反射器
            AnimationParams @params = PA_Helper.GetSkeletonParams(animationDef, loop);//获取def属性

            InitializeTransform(animation.gameObject, @params);
            animation.Skeleton.SetSkin(@params.skin);//设置默认皮肤
            animation.Skeleton.ApplyColor(@params.color, @params.slotSettings);//设置默认颜色
            InitializeAnimation(animation, @params);
            InitializeMonoBehaviour(animation.gameObject, animationDef.scripts);
            animation.gameObject.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(animation.gameObject);
            return animation.gameObject;
        }

        public static GameObject NewSkeletonGraphic(AnimationDef animationDef, Material materialProperySource, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (animationDef == null || animationDef.version != "3.8" || animationDef.mainAsset == null) return null;

            SkeletonDataAsset skeletonDataAsset = EnsureInitializedSkeletonData(animationDef);
            //单个Skeleton
            GameObject canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
            Canvas compCanvas = canvas.GetComponent<Canvas>();
            compCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            SkeletonGraphic graphic = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, canvas.transform, materialProperySource);
            //不开这个会导致多页材质的模型变成碎片
            graphic.allowMultipleCanvasRenderers = true;
            graphic.gameObject.name = animationDef.defName;
            graphic.gameObject.layer = layer;
            graphic.gameObject.SetActive(false);
            AnimationParams @params = animationDef.GetSkeletonParams(loop);//获取def属性

            InitializeTransform(graphic.gameObject, @params);
            graphic.gameObject.transform.localPosition = Vector3.zero;
            graphic.Skeleton.SetSkin(@params.skin);//设置默认皮肤
            graphic.Skeleton.ApplyColor(@params.color, @params.slotSettings);//设置默认颜色
            InitializeGraphic(graphic, @params);
            InitializeMonoBehaviour(graphic.gameObject, animationDef.scripts);
            graphic.gameObject.SetActive(value: active);
            if (DontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(graphic.gameObject);
            return canvas.gameObject;
        }

        public static void InitializeMonoBehaviour(GameObject @object, List<ScriptProperties> props)
        {
            if (props == null) return;
            foreach (var cmp in props)
            {
                if (cmp?.scriptClass == null) continue;
                if (typeof(ScriptBase).IsAssignableFrom(cmp?.scriptClass))
                {
                    Component comp = @object.AddComponent(cmp.scriptClass);
                    if (comp is ScriptBase script)
                        script.props = cmp;
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
        public static void SetTransparency(this Skeleton skeleton, float alpha = 0f) 
        {
            if (skeleton == null) return;
            Color color = new(skeleton.r, skeleton.g, skeleton.b, alpha);
            skeleton.SetColor(color);
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
        public static void InitializeGraphic(this SkeletonGraphic instance, AnimationParams @params)
        {
            if (instance == null) return;
            instance.startingLoop = @params.loop;
            instance.timeScale = @params.timeScale;
            instance.startingAnimation = @params.defaultAnimation;
            //instance.AnimationState.SetAnimation(0, @params.defaultAnimation, @params.loop);
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
        public static void ReloadSkeleton(this SkeletonAnimation instance, string skin = "default", SkeletonDataAsset newAsset = null)
        {
            if (newAsset != null && newAsset.IsLoaded)
            {
                instance.skeletonDataAsset = newAsset;
            }
            instance.initialSkinName = skin;
            instance.Initialize(overwrite: true);
        }
        //通用，禁用渲染器反射
        public static void DisableProbe(GameObject obj)
        {
            MeshRenderer MeshRenderer = obj.GetComponent<MeshRenderer>();
            if (MeshRenderer == null) return;
            MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
            MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }
    }
}
