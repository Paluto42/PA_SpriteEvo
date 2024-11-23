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
        public Type workerClass = typeof(BaseControllWorker);

        public AttachmentTag tag;

        public List<SpineObjectNodeProperties> children;

        private BaseControllWorker workerInt;

        public void EnsureInitialized() 
        {
            if (workerInt == null) 
            {
                workerInt = GetWorker(workerClass);
            }
        }
        public static BaseControllWorker GetWorker(Type type)
        {
            return (BaseControllWorker)Activator.CreateInstance(type);
        }
    }
}
