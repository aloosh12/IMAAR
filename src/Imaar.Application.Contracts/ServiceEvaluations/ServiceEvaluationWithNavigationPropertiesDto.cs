using Imaar.UserProfiles;
using Imaar.ImaarServices;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationWithNavigationPropertiesDtoBase
    {
        public ServiceEvaluationDto ServiceEvaluation { get; set; } = null!;

        public UserProfileDto Evaluator { get; set; } = null!;
        public ImaarServiceDto ImaarService { get; set; } = null!;

    }
}