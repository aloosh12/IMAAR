using System;
using System.Threading.Tasks;

namespace Imaar.ServiceEvaluations
{
    public partial interface IServiceEvaluationsAppService
    {
        //Write your custom code here...
        Task<double> GetAverageEvaluationForServiceAsync(Guid serviceId);
    }
}