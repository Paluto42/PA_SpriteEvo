using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteEvo
{
    #if DEBUG_BUILD
    public class ControllerTrackerContainer : ScriptBase
    {
        public ControllerTracker tracker;

        public ControllerTrackerContainer()
        {
        }
        public override void Awake()
        {
        }

        public override void OnEnable()
        {
            tracker?.OnEnable();
        }

        public override void Update()
        {
            tracker?.Update();
        }
    }
    #endif
}
