using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationCreateDtoBase
    {
        public int Rate { get; set; } = 0;
        public Guid EvaluatorId { get; set; }
        public Guid BuildingId { get; set; }
    }
}