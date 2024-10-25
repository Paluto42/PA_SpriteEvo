using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PA_SpriteEvo
{
    internal class PawnKindDef : Def
    {
        public class FacialParts 
        {
            public SpinePackDef head;
            //Node childrens 前发,后发,眉毛,左眼,右眼,嘴巴
            public SpinePackDef frontHair;

            public SpinePackDef backHair;

            public SpinePackDef eyeBow;

            public SpinePackDef leftEye;

            public SpinePackDef rightEye;

            public SpinePackDef mouth;
        }
        public class BodyParts 
        {
            public SpinePackDef body;

        }
        public class 

        public FacialParts south;

        public FacialParts north;

        public FacialParts east;
    }
}
