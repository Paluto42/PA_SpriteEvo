using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PA_SpriteEvo
{
    public class HeadPartsDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;

        public string assetBundle;

        public string folderPath;

        public string version = "3.8";
        //旋转方向
        public Rot4 rotation = Rot4.South;
        //头部Root FX 头部
        public SpinePackDef head;
        //Node childrens 前发,后发,眉毛,左眼,右眼,嘴巴
        public SpinePackDef frontHair;

        public SpinePackDef backHair;

        public SpinePackDef eyeBow;

        public SpinePackDef leftEye;

        public SpinePackDef rightEye;

        public SpinePackDef mouth;
    }
}
