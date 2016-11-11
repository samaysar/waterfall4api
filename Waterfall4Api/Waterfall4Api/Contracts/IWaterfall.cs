using System.Threading;
using System.Threading.Tasks;

namespace Waterfall4Api.Contracts
{
    public interface IWaterfall<in TInput, in TResult> where TResult : class
    {
        Task ExecuteAsync(TInput input, TResult result, CancellationToken token);
    }
}