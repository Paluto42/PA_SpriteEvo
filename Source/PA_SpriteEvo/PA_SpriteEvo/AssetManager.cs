using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    [StaticConstructorOnStartup]
    internal static class AssetManager
    {
        public static string Spine_Dict = "Asset/";

        public static bool AllAssetsLoaded = false;

        public static bool AllShadersLoaded = false;
        //Animation的Spine/Skeleton shader && Graphic的Spine/SkeletonGraphic shader
        public static Shader Spine_Skeleton;

        public static Material SkeletonGraphic;

        public static Dictionary<string, Shader> spineShaderDatabase = new Dictionary<string, Shader>();

        public static Dictionary<string, GameObject> ObjectDatabase = new Dictionary<string, GameObject>();

        public static Dictionary<string, SpineAssetPack> spine38_Database = new Dictionary<string, SpineAssetPack>();

        public static Dictionary<string, SpineAssetPack> spine41_Database = new Dictionary<string, SpineAssetPack>();

        private static List<ModContentPack> Mods => LoadedModManager.RunningModsListForReading;

        static AssetManager() 
        {
            LoadAllSpineShader();
        }
        private static void LoadAllSpineShader() 
        {
            ModContentPack modbase = Mods.FirstOrDefault(mod => mod.PackageId == TypeDef.ModID.ToLower());
            string SpinePath = Path.Combine(modbase.RootDir, "Asset", "spine38");
            if (SpinePath == null)
            {
                Log.Error("PA.SpineFramework: Critical Error: Missing Spine Assets " + SpinePath);
            }
            AssetBundle ab = AssetBundle.LoadFromFile(SpinePath);
            Shader Skeleton = ab.LoadAsset<Shader>("Spine-Skeleton");
            if (Skeleton != null && Spine_Skeleton == null)
            {
                Spine_Skeleton = Skeleton;
                Log.Message("PA.SpineFramework: Spine/Skeleton.Shader Loaded");
            }
            Shader[] shaders = ab.LoadAllAssets<Shader>();
            foreach (Shader s in shaders)
            {
                spineShaderDatabase.Add(s.name, s);
            }
            if (spineShaderDatabase.NullOrEmpty())
            {
                AllShadersLoaded = false;
                return;
            }
            else
            {
                AllShadersLoaded = true;
            }
        }
        internal static void ResloveAllAssetBundle() 
        {
            if (!AllShadersLoaded)
            {
                return;
            }
            List<AssetBundle> AssetBundle_Loaded = new List<AssetBundle>();
            List<SpinePackDef> packs = DefDatabase<SpinePackDef>.AllDefsListForReading;
            foreach (SpinePackDef def in packs) 
            {
                TextAsset atlasAsset;
                TextAsset skeletonAsset;
                Material[] materials = null;
                Texture2D[] textures = null;
                Shader shader = null;
                string IndividualPath = Path.Combine(def.modContentPack.RootDir, Spine_Dict);
                //二进制读取
                if (def.skelFormat == SkelFormat.Binary)
                {
                    AssetBundle ab;
                    string assetBundleName = Path.GetFileName(def.props.assetBundle);
                    if (!AssetBundle_Loaded.Exists((AssetBundle a) => a.name == assetBundleName))
                    {
                        string abPath = Path.Combine(IndividualPath, def.props.assetBundle);
                        if (!File.Exists(abPath))
                        {
                            continue;
                        }
                        ab = AssetBundle.LoadFromFile(abPath);
                        AssetBundle_Loaded.Add(ab);
                        Log.Warning("PA.SpineFramework: Loading From Assetbundle : " + Spine_Dict + ab.name);
                    }
                    else
                    {
                        ab = AssetBundle_Loaded.First((AssetBundle a) => a.name == assetBundleName);
                    }
                    if (SkeletonGraphic == null)
                    {
                        Material SkeletonGraphicDefault = ab.LoadAsset<Material>("SkeletonGraphicDefault");
                        SkeletonGraphic = SkeletonGraphicDefault;
                    }
                    atlasAsset = ab?.LoadAsset<TextAsset>(def.props.atlas);
                    skeletonAsset = ab?.LoadAsset<TextAsset>(def.props.skeleton);
                    if (def.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = spineShaderDatabase.TryGetValue(def.props.shader);
                    }
                    List<string> texinfo = def.props.textures;
                    List<string> matinfo = def.props.materials;
                    if (!texinfo.Empty())
                    {
                        textures = new Texture2D[def.props.textures.Count];
                        for (int i = 0; i < def.props.textures.Count; i++)
                        {
                            textures[i] = ab?.LoadAsset<Texture2D>(def.props.textures[i]);
                        }
                        if (textures.NullOrEmpty())
                        {
                            Log.Error("PA.SpineFramework: " + def.defName + " Invaild Texture2D Array");
                            continue;
                        }
                        SpineAssetPack texPack = new SpineAssetPack(atlasAsset, skeletonAsset, textures, shader, def, useAlpha: def.props.StraightAlphaInput);
                        SavePackToVersionDatabase(def, texPack);
                        Log.Message("PA.SpineFramework: Successful Loaded " + def.defName + " with " + textures.Length + (textures.Length > 1 ? " Textures" : " Texture"));
                    }
                    else if (!matinfo.Empty())
                    {
                        materials = new Material[def.props.materials.Count];
                        for (int i = 0; i < def.props.materials.Count; i++)
                        {
                            materials[i] = ab?.LoadAsset<Material>(def.props.materials[i]);
                        }
                        if (materials.NullOrEmpty())
                        {
                            Log.Error("PA.SpineFramework: " + def.defName + " Invaild Material Array");
                            continue;
                        }
                        SpineAssetPack matPack = new SpineAssetPack(atlasAsset, skeletonAsset, materials, def, useAlpha: def.props.StraightAlphaInput);
                        SavePackToVersionDatabase(def, matPack);
                        Log.Message("PA.SpineFramework: Successful Loaded " + def.defName + " with " + materials.Length + (materials.Length > 1 ? " Materials" : " Material"));
                    }
                    else
                    {
                        Log.Warning("PA.SpineFramework: " + def.defName + " Missing Material or Texture Info");
                        continue;
                    }
                }
                //JSON读取
                else
                {
                    string JSONPath = Path.Combine(IndividualPath, def.props.folderPath);
                    if (!Directory.Exists(JSONPath))
                    {
                        Log.Error("PA.SpineFramework: Failed Loading JSON : Invaild Directory " + Spine_Dict + def.props.folderPath);
                        continue;
                    }
                    //string folderPath = Path.Combine(JSONPath, def.props.folderPath);
                    string atlasPath = Path.Combine(JSONPath, def.props.atlas);
                    string skelPath = Path.Combine(JSONPath, def.props.skeleton);
                    if (!File.Exists(atlasPath) || !File.Exists(skelPath))
                    {
                        continue;
                    }
                    Log.Warning("PA.SpineFramework: Loading JSON From Directory : " + Spine_Dict + def.props.folderPath);
                    atlasAsset = new TextAsset(File.ReadAllText(atlasPath));
                    skeletonAsset = new TextAsset(File.ReadAllText(skelPath));
                    atlasAsset.name = Path.GetFileNameWithoutExtension(def.props.atlas);
                    skeletonAsset.name = Path.GetFileNameWithoutExtension(def.props.skeleton);
                    //
                    textures = new Texture2D[def.props.textures.Count];
                    for (int i = 0; i < def.props.textures.Count; i++)
                    {
                        string texPath = Path.Combine(JSONPath, def.props.textures[i]);
                        if (!File.Exists(texPath))
                        {
                            Log.Error("PA.SpineFramework: Invaild Image Path : " + Spine_Dict + def.props.folderPath + "/" + def.props.textures[i]);
                            continue;
                        }
                        Texture2D texture = LoadManager.LoadTexture(new FileInfo(texPath));
                        textures[i] = texture;
                    }
                    if (textures.NullOrEmpty())
                    {
                        Log.Error("PA.SpineFramework: " + def.defName + "Texture Not Found in : " + Spine_Dict + def.props.folderPath);
                        continue;
                    }
                    if (def.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = spineShaderDatabase.TryGetValue(def.props.shader);
                    }
                    SpineAssetPack texPack = new SpineAssetPack(atlasAsset, skeletonAsset, textures, shader, def, useAlpha: def.props.StraightAlphaInput);
                    SavePackToVersionDatabase(def, texPack);
                    Log.Message("PA.SpineFramework: Successful Loaded " + def.defName + " with " + textures.Length + (textures.Length > 1 ? " Textures" : " Texture"));
                }
            }
            AllAssetsLoaded = true;
            Log.Message("PA.SpineFramework: Spine Assets Initialized");
            if (spine38_Database.NullOrEmpty())
            {
                Log.Error("PA.SpineFramework: No Any Spine Asset Found !");
            }
        }
        private static void SavePackToVersionDatabase(SpinePackDef spinedef, SpineAssetPack pack) 
        {
            if (spinedef.props.version == "3.8" && !spine38_Database.ContainsKey(spinedef.defName))
            {
                spine38_Database.Add(spinedef.defName, pack);
            }
            else if (spinedef.props.version == "4.1" && !spine41_Database.ContainsKey(spinedef.defName))
            {
                spine41_Database.Add(spinedef.defName, pack);
            }
        }
    }
}
