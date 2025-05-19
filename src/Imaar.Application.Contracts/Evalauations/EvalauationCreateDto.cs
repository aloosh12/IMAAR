using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Evalauations
{
    public abstract class EvalauationCreateDtoBase
    {
        public int SpeedOfCompletion { get; set; } = 0;
        public int Dealing { get; set; } = 0;
        public int Cleanliness { get; set; } = 0;
        public int Perfection { get; set; } = 0;
        public int Price { get; set; } = 0;
        public Guid Evaluatord { get; set; }
        public Guid EvaluatedPersonId { get; set; }
    }
}