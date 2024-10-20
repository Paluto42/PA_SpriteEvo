using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PA_SpriteEvo
{
    public class BodyPartsDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;

        public string assetBundle;

        public string folderPath;
        //旋转方向
        public Rot4 rotation = Rot4.South;
        //头部Root FX Body
        public SpinePackDef body;
        //Node childrens 前衣后衣
        public SpinePackDef frontClothes;

        public SpinePackDef backClothes;
    }
}
