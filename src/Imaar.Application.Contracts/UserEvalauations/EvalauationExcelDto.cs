using System;

namespace Imaar.UserEvalauations
{
    public abstract class UserEvalauationExcelDtoBase
    {
        public int SpeedOfCompletion { get; set; }
        public int Dealing { get; set; }
        public int Cleanliness { get; set; }
        public int Perfection { get; set; }
        public int Price { get; set; }
    }
}