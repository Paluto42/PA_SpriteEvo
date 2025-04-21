using System;
using System.Reflection;
using UnityEngine;

namespace SpriteEvo
{
    ///<summary>
    ///对Spine导出文件提供封装实例,保存未创建Material实例的Spine导出文件
    ///<para>需要使用Spine/Shader创建Material实例后才可初始化SkeletonDataAsset</para>
    ///<para>对于PMA模式问题，请使用本框架提供的预设启用PMA的额外Spine/Skeleton Shader</para>
    ///<para>了解详细信息,请查看<a cref="AssetLoader"></a>类</para>
    ///</summary>
    public class Loader_Tex : AssetLoader
    {
        public Texture2D[] textures;
        public Shader shader;
        public bool useStraightAlpha;

        public Loader_Tex(SpineAssetDef def, TextAsset atlas, TextAsset skeleton, Texture2D[] texs, Shader shader, bool usePMA = false) : base(def, atlas, skeleton)
        {
            this.textures = texs;
            this.shader = shader;
            this.useStraightAlpha = usePMA;
        }

        public override TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton>()
        {
            base.CheckAssets();
            Type atlasType = typeof(TAtlas);
            Type skeletonType = typeof(TSkeleton);
            MethodInfo createAtlasMethod = atlasType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), typeof(Texture2D[]), typeof(Shader), typeof(bool), typeof(bool) });
            MethodInfo createSkeletonMethod = skeletonType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), atlasType, typeof(bool), typeof(float) });

            var atlas = (TAtlas)createAtlasMethod.Invoke(null, new object[] { this.atlasInput, this.textures, this.shader, true, useStraightAlpha });
            var skeleton = (TSkeleton)createSkeletonMethod.Invoke(null, new object[] { this.skeletonInput, atlas, true, 0.01f });
            return skeleton;
        }
        public override TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton, ITextureLoader>()
        {
            base.CheckAssets();
            Type atlasType = typeof(TAtlas);
            Type skeletonType = typeof(TSkeleton);
            MethodInfo createAtlasMethod = atlasType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), typeof(Texture2D[]), typeof(Shader), typeof(bool), typeof(Func<TAtlas, ITextureLoader>), typeof(bool) });
            MethodInfo createSkeletonMethod = skeletonType.GetMethod("CreateRuntimeInstance", new[] { typeof(TextAsset), atlasType, typeof(bool), typeof(float) });

            var atlas = (TAtlas)createAtlasMethod.Invoke(null, new object[] { this.atlasInput, this.textures, this.shader, true, null, useStraightAlpha });
            var skeleton = (TSkeleton)createSkeletonMethod.Invoke(null, new object[] { this.skeletonInput, atlas, true, 0.01f });
            return skeleton;
        }
    }
}
