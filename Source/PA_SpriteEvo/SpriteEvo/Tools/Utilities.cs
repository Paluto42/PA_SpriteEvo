using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpriteEvo
{
    /// <summary>用于生成初始化Spine动画提供的参数集
    /// <para>必须调用<a cref="SkeletonAnimationUtility.GetSkeletonParams(AnimationDef, bool)"></a>进行值的初始化</para>
    /// </summary>
    public struct AnimationParams
    {
        public Color32 color;
        public List<SlotSettings> slotSettings;
        public Vector3 offset;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string skin;
        public string defaultAnimation;
        public bool PremultipliedAlpha;
        public bool loop;
        public float timeScale;
    }

    public static class Utilities
    {
        /// <summary>从AnimationDef中读取动画属性作为参数结构体返回</summary>
        public static AnimationParams GetSkeletonParams(this AnimationDef def, bool loop = true)
        {
            AnimationParams @params = default;
            @params.color = def.props.color;
            @params.slotSettings = def.props.slotSettings;
            @params.skin = def.props.skin;
            @params.defaultAnimation = def.props.idleAnimation;
            @params.rotation = def.props.rotation;
            @params.scale = def.props.scale;
            @params.timeScale = def.props.timeScale;
            @params.position = def.props.position;
            @params.loop = loop;
            return @params;
        }

        public static void FixRenderQueueInternal<TAtlas, TSkeleton>(TSkeleton asset, int renderQueue = 3000) where TAtlas : ScriptableObject where TSkeleton : ScriptableObject
        {
            FieldInfo atlasAssetsInfo = typeof(TSkeleton).GetField("atlasAssets", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo primaryMaterialProperty = typeof(TAtlas).GetProperty("PrimaryMaterial");

            TAtlas[] atlasAssets = (TAtlas[])atlasAssetsInfo.GetValue(asset);
            foreach (TAtlas atlas in atlasAssets)
            {
                Material primaryMaterial = (Material)primaryMaterialProperty.GetValue(atlas);
                if (primaryMaterial != null)
                    primaryMaterial.renderQueue = renderQueue;
            }
        }

    }
}
