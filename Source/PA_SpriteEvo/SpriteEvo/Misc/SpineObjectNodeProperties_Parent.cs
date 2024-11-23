using SpriteEvo.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpriteEvo
{
    public class SpineObjectNodeProperties_Parent : SpineObjectNodeProperties
    {
        public SpineObjectNodeProperties_Parent()
        {
            workerClass = typeof(FxRootWorker);
        }
        
    }
}
