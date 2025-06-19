using Verse;

namespace SpriteEvo
{
#if !RELEASE_BUILD
    public class CC_Pawn : CastConditioner_Base
    {
        public int Priority = 1;
        public override bool Castable(ScriptBase instance)
        {
            if (instance.referenceKey is not Pawn p) return false;
            return true;
        }
    }
#endif
}
