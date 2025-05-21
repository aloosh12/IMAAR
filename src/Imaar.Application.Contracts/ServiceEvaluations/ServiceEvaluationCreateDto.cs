using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationCreateDtoBase
    {
        public int Rate { get; set; } = 0;
        public Guid EvaluatorId { get; set; }
        public Guid ImaarServiceId { get; set; }
    }
}