using Spine42;
using Spine42.Unity;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
#if !RELEASE_BUILD
    public enum SpinePawnState
    {
        Idle,
        Running,
        Jumping
    }
    public struct PawnParams
    {
        public float mood;
        public float pain;
    }
    //ViewModel视图层
    public class Test_PawnViewController : PawnBaseController<SkeletonAnimation>
    {
        // 槽位属性
        #region
        private SlotRotation LastRotation = SlotRotation.None;
        private readonly ExposedList<Slot> frontSlotInt = new();
        private readonly ExposedList<Slot> backSlotInt = new();
        private readonly ExposedList<Slot> sideSlotInt = new();
        #endregion

        private SpinePawnState previousViewState;

        //先实现一个忙等 写完换成协程
        protected override void Update()
        {
            base.Update();

        }

        public override void DoMove()
        {
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {
            if (Owner == null) return;
            transform.position = Owner.DrawPos + (Vector3.up * drawDepth);
        }

        public virtual void UpdateRotation()
        {
            switch (Owner.Rotation.AsInt)
            {
                case 0: FaceNorth(); break;
                case 1: FaceEast(); break;
                case 2: FaceSouth(); break;
                case 3: FaceWest(); break;
            }
        }

        //旋转
        #region
        public virtual void FaceNorth()
        {
            if (LastRotation == SlotRotation.Back) return;
            SetSlotVisible(frontSlotInt, false);
            SetSlotVisible(backSlotInt, true);
            SetSlotVisible(sideSlotInt, false);
            LastRotation = SlotRotation.Back;
        }

        public virtual void FaceEast()
        {
            if (LastRotation != SlotRotation.Side)
            {
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            SkeletonInstanceInt.DoFlipX(false);
            LastRotation = SlotRotation.Side;
        }

        public virtual void FaceSouth()
        {
            if (LastRotation == SlotRotation.Front) return;
            SetSlotVisible(frontSlotInt, true);
            SetSlotVisible(backSlotInt, false);
            SetSlotVisible(sideSlotInt, false);
            LastRotation = SlotRotation.Front;
        }

        public virtual void FaceWest()
        {
            if (LastRotation != SlotRotation.Side)
            {
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            SkeletonInstanceInt.DoFlipX(true);
            LastRotation = SlotRotation.Side;
        }

        //槽位
        protected virtual void InitializeSlotInfo()
        {
            if (SkeletonInstanceInt == null) return;
            var list = SkeletonInstanceInt.Skeleton.Slots;
            foreach (var slot in list)
            {
                string slotName = slot.Data.Name;
                if (slotName.StartsWith("F_")) frontSlotInt.Add(slot);
                else if (slotName.StartsWith("B_")) backSlotInt.Add(slot);
                else if (slotName.StartsWith("S_")) sideSlotInt.Add(slot);
            }
        }

        public virtual void SetSlotVisible(ExposedList<Slot> targetSlots, bool visible = true)
        {
            float alpha = visible ? 1 : 0;
            targetSlots.ForEach(slot => slot.A = alpha);
        }
        #endregion

        public void AnimationSwitchHandler()
        {
            if (SkeletonInstanceInt == null) return;

            SpinePawnState currentModelState = GetPawnCurrentState(Owner);
            if (previousViewState != currentModelState)
            {
                PlayNewStableAnimation();
            }
            previousViewState = currentModelState;
        }

        public void PlayNewStableAnimation()
        {
            SpinePawnState newModelState = GetPawnCurrentState(Owner);
            string nextAnimation;

            if (newModelState == SpinePawnState.Running)
            {
                nextAnimation = "run";
            }
            else
            {
                nextAnimation = "idle";
            }
            SkeletonInstanceInt.AnimationState.SetAnimation(0, nextAnimation, true);
        }

        //需要一个结构体保存参数
        public SpinePawnState GetPawnCurrentState(Pawn pawn)
        {
            return SpinePawnState.Idle;
        }
    }
#endif
}
