using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RimWorld.IO;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    internal class LoadManager
    {
        public static string CombineFilesName(Texture2D[] texs)
        {
            string[] message = texs.Select(t => $"{t.name}.png").ToArray();
            string result = string.Join("   ", message);
            return result;
        }
        public static string CombineFilesName(Material[] mats)
        {
            string[] message = mats.Select(m => $"{m.name}.mat").ToArray();
            string result = string.Join("   ", message);
            return result;
        }
        public static Texture2D LoadTexture(FileInfo file)
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
