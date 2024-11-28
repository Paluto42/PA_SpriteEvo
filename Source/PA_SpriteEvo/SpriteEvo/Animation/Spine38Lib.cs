using Spine38.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace SpriteEvo
{
    public static class Spine38Lib
    {
        public static GameObject CreateAnimationSafe(AnimationDef animationDef, AnimationParams @params, int layer = 2, bool DontDestroyOnLoad = false)
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
                SkeletonAnimation animation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
                //Initilize
                animation.InitAnimation(@params, layer, DontDestroyOnLoad);
                animation.gameObject.ApplyColor(@params.color, @params.slotSettings);
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
        public static void ApplyColor(this SkeletonAnimation instance, Color color, List<SlotSettings> slotSettings)
        {
            if (instance == null) return;
            ISkeletonComponent skeletonComponent = instance.gameObject.GetComponent<ISkeletonComponent>();
            if (skeletonComponent != null)
            {
                Spine38.Skeleton skeleton = skeletonComponent.Skeleton;
                SkeletonExtensions.SetColor(skeleton, color);
                foreach (SlotSettings s in slotSettings)
                {
                    Spine38.Slot slot = skeleton.FindSlot(s.slot);
                    slot?.SetColor(color);
                }
            }
        }
        public static void InitAnimation(this SkeletonAnimation instance, AnimationParams @params, int layer = 2 ,bool activeSelf = true, bool DontDestroyOnLoad = false)
        {
            if (instance == null) { return; }
            //Initilize
            Vector3 scale = new(@params.scale.x, @params.scale.y, 1f);
            instance.gameObject.name = "Spine_" + @params.name;
            instance.gameObject.layer = layer;
            instance.transform.rotation = Quaternion.Euler(@params.rotation);
            //animation.transform.position = pawn.DrawPos + Vector3.back + Vector3.up;
            instance.transform.localScale = scale;
            instance.timeScale = @params.timeScale;
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
        //通用，禁用渲染器反射
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
