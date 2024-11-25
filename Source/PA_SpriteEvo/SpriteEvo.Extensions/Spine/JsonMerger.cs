using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpriteEvo.Extensions
{
    ///<summary>对导出为JSON格式的Spine骨骼进行合并
    /// <para>强制要求使用SpineTexAsset类型的包.</para>
    /// </summary>
    public static class JsonMerger
    {
        //合并Atlas
        /*public static Spine41.Unity.SpineAtlasAsset MergeAtlas(SpineTexAsset parent, SpineTexAsset child) 
        {
            TextAsset mergedAtlas = child.atlasInput.AppendToAtlasText(parent.atlasInput);
            Texture2D[] mergedTexs = child.textures.AppendToTextureArray(parent.textures);
            return Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(mergedAtlas, mergedTexs, parent.shader, initialize: true);
        }*/
        //May be Null
        public static TextAsset AppendToAtlasText(this TextAsset child, TextAsset parent)
        {
            if (parent == null || child == null) return null;
            string origincontent = parent.text;
            string toappend = child.text;
            string result = origincontent + "\n\n" + toappend;
            TextAsset newatlas = new TextAsset(result)
            {
                name = parent.name
            };
            return newatlas;
        }
        //May be Null
        public static Texture2D[] AppendToTextureArray(this Texture2D[] child, Texture2D[] parent)
        {
            if (parent == null || child == null) return null;
            Texture2D[] mergedTexArray = new Texture2D[parent.Length + child.Length];
            for (int i = 0; i < parent.Length; i++)
            {
                mergedTexArray[i] = parent[i];
            }
            for (int i = 0; i < child.Length; i++)
            {
                mergedTexArray[parent.Length + i] = child[i];
            }
            return mergedTexArray;
        }
        public static Spine41.Unity.SpineAtlasAsset MergeAtlases(SpineTexAsset parent, SpineTexAsset[] childs)
        {
            TextAsset mergedAtlas = childs[0].atlasInput.AppendToAtlasText(parent.atlasInput);
            Texture2D[] mergedTexs = childs[0].textures.AppendToTextureArray(parent.textures);
            for (int i = 1; i < childs.Length; i++)
            {
                mergedAtlas = childs[i].atlasInput.AppendToAtlasText(mergedAtlas);
                mergedTexs = childs[i].textures.AppendToTextureArray(mergedTexs);
            }
            return Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(mergedAtlas, mergedTexs, parent.shader, initialize: true);
        }
        /*public static Spine41.Unity.SkeletonDataAsset MergeSkeletonFromJSON(SpineTexAsset parent, SpineTexAsset child)
        {
            //使用合并后的AtlasAsset创建新的SkeletonDataAsset实例
            Spine41.Unity.SpineAtlasAsset atlasAsset = MergeAtlas(parent, child);
            var newSkeletonDataAsset = CreateMergedRuntimeInstance(parent.skeletonInput, child.skeletonInput, atlasAsset, initialize: true);
            return newSkeletonDataAsset;
        }*/
        public static Spine41.Unity.SkeletonDataAsset MergeSkeletonFromJSONs(SpineTexAsset parent, SpineTexAsset[] childs)
        {
            //使用合并后的AtlasAsset创建新的SkeletonDataAsset实例
            Spine41.Unity.SpineAtlasAsset atlasAsset = MergeAtlases(parent, childs);
            List<TextAsset> childstextAssets = new List<TextAsset>();
            foreach (SpineTexAsset child in childs) 
            {
                childstextAssets.Add(child.skeletonInput);
            }
            var newSkeletonDataAsset = CreateMergedRuntimeInstance(parent.skeletonInput, childstextAssets, atlasAsset, initialize: true);
            return newSkeletonDataAsset;
        }
        private static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, List<TextAsset> skeletonDataFile2, Spine41.Unity.AtlasAssetBase atlasAsset, bool initialize, float scale = 0.01f)
        {
            return CreateMergedRuntimeInstance(skeletonDataFile, skeletonDataFile2, new[] { atlasAsset }, initialize, scale);
        }
        /*private static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, TextAsset skeletonDataFile2, Spine41.Unity.AtlasAssetBase atlasAsset, bool initialize, float scale = 0.01f)
        {
            return CreateMergedRuntimeInstance(skeletonDataFile, skeletonDataFile2, new[] { atlasAsset }, initialize, scale);
        }*/
        private static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, List<TextAsset> skeletonDataFile2, Spine41.Unity.AtlasAssetBase[] atlasAssets, bool initialize, float scale = 0.01f)
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

                List<string> texts = new List<string>();
                foreach (var asset in skeletonDataFile2)
                {
                    texts.Add(asset.text);
                }

                try
                {
                    if (hasBinaryExtension)
                        loadedSkeletonData = Spine41.Unity.SkeletonDataAsset.ReadSkeletonData(skeletonDataAsset.skeletonJSON.bytes, attachmentLoader, skeletonDataScale);
                    else
                        loadedSkeletonData = ReadSkeletonDatas(skeletonDataAsset.skeletonJSON.text, texts, attachmentLoader, skeletonDataScale);
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
        /*private static Spine41.Unity.SkeletonDataAsset CreateMergedRuntimeInstance(TextAsset skeletonDataFile, TextAsset skeletonDataFile2, Spine41.Unity.AtlasAssetBase[] atlasAssets, bool initialize, float scale = 0.01f) 
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
        }*/
        /*private static Spine41.SkeletonData ReadSkeletonData(string text1, string text2, Spine41.AttachmentLoader attachmentLoader, float scale)
        {
            StringReader input1 = new StringReader(text1);
            StringReader input2 = new StringReader(text2);
            SkeletonJsonMerger json1 = new SkeletonJsonMerger(attachmentLoader)
            {
                Scale = scale
            };
            return json1.ReadSkeletonDataToMerge(input1, input2);
        }*/
        private static Spine41.SkeletonData ReadSkeletonDatas(string text1, List<string> text2, Spine41.AttachmentLoader attachmentLoader, float scale)
        {
            StringReader input1 = new(text1);
            StringReader[] input2 = new StringReader[text2.Count];
            for (int i = 0; i < input2.Length; i++)
            {
                StringReader input = new(text2[i]);
                input2[i] = input;
            }
            SkeletonJsonMerger json1 = new(attachmentLoader)
            {
                Scale = scale
            };
            return json1.ReadSkeletonDatasToMerge(input1, input2);
        }

    }
}
