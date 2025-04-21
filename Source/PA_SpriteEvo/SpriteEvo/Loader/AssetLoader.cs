using System;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    /// <summary>
    /// 加载Spine导出文件数据并缓存为 .NET数据对象, 以便在Rimworld内使用.
    /// <para>
    /// 一套完整的Spine导出文件包括三者: 图集信息(.atlas), 骨骼数据(.json/.skel), 贴图文件(png).</para>
    /// <para>
    /// See <a href="http://esotericsoftware.com/spine-json-format">Spine JSON format</a> and
    /// <a href = "http://esotericsoftware.com/spine-loading-skeleton-data#JSON-and-binary-data" > JSON and binary data</a> in the Spine
    /// Runtimes Guide.</para>
    /// </summary>
    public abstract class AssetLoader
    {
        public string version;
        public SpineAssetDef def;
        public TextAsset atlasInput;
        public TextAsset skeletonInput;
        public AssetLoader(SpineAssetDef def, TextAsset atlas, TextAsset skeleton)
        {
            this.version = def.asset.version;
            this.def = def;
            this.atlasInput = atlas;
            this.skeletonInput = skeleton;
        }

        //理论上是全套支持3.8以下和4.0以上版本的
        protected virtual TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton>() where TAtlas : ScriptableObject where TSkeleton : ScriptableObject
        {
            throw new NotImplementedException();
        }
        protected virtual TSkeleton CreateSkeletonDataAsset<TAtlas, TSkeleton, ITextureLoader>() where TAtlas : ScriptableObject where TSkeleton : ScriptableObject
        {
            throw new NotImplementedException();
        }

        public Spine38.Unity.SkeletonDataAsset GetSkeletonDataAssetVer38()
        {
            return CreateSkeletonDataAsset<Spine38.Unity.SpineAtlasAsset, Spine38.Unity.SkeletonDataAsset>();
        }
        public Spine41.Unity.SkeletonDataAsset GetSkeletonDataAssetVer41()
        {
            return CreateSkeletonDataAsset<Spine41.Unity.SpineAtlasAsset, Spine41.Unity.SkeletonDataAsset, Spine41.TextureLoader>();
        }
        public Spine42.Unity.SkeletonDataAsset GetSkeletonDataAssetVer42()
        {
            return CreateSkeletonDataAsset<Spine42.Unity.SpineAtlasAsset, Spine42.Unity.SkeletonDataAsset, Spine42.TextureLoader>();
        }

        protected void CheckTextAssets()
        {
            if (this.atlasInput == null){
                throw new NullReferenceException("SpriteEvo. Atlas Not Found");
            }
            if (this.skeletonInput == null){
                throw new NullReferenceException("SpriteEvo. Skeleton Not Found");
            }
        }

        protected void CheckArray<T>(T[] array)
        {
            if (array.NullOrEmpty()){
                throw new NullReferenceException("SpriteEvo. Materials or Textures Not Found");
            }
        }
    }
}
