using System;

namespace Imaar.Evalauations
{
    public abstract class EvalauationExcelDtoBase
    {
        public int SpeedOfCompletion { get; set; }
        public int Dealing { get; set; }
        public int Cleanliness { get; set; }
        public int Perfection { get; set; }
        public int Price { get; set; }
    }
}