using Spine41;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteEvo.Extensions
{
    internal class SkeletonBinaryMerger : SkeletonBinary
    {
        public SkeletonBinaryMerger(AttachmentLoader attachmentLoader)
            : base(attachmentLoader)
        {
        }

        public SkeletonBinaryMerger(params Atlas[] atlasArray)
            : base(atlasArray)
        {
        }

        public SkeletonData ReadSkeletonDataToMerge(Stream file1, Stream file2)
        {
            return null;
        }
    }
}
