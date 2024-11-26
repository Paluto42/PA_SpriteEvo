using System.Collections.Generic;
using Verse;

namespace SpriteEvo
{
    public enum AttachmentTag 
    {
        None = 0,
        FrontHair = 1,
        BackHair = 2,
        EyeBow = 3,
        LeftEye = 4,
        RightEye = 5,
        Mouth = 6,
        FrontClothes = 7,
        BackClothes = 8
    }
    public class Attachment 
    {
        public SpineAssetDef attachment;
        public AttachmentTag tag = AttachmentTag.None;
        public int layer = 0;
    } 
    public class ParentWithAttachment
    {
        public SpineAssetDef parent;
        public List<Attachment> attachments = new List<Attachment>();
    }
    public class VWH_Model
    {
        public ParentWithAttachment south;
        public ParentWithAttachment north;
        public ParentWithAttachment west;
        public ParentWithAttachment east;
    }
    public class PawnKindSpriteDef : Def
    {
        public string version = "4.1";

        public VWH_Model head;

        public VWH_Model body;
    }
}
