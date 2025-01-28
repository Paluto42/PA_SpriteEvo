using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public class HierarchyWindow : Window
    {
        public override Vector2 InitialSize => new(850f, 850f);
        public HierarchyWindow() 
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            forcePause = true;
            absorbInputAroundWindow = false;
            closeOnCancel = true;
            doCloseButton = false;
            doCloseX = true;
            draggable = true;
            drawShadow = false;
            preventCameraMotion = false;
            onlyOneOfTypeAllowed = true;
            resizeable = false;
            layer = WindowLayer.GameUI;
        }
        public override void DoWindowContents(Rect inRect)
        {
            DoHierarchyView(inRect);
        }
        public void DoHierarchyView(Rect inRect) 
        {

        }
    }
}
