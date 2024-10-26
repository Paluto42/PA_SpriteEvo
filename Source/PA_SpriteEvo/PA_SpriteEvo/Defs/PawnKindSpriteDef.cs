using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static PA_SpriteEvo.PawnKindSpriteDef;

namespace PA_SpriteEvo
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
    public class Head
    {
        public FacialParts south;
        public FacialParts north;
        public FacialParts west;
        public FacialParts east;
    }
    public class Body
    {
        public BodyParts south;
        public BodyParts north;
        public BodyParts west;
        public BodyParts east;
    }
    public class PawnKindSpriteDef : Def
    {
        public string version = "4.1";

        public Head head;

        public Body body;
    }
}
