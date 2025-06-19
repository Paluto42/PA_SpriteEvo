using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public enum AssetVersion
    {
        Spine38,
        Spine41,
        Spine42
    }
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
        public bool IsLoaded = false;
        //缓存
        private Spine38.Unity.SkeletonDataAsset _skeletonDataAsset38;
        private Spine41.Unity.SkeletonDataAsset _skeletonDataAsset41;
        private Spine42.Unity.SkeletonDataAsset _skeletonDataAsset42;
        //属性
        #region
        public string version;
        public bool useStraightAlpha;
        public SpineAssetDef def;
        public TextAsset atlasInput;
        public TextAsset skeletonInput;
        //默认设置
        public float defaultMix;
        public float defaultScale;
        public List<CustomMixSetting> customMixSettings;
        #endregion

        public AssetLoader(SpineAssetDef def, TextAsset atlas, TextAsset skeleton)
        {
            this.version = def.asset.version;
            this.def = def;
            this.atlasInput = atlas;
            this.skeletonInput = skeleton;
            this.defaultMix = def.asset.defaultMix;
            this.defaultScale = def.asset.defaultScale;
            this.customMixSettings = def.asset.customMixSettings;
        }

        //理论上是全套支持3.8以下和4.0以上版本的
        protected virtual TSkeleton CreateSkeletonDataAssetInternal<TAtlas, TSkeleton>() where TAtlas : ScriptableObject where TSkeleton : ScriptableObject
        {
            throw new NotImplementedException();
        }
        protected virtual TSkeleton CreateSkeletonDataAssetInternal<TAtlas, TSkeleton, ITextureLoader>() where TAtlas : ScriptableObject where TSkeleton : ScriptableObject
        {
            throw new NotImplementedException();
        }

        public Spine38.Unity.SkeletonDataAsset GetSkeletonDataAssetVer38()
        {
            _skeletonDataAsset38 ??= CreateSkeletonDataAssetInternal<Spine38.Unity.SpineAtlasAsset, Spine38.Unity.SkeletonDataAsset>();
            if (!IsLoaded && _skeletonDataAsset38 != null)
            {
                _skeletonDataAsset38.InitializeSkeletonData(defaultMix, defaultScale);
                _skeletonDataAsset38.SetCustomMix(customMixSettings);
                IsLoaded = true;
            }
            return _skeletonDataAsset38;
        }
        public Spine41.Unity.SkeletonDataAsset GetSkeletonDataAssetVer41()
        {
            _skeletonDataAsset41 ??= CreateSkeletonDataAssetInternal<Spine41.Unity.SpineAtlasAsset, Spine41.Unity.SkeletonDataAsset, Spine41.TextureLoader>();
            if (!IsLoaded && _skeletonDataAsset41 != null)
            {
                _skeletonDataAsset41.InitializeSkeletonData(defaultMix, defaultScale);
                IsLoaded = true;
            }
            return _skeletonDataAsset41;
        }
        public Spine42.Unity.SkeletonDataAsset GetSkeletonDataAssetVer42()
        {
            _skeletonDataAsset42 ??= CreateSkeletonDataAssetInternal<Spine42.Unity.SpineAtlasAsset, Spine42.Unity.SkeletonDataAsset, Spine42.TextureLoader>();
            if (!IsLoaded && _skeletonDataAsset42 != null)
            {
                _skeletonDataAsset42.InitializeSkeletonData(defaultMix, defaultScale);
                IsLoaded = true;
            }
            return _skeletonDataAsset42;
        }

        protected void CheckTextAssets()
        {
            if (this.atlasInput == null)
            {
                throw new NullReferenceException("SpriteEvo. Atlas Not Found");
            }
            if (this.skeletonInput == null)
            {
                throw new NullReferenceException("SpriteEvo. Skeleton Not Found");
            }
        }

        protected void CheckArray<T>(T[] array)
        {
            if (array.NullOrEmpty())
            {
                throw new NullReferenceException("SpriteEvo. Materials or Textures Not Found");
            }
        }
    }
}
