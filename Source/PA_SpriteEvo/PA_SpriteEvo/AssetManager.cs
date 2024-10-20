﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    [StaticConstructorOnStartup]
    internal static class AssetManager
    {
        public static Shader Spine_Skeleton;

        public static Material SkeletonGraphic;

        public static Dictionary<string, Shader> SpineShaderDatabase = new Dictionary<string, Shader>();

        public static Dictionary<string, GameObject> ObjectDatabase = new Dictionary<string, GameObject>();

        public static Dictionary<string, SpineAssetPack> spine38_Database = new Dictionary<string, SpineAssetPack>();

        public static Dictionary<string, SpineAssetPack> spine41_Database = new Dictionary<string, SpineAssetPack>();

    }
}
