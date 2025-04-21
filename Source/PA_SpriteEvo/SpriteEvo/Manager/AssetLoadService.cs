using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    internal static class AssetLoadService
    {
        private const string Spine_Dict = "Asset/";

        private static bool AllAssetsLoaded = false;
        private static bool AllShadersLoaded = false;

        //SkeletonAnimation使用Spine/Skeleton.shader && 而Graphic使用Spine/SkeletonGraphic.shader
        public static Shader Spine_Skeleton;
        public static Shader Spine_Skeleton_Straight;
        //public static Shader Spine_SkeletonGraphic;
        //这两个比较特殊 不能运行时改
        public static Material SkeletonGraphicDefault;
        public static Material SkeletonGraphicDefaul_Straight;

        private static Dictionary<string, Shader> Shader_DB => AssetManager.SpineShaderDatabase;
        private static Dictionary<string, AssetLoader> Spine38_DB => AssetManager.spine38_Database;
        private static Dictionary<string, AssetLoader> Spine41_DB => AssetManager.spine41_Database;
        private static Dictionary<string, AssetLoader> Spine42_DB => AssetManager.spine42_Database;
        private static List<ModContentPack> Mods => LoadedModManager.RunningModsListForReading;
        static AssetLoadService() 
        {
            LoadAllShader();
            LoadAllAssets();
            /*Patch_UIRoot.DoMainMenuOnce += LoadAllShader;
            Patch_UIRoot.DoMainMenuOnce += LoadAllAssets;
            Patch_UIRoot.DoMainMenuOnce += Patch_UIRoot.ClearEvents;*/
        }

        private static void LoadAllShader()
        {
            if (AllShadersLoaded) return;
            ModContentPack modbase = Mods.FirstOrDefault(mod => mod.PackageId == TypeDef.ModID.ToLower());
            string SpinePath = Path.Combine(modbase.RootDir, "Asset", "spine38");
            if (SpinePath == null)
            {
                Log.Error("SpriteEvo: Critical Error: Missing Skeleton Assets " + SpinePath);
            }
            AssetBundle ab = AssetBundle.LoadFromFile(SpinePath);
            Shader skeletonShader = ab.LoadAsset<Shader>("Spine-Skeleton");
            if (skeletonShader != null && Spine_Skeleton == null)
            {
                Spine_Skeleton = skeletonShader;
                Log.Message("SpriteEvo: Spine/Skeleton.Shader Loaded");
            }
            Shader skeletonShader_Straight = ab.LoadAsset<Shader>("Spine-Skeleton#Straight");
            if (skeletonShader_Straight != null && Spine_Skeleton_Straight == null)
            {
                Spine_Skeleton_Straight = skeletonShader_Straight;
                Log.Message("SpriteEvo: Spine/Skeleton#Straight.Shader Loaded");
            }
            Material SkeletonGraphic = ab.LoadAsset<Material>("SkeletonGraphicDefault");
            if (SkeletonGraphic != null && SkeletonGraphicDefault == null)
            {
                Log.Message("SpriteEvo: Spine/SkeletonGraphicDefault.Material Loaded");
                SkeletonGraphicDefault = SkeletonGraphic;
            }
            Material SkeletonGraphic_Straight = ab.LoadAsset<Material>("SkeletonGraphicDefaul-Straight");
            if (SkeletonGraphic_Straight != null && SkeletonGraphicDefault == null)
            {
                Log.Message("SpriteEvo: Spine/SkeletonGraphicDefault-Straight.Material Loaded");
                SkeletonGraphicDefaul_Straight = SkeletonGraphic_Straight;
            }
            //Shader[] shaders = ab.LoadAllAssets<Shader>();
            string[] shaderfiles = ab.GetAllAssetNames();
            foreach (string fileName in shaderfiles) 
            {
                string id = Path.GetFileName(fileName);
                Shader shader = ab.LoadAsset<Shader>(id);
                if (shader == null) continue;
                Shader_DB.Add(id, shader);
            }
            /*foreach (Shader s in shaders)
            {
                Shader_DB.Add(s.name, s);
                Log.Message(s.name);
            }*/
            if (Shader_DB.NullOrEmpty())
            {
                AllShadersLoaded = false;
                return;
            }
            else
            {
                AllShadersLoaded = true;
                Log.Message("SpriteEvo: All Shader Loaded, Initializing Assets...");
            }
        }
        private static void LoadAllAssets()
        {
            if (AllAssetsLoaded) return;
            ResloveAllAssetBundle();
            AllAssetsLoaded = true;
        }

        private static void ResloveAllAssetBundle()
        {
            if (!AllShadersLoaded) return;
            List<AssetBundle> AssetBundle_Loaded = new();
            List<string> SkeletonJSON_Loaded = new();
            List<SpineAssetDef> packs = DefDatabase<SpineAssetDef>.AllDefsListForReading;
            foreach (SpineAssetDef def in packs)
            {
                if (def.asset.version == null)
                {
                    Log.Error($"SpriteEvo: SpineAsset \"" + def.defName + "\" Has Invalid Version, Skiped");
                    continue;
                }
                TextAsset atlasAsset = null;
                TextAsset skeletonAsset = null;
                Material[] materials = null;
                Texture2D[] textures = null;
                Shader shader = null;
                string IndividualPath = Path.Combine(def.modContentPack.RootDir, Spine_Dict);
                //AB是唯一读取外部二进制TextAsset的方法 AssetDataBase.LoadAsset<T>()和Resource.Load<T>()都无法使用
                List<string> errorInfos = new();
                bool isInvalid = false;
                if (def.skelFormat == SkelFormat.SkeletonBinary)
                {
                    if (def.asset.filePath == null) continue;
                    AssetBundle ab;
                    string assetBundleName = Path.GetFileName(def.asset.filePath);
                    if (!AssetBundle_Loaded.Exists((AssetBundle a) => a.name == assetBundleName))
                    {
                        string abPath = Path.Combine(IndividualPath, def.asset.filePath);
                        if (!File.Exists(abPath)) 
                        {
                            Log.Error("SpriteEvo: Asset \"" + def.defName + "\" Does not exist filePath :\"" + def.asset.filePath + "\"");
                            continue;
                        }
                        ab = AssetBundle.LoadFromFile(abPath);
                        AssetBundle_Loaded.Add(ab);
                        if (SPE_ModSettings.debugOverride)
                            Log.Warning("SpriteEvo: Loading SkeletonBinary" + " From Assetbundle : " + Spine_Dict + ab.name);
                    }
                    else
                    {
                        ab = AssetBundle_Loaded.First((AssetBundle a) => a.name == assetBundleName);
                    }
                    string atlas;
                    string skeleton;
                    string autofillfilename = def.asset.seriesname;
                    if (def.asset.atlas != null)
                        atlas = def.asset.atlas;
                    else
                        atlas = autofillfilename + ".atlas.txt";
                    if (def.asset.skeleton != null)
                        skeleton = def.asset.skeleton;
                    else
                        skeleton = autofillfilename + ".skel.bytes";
                    //模糊查找
                    atlasAsset = ab?.LoadAsset<TextAsset>(atlas);
                    if (atlasAsset == null)
                    {
                        atlas = autofillfilename + ".atlas";
                        atlasAsset = ab?.LoadAsset<TextAsset>(atlas);
                    }
                    skeletonAsset = ab?.LoadAsset<TextAsset>(skeleton);
                    if (skeletonAsset == null)
                    {
                        skeleton = autofillfilename + ".skel";
                        skeletonAsset = ab?.LoadAsset<TextAsset>(skeleton);
                    }
                    //
                    if (atlasAsset == null)
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Atlas\"" + atlas + "\"");
                    }
                    if (skeletonAsset == null)
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Skeleton\"" + skeleton + "\"");
                    }
                    /*if (def.asset.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = Shader_DB.TryGetValue(def.asset.shader);
                    }*/
                    List<string> texinfo = def.asset.textures;
                    List<string> matInfo = def.asset.materials;
                    if (!matInfo.Empty())
                    {
                        List<string> badmatInfos = new();
                        materials = new Material[matInfo.Count];
                        for (int i = 0; i < matInfo.Count; i++)
                        {
                            materials[i] = ab?.LoadAsset<Material>(matInfo[i]);
                            if (materials[i] == null)
                            {
                                isInvalid = true;
                                badmatInfos.Add(matInfo[i]);
                            }
                        }
                        if (materials.NullOrEmpty())
                        {
                            isInvalid = true;
                            errorInfos.Add("SpriteEvo: SkeletonBinary \"" + def.defName + "\" Missing Materials");
                        }
                        if (badmatInfos.Count != 0)
                        {
                            foreach (var matError in badmatInfos)
                            {
                                errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Material\"" + matError + "\"");
                            }
                        }
                        if (isInvalid)
                        {
                            OutputAssetErrorMsg(errorInfos, def.defName, true);
                            continue;
                        }
                        Loader_Mat matPack = new(def, atlasAsset, skeletonAsset, materials, usePMA: def.asset.StraightAlphaInput);
                        SavePackToVersionDatabase(def, matPack);
                        Log.Message("SpriteEvo: Successful Loaded SkeletonBinary \"" + def.defName + "\" with " + materials.Length + (materials.Length > 1 ? " Materials" : " Material"));
                    }
                    else if (!texinfo.Empty())
                    {
                        List<string> badtexInfos = new();
                        textures = new Texture2D[texinfo.Count];
                        for (int i = 0; i < texinfo.Count; i++)
                        {
                            textures[i] = ab?.LoadAsset<Texture2D>(texinfo[i]);
                            if (textures[i] == null)
                            {
                                isInvalid = true;
                                badtexInfos.Add(texinfo[i]);
                            }
                        }
                        if (textures.NullOrEmpty())
                        {
                            isInvalid = true;
                            errorInfos.Add("SpriteEvo: SkeletonBinary\"" + def.defName + "\" Missing Textures");
                        }
                        if (badtexInfos.Count != 0)
                        {
                            foreach (var texError in badtexInfos)
                            {
                                errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Texture\"" + texError + "\"");
                            }
                        }
                        shader = ParseFromFileName(def.asset.shader, def.asset.StraightAlphaInput);
                        if (shader == null)
                        {
                            isInvalid = true;
                            errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Failed to load corresponding Shader \"" + def.asset.shader + "\"");
                        }
                        if (isInvalid)
                        {
                            OutputAssetErrorMsg(errorInfos, def.defName, true);
                            continue;
                        }
                        Loader_Tex texPack = new(def, atlasAsset, skeletonAsset, textures, shader, useStraight: def.asset.StraightAlphaInput);
                        SavePackToVersionDatabase(def, texPack);
                        Log.Message("SpriteEvo: Successful Loaded SkeletonBinary \"" + def.defName + "\" with " + textures.Length + (textures.Length > 1 ? " Textures" : " Texture"));
                    }
                    else
                    {
                        Log.Warning("SpriteEvo: SkeletonBinary \"" + def.defName + "\" Missing Material or Textures");
                        continue;
                    }
                }
                //JSON读取
                else if(def.skelFormat == SkelFormat.SkeletonJSON)
                {
                    if (def.asset.filePath == null) continue;
                    string JSONPath;
                    if (!SkeletonJSON_Loaded.Exists((string s) => s == def.asset.filePath))
                    {
                        JSONPath = Path.Combine(IndividualPath, def.asset.filePath);
                        if (!Directory.Exists(JSONPath))
                        {
                            Log.Error("SpriteEvo: Failed Loading SkeletonJSON \"" + def.defName + "\" : Invalid Directory " + Spine_Dict + def.asset.filePath);
                            continue;
                        }
                        SkeletonJSON_Loaded.Add(def.asset.filePath);
                        if (SPE_ModSettings.debugOverride)
                            Log.Warning("SpriteEvo: Loading SkeletonJSON From Directory : " + Spine_Dict + def.asset.filePath);
                    }
                    else 
                    {
                        string existsPath = SkeletonJSON_Loaded.First((string s) => s == def.asset.filePath);
                        JSONPath = Path.Combine(IndividualPath, existsPath);
                    }
                    //string folderPath = Path.Combine(JSONPath, def.props.folderPath);
                    string atlas;
                    string skeleton;
                    string autofillfilename = def.asset.seriesname;

                    if (def.asset.atlas != null)
                        atlas = def.asset.atlas;
                    else
                        atlas = autofillfilename + ".atlas.txt";
                    if (def.asset.skeleton != null)
                        skeleton = def.asset.skeleton;
                    else
                        skeleton = autofillfilename + ".json";

                    string atlasPath = Path.Combine(JSONPath, atlas);
                    string skelPath = Path.Combine(JSONPath, skeleton);
                    if (!File.Exists(atlasPath)) 
                    {
                        atlas = autofillfilename + ".atlas";
                        atlasPath = Path.Combine(JSONPath, atlas);
                    }
                    if (!File.Exists(atlasPath))
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Atlas\"" + atlas + "\"");
                    }
                    if (!File.Exists(skelPath))
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Does not exist Skeleton\"" + skeleton + "\"");
                    }
                    if (File.Exists(atlasPath) && File.Exists(skelPath))
                    {
                        atlasAsset = new TextAsset(File.ReadAllText(atlasPath));
                        skeletonAsset = new TextAsset(File.ReadAllText(skelPath));
                        atlasAsset.name = Path.GetFileNameWithoutExtension(atlas);
                        skeletonAsset.name = Path.GetFileNameWithoutExtension(skeleton);
                    }
                    //
                    List<string> texinfo = def.asset.textures;
                    //Material只能在Unity运行时内部创建,读不了
                    if (!texinfo.NullOrEmpty())
                    {
                        textures = new Texture2D[texinfo.Count];
                        for (int i = 0; i < texinfo.Count; i++)
                        {
                            string texPath = Path.Combine(JSONPath, texinfo[i]);
                            if (!File.Exists(texPath)) 
                            {
                                texPath += ".png";
                            }
                            if (!File.Exists(texPath))
                            {
                                isInvalid = true;
                            }
                            Texture2D texture = LoadTexture(new FileInfo(texPath));
                            textures[i] = texture;
                        }
                    }
                    else
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: SkeletonBinary \"" + def.defName + "\" Missing Textures");
                    }
                    shader = ParseFromFileName(def.asset.shader, def.asset.StraightAlphaInput);
                    if (shader == null)
                    {
                        isInvalid = true;
                        errorInfos.Add("SpriteEvo: Asset \"" + def.defName + "\" Failed to load corresponding Shader \"" + def.asset.shader + "\"");
                    }
                    if (isInvalid)
                    {
                        OutputAssetErrorMsg(errorInfos, def.defName, false);
                        continue;
                    }
                    /*if (def.asset.shader == "Spine-Skeleton.shader")
                    {
                        shader = Spine_Skeleton;
                    }
                    else
                    {
                        shader = Shader_DB.TryGetValue(def.asset.shader);
                    }*/
                    Loader_Tex texPack = new(def, atlasAsset, skeletonAsset, textures, shader, useStraight: def.asset.StraightAlphaInput);
                    SavePackToVersionDatabase(def, texPack);
                    if (SPE_ModSettings.debugOverride)
                        Log.Message("SpriteEvo: Successful Loaded SkeletonJSON \"" + def.defName + "\" with " + textures.Length + (textures.Length > 1 ? " Textures" : " Texture"));
                }
            }
            AllAssetsLoaded = true;
            if (Spine38_DB.NullOrEmpty())
            {
                Log.Warning("SpriteEvo: No Any Asset Found !");
            }
            else 
            { 
                Log.Message("SpriteEvo: All Assets Initialized");
            }
        }

        private static void SavePackToVersionDatabase(SpineAssetDef spinedef, AssetLoader pack)
        {
            if (spinedef.asset.version == "3.8" && !Spine38_DB.ContainsKey(spinedef.defName))
            {
                Spine38_DB.Add(spinedef.defName, pack);
            }
            else if (spinedef.asset.version == "4.1" && !Spine41_DB.ContainsKey(spinedef.defName))
            {
                Spine41_DB.Add(spinedef.defName, pack);
            }
            else if (spinedef.asset.version == "4.2" && !Spine42_DB.ContainsKey(spinedef.defName))
            {
                Spine42_DB.Add(spinedef.defName, pack);
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

        private static void OutputAssetErrorMsg(List<string> errorInfos, string defName, bool isBinary) 
        {
            string SkeletonType;
            foreach (var errorInfo in errorInfos)
            {
                Log.Error(errorInfo);
            }
            if (isBinary) 
                SkeletonType = "SkeletonBinary";
            else 
                SkeletonType = "SkeletonJSON";
            Log.Error("SpriteEvo: Failed To Load " + SkeletonType + " \"" + defName + "\"");
        }

        private static Shader ParseFromFileName(string originalName, bool useStraight) 
        {
            string name = originalName.ToLower();
            string id = Path.GetFileNameWithoutExtension(name);
            string extension = Path.GetExtension(name);
            string result = useStraight ? string.Concat(id, "#straight", extension) : name;
            if (Shader_DB.ContainsKey(result)){
                Shader_DB.TryGetValue(result, out var shaderStright);
                return shaderStright;
            }
            if(useStraight){
                Shader_DB.TryGetValue(id, out var shaderPMA);
                return shaderPMA;
            }
            return null;
        }
    }
}
