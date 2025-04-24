using Spine42;
using Spine42.Unity;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
#if DEBUG_BUILD
    public enum SlotRotation
    {
        None,
        Front,
        Back,
        Side
    }
    public class ACP_PawnController : ScriptProperties
    {
        public float blinkMixIn = 0.4f;
        public float blinkMixOut = 0.833f;

        public ACP_PawnController()
        {
            scriptClass = typeof(AC_PawnController);
        }
    }

    public class AC_PawnController : AnimationControllerBase<ISkeletonComponent, IAnimationStateComponent>, IPawnRotate
    {
        ACP_PawnController Props => props as ACP_PawnController;

        int count = 0;
        float BlinkMixIn => Props.blinkMixIn;
        float BlinkMixOut => Props.blinkMixOut;

        // 槽位属性
        #region
        private SlotRotation LastRotation = SlotRotation.None;
        private readonly ExposedList<Slot> frontSlotInt = new();
        private readonly ExposedList<Slot> backSlotInt = new();
        private readonly ExposedList<Slot> sideSlotInt = new();
        #endregion

        protected Pawn ownerInt;
        public Pawn Owner => ownerInt ??= referenceKey as Pawn;

        protected override void Awake() 
        {
            base.Awake();
            InitializeSlotInfo();
        }

        protected override void OnEnable()
        {
            if (animationStateInt == null) return;
            TrackEntry track0 = animationStateInt.AnimationState.SetAnimation(0, "idle", true);
            track0.Complete += CompleteEventHandler;
        }

        protected override void Update()
        {
            UpdateRotation();
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {
            transform.position = Owner.DrawPos + (Vector3.up * drawDepth);
        }

        public virtual void UpdateRotation() 
        {
            switch (Owner.Rotation.AsInt)
            {
                case 0: FaceNorth(); break;
                case 1: FaceEast();  break;
                case 2: FaceSouth(); break;
                case 3: FaceWest();  break;
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
            if (LastRotation != SlotRotation.Side){
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            skeletonInt.DoFlipX(false);
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
            if (LastRotation != SlotRotation.Side){
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            skeletonInt.DoFlipX(true);
            LastRotation = SlotRotation.Side;
        }
        #endregion

        protected virtual void InitializeSlotInfo()
        {
            if (animationStateInt == null) return;
            var list = skeletonInt.Skeleton.Slots;
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
            float alpha = visible? 1 : 0;
            targetSlots.ForEach(slot => slot.A = alpha);
        }

        private void CompleteEventHandler(TrackEntry trackEntry)
        {
            count++;
            if (count == 1)
            {
                TrackEntry track1 = animationStateInt.AnimationState.AddAnimation(1, "blink", false, BlinkMixIn);
                track1.Complete += CompleteEventHandler;
                return;
            }
            if (count >= 2)
            {
                count = 0;
                TrackEntry track2 = animationStateInt.AnimationState.AddAnimation(1, "idle", false, BlinkMixOut);
            }
        }
    }
    #endif
}
