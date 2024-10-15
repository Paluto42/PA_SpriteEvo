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
        private static string GetAssetLog(Texture2D[] texs) 
        {
            string[] message = texs.Select(t => $"{t.name}.png").ToArray();  
            string result = string.Join("   ", message);
            return result;
        }
        private static string GetAssetLog(Material[] mats)
        {
            string[] message = mats.Select(m => $"{m.name}.mat").ToArray();
            string result = string.Join("   ", message);
            return result;
        }
        private static void LoadAllSpineShader() 
        {
            ModContentPack modbase = Mods.FirstOrDefault(mod => mod.PackageId == TypeDef.ModID.ToLower());
            string SpinePath = Path.Combine(modbase.RootDir, "Asset", "spine38");
            if (SpinePath == null)
            {
                Log.Error("PA.SpineFramework Critical Error: Missing Spine Assets  " + SpinePath);
            }
            AssetBundle ab = AssetBundle.LoadFromFile(SpinePath);
            Shader Skeleton = ab.LoadAsset<Shader>("Spine-Skeleton");
            if (Skeleton != null && Spine_Skeleton == null)
            {
                Spine_Skeleton = Skeleton;
                Log.Message("PA.SpineFramework  Spine/Skeleton.Shader Loaded");
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
            List<SpinePackDef> list = DefDatabase<SpinePackDef>.AllDefsListForReading;
            List<AssetBundle> AssetBundle_Loaded = new List<AssetBundle>();
            string[] PATH_Spine = Mods.Select(mod_spine => Path.Combine(mod_spine.RootDir, "Asset_Spine")).Where(Directory.Exists).ToArray();
            foreach (SpinePackDef spinedef in list) 
            {
                TextAsset atlasAsset;
                TextAsset skeletonAsset;
                Material[] materials = null;
                Texture2D[] textures = null;
                Shader shader = null;
                //二进制读取
                if (spinedef.skelFormat == SkelFormat.Binary)
                {
                    AssetBundle ab;
                    if (!AssetBundle_Loaded.Exists((AssetBundle a) => a.name == spinedef.props.assetBundle))
                    {
                        string abPath = PATH_Spine.FirstOrDefault((string x) => File.Exists(Path.Combine(x, spinedef.props.assetBundle)));
                        if (abPath == null)
                        {
                            continue;
                        }
                        ab = AssetBundle.LoadFromFile(Path.Combine(abPath, spinedef.props.assetBundle));
                        AssetBundle_Loaded.Add(ab);
                        Log.Warning("PA.SpineFramework  : Load Assetbundle From : " + ab.name);
                    }
                    else
                    {
                        ab = AssetBundle_Loaded.First((AssetBundle a) => a.name == spinedef.props.assetBundle);
                    }
                    if (SkeletonGraphic == null)
                    {
                        Material SkeletonGraphicDefault = ab.LoadAsset<Material>("SkeletonGraphicDefault");
                        SkeletonGraphic = SkeletonGraphicDefault;
                    }
                    atlasAsset = ab?.LoadAsset<TextAsset>(spinedef.props.atlas);
                    skeletonAsset = ab.LoadAsset<TextAsset>(spinedef.props.skeleton);
                    if (spinedef.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = spineShaderDatabase.TryGetValue(spinedef.props.shader);
                    }
                    List<string> texinfo = spinedef.props.textures;
                    List<string> matinfo = spinedef.props.materials;
                    if (!texinfo.Empty())
                    {
                        textures = new Texture2D[spinedef.props.textures.Count];
                        for (int i = 0; i < spinedef.props.textures.Count; i++)
                        {
                            textures[i] = ab?.LoadAsset<Texture2D>(spinedef.props.textures[i]);
                        }
                        if (textures.NullOrEmpty())
                        {
                            Log.Error("[PA]" + spinedef.defName + "Texture2D[] Not Found");
                            continue;
                        }
                        else
                        {
                            Log.Message(spinedef.defName + " :   " + GetAssetLog(textures));
                        }
                        SpineAssetPack texPack = new SpineAssetPack(atlasAsset, skeletonAsset, textures, shader, spinedef, useAlpha: spinedef.props.StraightAlphaInput);
                        SavePackToVersionDatabase(spinedef, texPack);
                        //spine38_Database.Add(spinedef.defName, texPack);
                    }
                    else if (!matinfo.Empty())
                    {
                        materials = new Material[spinedef.props.materials.Count];
                        for (int i = 0; i < spinedef.props.materials.Count; i++)
                        {
                            materials[i] = ab?.LoadAsset<Material>(spinedef.props.materials[i]);
                        }
                        if (materials.NullOrEmpty())
                        {
                            Log.Error("[PA]" + spinedef.defName + "Material[] Not Found");
                            continue;
                        }
                        else
                        {
                            Log.Message(spinedef.defName + " :   " + GetAssetLog(materials));
                        }
                        SpineAssetPack matPack = new SpineAssetPack(atlasAsset, skeletonAsset, materials, spinedef, useAlpha: spinedef.props.StraightAlphaInput);
                        SavePackToVersionDatabase(spinedef, matPack);
                        //spine38_Database.Add(spinedef.defName, matPack);
                    }
                    else
                    {
                        Log.Error("[PA]" + spinedef.defName + " Non Texture or Material Found!");
                        continue;
                    }
                }
                //JSON读取
                else
                {
                    string JSONPath = PATH_Spine.FirstOrDefault((string x) => Directory.Exists(Path.Combine(x, spinedef.props.folderName)));
                    if (JSONPath == null)
                    {
                        Log.Message("PA.SpineFramework  : JSON Path " + spinedef.props.folderName + " Not Found!");
                        continue;
                    }
                    string folderPath = Path.Combine(JSONPath, spinedef.props.folderName);
                    string atlasPath = Path.Combine(folderPath, spinedef.props.atlas);
                    string skelPath = Path.Combine(folderPath, spinedef.props.skeleton);
                    if (!File.Exists(atlasPath) || !File.Exists(skelPath))
                    {
                        continue;
                    }
                    Log.Warning("PA.SpineFramework  : Load JSON Data From Folder : " + spinedef.props.folderName);
                    string altas = File.ReadAllText(atlasPath);
                    string json = File.ReadAllText(skelPath);
                    atlasAsset = new TextAsset(altas);
                    skeletonAsset = new TextAsset(json);
                    atlasAsset.name = Path.GetFileName(spinedef.props.atlas);
                    skeletonAsset.name = Path.GetFileName(spinedef.props.skeleton);
                    textures = new Texture2D[spinedef.props.textures.Count];
                    for (int i = 0; i < spinedef.props.textures.Count; i++)
                    {
                        Texture2D texture = Resources.Load<Texture2D>(Path.Combine(folderPath, spinedef.props.textures[i]));
                        textures[i] = texture;
                    }
                    if (textures.NullOrEmpty())
                    {
                        Log.Error("[PA]" + spinedef.defName + "Texture2D[] Not Found");
                        continue;
                    }
                    else
                    {
                        //您猜怎么着 直接从文件加载的贴图 没有name属性！
                        string[] message = new string[textures.Length];
                        for (int i = 0; i < message.Length; i++) 
                        {
                            message[i] = spinedef.props.textures[i];
                        }
                        string result = string.Join("   ", message);
                        Log.Message(spinedef.defName + " :   " + result);
                    }
                    if (spinedef.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = spineShaderDatabase.TryGetValue(spinedef.props.shader);
                    }
                    SpineAssetPack texPack = new SpineAssetPack(atlasAsset, skeletonAsset, textures, shader, spinedef, useAlpha: spinedef.props.StraightAlphaInput);
                    SavePackToVersionDatabase(spinedef, texPack);
                }
            }
            AllAssetsLoaded = true;
            Log.Message("PA.SpineFramework : Spine Assets Initialized");
            if (spine38_Database.NullOrEmpty())
            {
                Log.Error("PA.SpineFramework : No Any Spine Asset Found !");
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
