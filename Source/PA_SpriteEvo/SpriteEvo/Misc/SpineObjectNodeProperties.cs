using SpriteEvo.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public class SpineObjectNodeProperties
    {
        public Type workerClass = typeof(BaseControllComp);

        public AttachmentTag tag;

        public List<SpineObjectNodeProperties> children;

        private BaseControllComp workerInt;

        public void EnsureInitialized() 
        {
            if (workerInt == null) 
            {
                workerInt = GetWorker(workerClass);
            }
        }
        public static BaseControllComp GetWorker(Type type)
        {
            return (BaseControllComp)Activator.CreateInstance(type);
        }
    }
}
