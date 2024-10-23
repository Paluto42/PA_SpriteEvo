using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PA_SpriteEvo
{
    [StaticConstructorOnStartup]
    internal static class AssetLoadManager
    {
        public static bool AllAssetsLoaded = false;

        public static bool AllShadersLoaded = false;

        private static readonly string Spine_Dict = "Asset/";
        //Animation的Spine/Skeleton shader && Graphic的Spine/SkeletonGraphic shader
        public static Shader Spine_Skeleton;

        public static Material SkeletonGraphic;
        private static Dictionary<string, Shader> Shader_DB => AssetManager.SpineShaderDatabase;
        private static Dictionary<string, SpineAssetPack> Spine38_DB => AssetManager.spine38_Database;
        private static Dictionary<string, SpineAssetPack> Spine41_DB => AssetManager.spine41_Database;
        private static List<ModContentPack> Mods => LoadedModManager.RunningModsListForReading;
        static AssetLoadManager() 
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
                Shader_DB.Add(s.name, s);
            }
            if (Shader_DB.NullOrEmpty())
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
            List<string> SkeletonJSON_Loaded = new List<string>();
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
                if (def.skelFormat == SkelFormat.SkeletonBinary)
                {
                    AssetBundle ab;
                    string assetBundleName = Path.GetFileName(def.assetBundle);
                    if (!AssetBundle_Loaded.Exists((AssetBundle a) => a.name == assetBundleName))
                    {
                        string abPath = Path.Combine(IndividualPath, def.assetBundle);
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
                    string atlas;
                    string skeleton;
                    string autofillfilename = def.seriesname;
                    if (def.props.atlas != null)
                        atlas = def.props.atlas;
                    else
                        atlas = autofillfilename + ".atlas.txt";
                    if (def.props.skeleton != null)
                        skeleton = def.props.skeleton;
                    else
                        skeleton = autofillfilename + ".skel.bytes";

                    atlasAsset = ab?.LoadAsset<TextAsset>(atlas);
                    skeletonAsset = ab?.LoadAsset<TextAsset>(skeleton);
                    if (def.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = Shader_DB.TryGetValue(def.props.shader);
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
                    string JSONPath;
                    if (!SkeletonJSON_Loaded.Exists((string s) => s == def.folderPath))
                    {
                        JSONPath = Path.Combine(IndividualPath, def.folderPath);
                        if (!Directory.Exists(JSONPath))
                        {
                            Log.Error("PA.SpineFramework: Failed Loading JSON : Invaild Directory " + Spine_Dict + def.folderPath);
                            continue;
                        }
                        SkeletonJSON_Loaded.Add(def.folderPath);
                        Log.Warning("PA.SpineFramework: Loading JSON From Directory : " + Spine_Dict + def.folderPath);
                    }
                    else 
                    {
                        string existsPath = SkeletonJSON_Loaded.First((string s) => s == def.folderPath);
                        JSONPath = Path.Combine(IndividualPath, existsPath);
                    }
                    //string folderPath = Path.Combine(JSONPath, def.props.folderPath);
                    string atlas;
                    string skeleton;
                    string autofillfilename = def.seriesname;

                    if (def.props.atlas != null)
                        atlas = def.props.atlas;
                    else
                        atlas = autofillfilename + ".atlas.txt";
                    if (def.props.skeleton != null)
                        skeleton = def.props.skeleton;
                    else
                        skeleton = autofillfilename + ".json";

                    string atlasPath = Path.Combine(JSONPath, atlas);
                    string skelPath = Path.Combine(JSONPath, skeleton);
                    if (!File.Exists(atlasPath) || !File.Exists(skelPath))
                    {
                        continue;
                    }
                    atlasAsset = new TextAsset(File.ReadAllText(atlasPath));
                    skeletonAsset = new TextAsset(File.ReadAllText(skelPath));
                    atlasAsset.name = Path.GetFileNameWithoutExtension(atlas);
                    skeletonAsset.name = Path.GetFileNameWithoutExtension(skeleton);
                    //
                    if (!def.props.textures.NullOrEmpty())
                    {
                        textures = new Texture2D[def.props.textures.Count];
                        for (int i = 0; i < def.props.textures.Count; i++)
                        {
                            string texPath = Path.Combine(JSONPath, def.props.textures[i]);
                            if (!File.Exists(texPath))
                            {
                                Log.Error("PA.SpineFramework: Invaild Image Path : " + Spine_Dict + def.folderPath + "/" + def.props.textures[i]);
                                continue;
                            }
                            Texture2D texture = LoadTexture(new FileInfo(texPath));
                            textures[i] = texture;
                        }
                    }
                    else
                    {
                        string tex = autofillfilename + ".png";
                        string texPath = Path.Combine(JSONPath, tex);
                        if (!File.Exists(texPath))
                        {
                            Log.Error("PA.SpineFramework: Invaild AutoGenerated Image Path : " + Spine_Dict + def.folderPath + "/" + tex);
                            continue;
                        }
                        textures = new Texture2D[1];
                        Texture2D texture = LoadTexture(new FileInfo(texPath));
                        textures[0] = texture;
                    }
                    if (textures.NullOrEmpty())
                    {
                        Log.Error("PA.SpineFramework: " + def.defName + " Texture Not Found in : " + Spine_Dict + def.folderPath);
                        continue;
                    }
                    if (def.props.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = Shader_DB.TryGetValue(def.props.shader);
                    }
                    SpineAssetPack texPack = new SpineAssetPack(atlasAsset, skeletonAsset, textures, shader, def, useAlpha: def.props.StraightAlphaInput);
                    SavePackToVersionDatabase(def, texPack);
                    Log.Message("PA.SpineFramework: Successful Loaded " + def.defName + " with " + textures.Length + (textures.Length > 1 ? " Textures" : " Texture"));
                }
            }
            AllAssetsLoaded = true;
            Log.Message("PA.SpineFramework: Spine Assets Initialized");
            if (Spine38_DB.NullOrEmpty())
            {
                Log.Error("PA.SpineFramework: No Any Spine Asset Found !");
            }
        }
        private static void SavePackToVersionDatabase(SpinePackDef spinedef, SpineAssetPack pack)
        {
            if (spinedef.props.version == "3.8" && !Spine38_DB.ContainsKey(spinedef.defName))
            {
                Spine38_DB.Add(spinedef.defName, pack);
            }
            else if (spinedef.props.version == "4.1" && !Spine41_DB.ContainsKey(spinedef.defName))
            {
                Spine41_DB.Add(spinedef.defName, pack);
            }
        }
        private static Texture2D LoadTexture(FileInfo file)
        {
            Texture2D texture2D = null;
            if (file.Exists)
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: true);
                texture2D.LoadImage(data);
                if (texture2D.width % 4 != 0 || texture2D.height % 4 != 0)
                {
                    if (Prefs.LogVerbose)
                    {
                        Debug.LogWarning($"Texture does not support mipmapping, needs to be divisible by 4 ({texture2D.width}x{texture2D.height}) for '{file.Name}'");
                    }
                    texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: false);
                    texture2D.LoadImage(data);
                }
                texture2D.name = Path.GetFileNameWithoutExtension(file.Name);
                texture2D.filterMode = FilterMode.Trilinear;
                texture2D.anisoLevel = 2;
                texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);
            }
            return texture2D;
        }
    }
}
