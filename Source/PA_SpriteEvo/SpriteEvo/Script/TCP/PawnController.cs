using Spine42;
using System;
using System.Collections;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    #if DEBUG_BUILD
    public class PawnController : AnimationController_Base, IPawnRotate
    {
        int count = 0;
        float blinkMixIn = 0.4f;
        float blinkMixOut = 0.833f;
        //旋转
        #region
        public enum SlotRotation
        {
            None,
            Front,
            Back,
            Side
        }

        private SlotRotation lastRot = SlotRotation.None;
        #endregion

        #region
        ExposedList<Slot> frontSlotInt = new();
        ExposedList<Slot> backSlotInt = new();
        ExposedList<Slot> sideSlotInt = new();
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

        public override void ControllerTick()
        {
            if (Owner == null) return;
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
                case 0: FaceNorth();
                    break;
                case 1: FaceEast();
                    break;
                case 2: FaceSouth();
                    break;
                case 3: FaceWest();
                    break;
            }
        }

        //旋转
        #region
        public virtual void FaceNorth()
        {
            if (lastRot == SlotRotation.Back) return;
            SetSlotVisible(frontSlotInt, false);
            SetSlotVisible(backSlotInt, true);
            SetSlotVisible(sideSlotInt, false);
            lastRot = SlotRotation.Back;
        }

        public virtual void FaceEast()
        {
            if (lastRot != SlotRotation.Side){
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            skeletonInt.DoFlipX(false);
            lastRot = SlotRotation.Side;
        }

        public virtual void FaceSouth()
        {
            if (lastRot == SlotRotation.Front) return;
            SetSlotVisible(frontSlotInt, true);
            SetSlotVisible(backSlotInt, false);
            SetSlotVisible(sideSlotInt, false);
            lastRot = SlotRotation.Front;
        }

        public virtual void FaceWest()
        {
            if (lastRot != SlotRotation.Side){
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            skeletonInt.DoFlipX(true);
            lastRot = SlotRotation.Side;
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
                TrackEntry track1 = animationStateInt.AnimationState.AddAnimation(1, "blink", false, blinkMixIn);
                track1.Complete += CompleteEventHandler;
                return;
            }
            if (count >= 2)
            {
                count = 0;
                TrackEntry track2 = animationStateInt.AnimationState.AddAnimation(1, "idle", false, blinkMixOut);
            }
        }
    }
    #endif
}
