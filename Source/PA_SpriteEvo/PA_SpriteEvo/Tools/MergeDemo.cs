using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpriteEvo
{
    internal class MergeDemo
    {
        //示范Spine4.1版本的合并
        public void MergeSkeletonDemo(SpineAssetPack parent, SpineAssetPack child) 
        {
            TextAsset mergedAtlas = child.atlasInput.AppendToAtlasText(parent.atlasInput);
            Texture2D[] mergedTexs = child.textures.AppendToTextureArray(parent.textures);
            Spine41.Unity.SpineAtlasAsset atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(mergedAtlas, mergedTexs, parent.shader, initialize: true);
            //
            Spine41.Unity.SkeletonDataAsset skeleton = null;
            Spine41.Unity.SkeletonDataAsset psda = parent.Create_SkeletonDataAsset41();
            Spine41.Unity.SkeletonDataAsset csda = child.Create_SkeletonDataAsset41();
            SkeletonMerger merger = new SkeletonMerger(psda, csda);
            if (merger.SkeletonData != null) 
            {
                psda.InitializeWithData(merger.SkeletonData);
                skeleton = psda;
            }
            Spine41.Unity.SkeletonAnimation animation = Spine41.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(skeleton);
        }
    }
}
