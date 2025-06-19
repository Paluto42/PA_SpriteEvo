using UnityEngine;
using Verse;

namespace SpriteEvo
{
#if !RELEASE_BUILD
    public class PawnBaseController<T> : ControllerBase<T> where T : MonoBehaviour
    {
        private Pawn ownerInt;
        public Pawn Owner => ownerInt ??= referenceKey as Pawn;

        protected override void Update()
        {
            DoMove();
        }

        public virtual void DoMove()
        {

        }
    }
#endif
}
