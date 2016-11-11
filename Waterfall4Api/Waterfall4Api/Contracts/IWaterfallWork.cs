using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Waterfall4Api.Contracts
{
    public interface IWaterfallWork<in TInput, in TResult> where TResult : class
    {
        void Init(IUnityContainer container);
        Task<int> ExecuteWorkAsync(TInput input, TResult result, CancellationToken token, bool configAwait = false);
    }

    internal interface IMeasurableWork<in TInput, in TResult> : IWaterfallWork<TInput, TResult> where TResult : class
    {
    }
}