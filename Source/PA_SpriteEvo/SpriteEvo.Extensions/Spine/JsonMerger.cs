using System;
using System.IO;
using UnityEngine;

namespace SpriteEvo.Extensions
{
    public static class JsonMerger
    {
        public static Spine41.Unity.SkeletonDataAsset MergeSkeletonFromJSON(SpineAssetPack parent, SpineAssetPack child)
        {
            //合并AtlasAsset
            TextAsset mergedAtlas = child.atlasInput.AppendToAtlasText(parent.atlasInput);
            Texture2D[] mergedTexs = child.textures.AppendToTextureArray(parent.textures);
            Spine41.Unity.SpineAtlasAsset atlasAsset = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(mergedAtlas, mergedTexs, parent.shader, initialize: true);
            //使用合并后的AtlasAsset创建新的SkeletonDataAsset实例
            var newSkeletonDataAsset = CreateMergedRuntimeInstance(parent.skeletonInput, child.skeletonInput, atlasAsset, initialize: true);
            return newSkeletonDataAsset;
        }
        public static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, TextAsset skeletonDataFile2, Spine41.Unity.AtlasAssetBase atlasAsset, bool initialize, float scale = 0.01f)
        {
            return CreateMergedRuntimeInstance(skeletonDataFile, skeletonDataFile2, new[] { atlasAsset }, initialize, scale);
        }
        public static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, TextAsset skeletonDataFile2, Spine41.Unity.AtlasAssetBase[] atlasAssets, bool initialize, float scale = 0.01f) 
        {
            Spine41.Unity.SkeletonDataAsset skeletonDataAsset = ScriptableObject.CreateInstance<Spine41.Unity.SkeletonDataAsset>();
            skeletonDataAsset.Clear();
            skeletonDataAsset.skeletonJSON = skeletonDataFile;
            skeletonDataAsset.atlasAssets = atlasAssets;
            skeletonDataAsset.scale = scale;
            if (initialize) 
            {
                if (skeletonDataAsset.skeletonJSON == null)
                {
                    //if (!quiet)
                        //Debug.LogError("Skeleton JSON file not set for SkeletonData asset: " + skeletonDataAsset.name, skeletonDataAsset);
                    skeletonDataAsset.Clear();
                    return null;
                }
                if (skeletonDataAsset.skeletonDataInternal() != null)
                    return skeletonDataAsset;
                Spine41.AttachmentLoader attachmentLoader;
                float skeletonDataScale;
                Spine41.Atlas[] atlasArray = skeletonDataAsset.GetAtlasArray();

#if !SPINE_TK2D
                attachmentLoader = (atlasArray.Length == 0) ? (Spine41.AttachmentLoader)new Spine41.Unity.RegionlessAttachmentLoader() : (Spine41.AttachmentLoader)new Spine41.AtlasAttachmentLoader(atlasArray);
                skeletonDataScale = scale;
#else
			if (spriteCollection != null) {
				attachmentLoader = new Spine41.Unity.TK2D.SpriteCollectionAttachmentLoader(spriteCollection);
				skeletonDataScale = (1.0f / (spriteCollection.invOrthoSize * spriteCollection.halfTargetHeight) * scale);
			} else {
				if (atlasArray.Length == 0) {
					Reset();
					if (!quiet) Debug.LogError("Atlas not set for SkeletonData asset: " + name, this);
					return null;
				}
				attachmentLoader = new AtlasAttachmentLoader(atlasArray);
				skeletonDataScale = scale;
			}
#endif
                bool hasBinaryExtension = skeletonDataAsset.skeletonJSON.name.ToLower().Contains(".skel");
                Spine41.SkeletonData loadedSkeletonData = null;

                try
                {
                    if (hasBinaryExtension)
                        loadedSkeletonData = Spine41.Unity.SkeletonDataAsset.ReadSkeletonData(skeletonDataAsset.skeletonJSON.bytes, attachmentLoader, skeletonDataScale);
                    else
                        loadedSkeletonData = ReadSkeletonData(skeletonDataAsset.skeletonJSON.text, skeletonDataFile2.text, attachmentLoader, skeletonDataScale);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error merging skeleton JSON file for SkeletonData asset: " + skeletonDataAsset.name + "\n" + ex.Message + "\n" + ex.StackTrace, skeletonDataAsset.skeletonJSON);
                }
                if (loadedSkeletonData == null)
                    return null;

                if (skeletonDataAsset.skeletonDataModifiers != null)
                {
                    foreach (Spine41.Unity.SkeletonDataModifierAsset modifier in skeletonDataAsset.skeletonDataModifiers)
                    {
                        if (modifier != null && !(skeletonDataAsset.isUpgradingBlendModeMaterials && modifier is Spine41.Unity.BlendModeMaterialsAsset))
                        {
                            modifier.Apply(loadedSkeletonData);
                        }
                    }
                }
                if (!skeletonDataAsset.isUpgradingBlendModeMaterials)
                    skeletonDataAsset.blendModeMaterials.ApplyMaterials(loadedSkeletonData);

                skeletonDataAsset.InitializeWithData(loadedSkeletonData);

            }
            return skeletonDataAsset;
        }
        internal static Spine41.SkeletonData ReadSkeletonData(string text1, string text2, Spine41.AttachmentLoader attachmentLoader, float scale)
        {
            StringReader input1 = new StringReader(text1);
            StringReader input2 = new StringReader(text2);
            SkeletonJsonMerger json1 = new SkeletonJsonMerger(attachmentLoader)
            {
                Scale = scale
            };
            return json1.ReadSkeletonDataToMerge(input1, input2);
        }

    }
}
