using System;
using System.Reflection;
using UnityEngine;

namespace SpriteEvo
{
    ///<summary>
    ///对Spine导出文件提供封装实例,保存已创建Material实例的Spine导出文件
    ///<para>一般情况下该Material使用的Shader为Spine/Shader</para>
    ///<para>对于PMA模式问题，请确认在导出前就在Unity中为Mat设置了Straight Alpha Texture选项</para>
    ///<para>了解详细信息,请查看<a cref="AssetLoader"></a>类</para>
    ///</summary>
    public class Loader_Mat : AssetLoader
    {
        public Material[] materials;
        public bool useStraightAlpha;

        public Loader_Mat(SpineAssetDef def, TextAsset atlas, TextAsset skeleton, Material[] mats, bool usePMA = false) : base(def, atlas, skeleton)
        {
            this.materials = mats;
            this.useStraightAlpha = usePMA;
        }

        protected override TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton>()
        {
            base.CheckTextAssets();
            base.CheckArray(materials);
            Type atlasType = typeof(TAtlas);
            Type skeletonType = typeof(TSkeleton);
            MethodInfo createAtlasMethod = atlasType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), typeof(Material[]), typeof(bool) }); ;
            MethodInfo createSkeletonMethod = skeletonType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), atlasType, typeof(bool), typeof(float) });

            var atlas = (TAtlas)createAtlasMethod.Invoke(null, new object[] { this.atlasInput, this.materials, true });
            var skeleton = (TSkeleton)createSkeletonMethod.Invoke(null, new object[] { this.skeletonInput, atlas, true, 0.01f });
            return skeleton;
        }
        protected override TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton, ITextureLoader>()
        {
            base.CheckTextAssets();
            base.CheckArray(materials);
            Type atlasType = typeof(TAtlas);
            Type skeletonType = typeof(TSkeleton);
            MethodInfo createAtlasMethod = atlasType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), typeof(Material[]), typeof(bool), typeof(Func<TAtlas, ITextureLoader>) }); ;
            MethodInfo createSkeletonMethod = skeletonType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), atlasType, typeof(bool), typeof(float) });

            var atlas = (TAtlas)createAtlasMethod.Invoke(null, new object[] { this.atlasInput, this.materials, true, null });
            var skeleton = (TSkeleton)createSkeletonMethod.Invoke(null, new object[] { this.skeletonInput, atlas, true, 0.01f });
            return skeleton;
        }
    }
}
