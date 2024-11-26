using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public abstract class SkeletonLoader
    {
        public SpineAssetDef def;
        public TextAsset atlasInput;
        public TextAsset skeletonInput;
        public SkeletonLoader(SpineAssetDef def, TextAsset atlas, TextAsset skeleton) 
        {
            this.def = def;
            this.atlasInput = atlas;
            this.skeletonInput = skeleton;
        }
        public virtual void AtlasErrorMessage() 
        {
            Log.Error(this.def.defName + " SpineAssetPack Missing Atlas");
        }
        public virtual void SkeletonErrorMessage() 
        {
            Log.Error(this.def.defName + " SpineAssetPack Missing Skeleton");
        }
        ///<summary>读取 SkeletonDataAsset (Spine3.8版本)</summary>
        public abstract Spine38.Unity.SkeletonDataAsset Create_SkeletonDataAsset38();
        ///<summary>读取 SkeletonDataAsset (Spine4.1版本)</summary>
        public abstract Spine41.Unity.SkeletonDataAsset Create_SkeletonDataAsset41();
    }
}
