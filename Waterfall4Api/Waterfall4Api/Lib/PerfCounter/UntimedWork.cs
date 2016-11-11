using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Waterfall4Api.Contracts;

namespace Waterfall4Api.Lib.PerfCounter
{
    internal sealed class UntimedWork<TInput, TResult> : IMeasurableWork<TInput, TResult> where TResult : class
    {
        private readonly IWaterfallWork<TInput, TResult> _workItem;

        internal UntimedWork(IWaterfallWork<TInput, TResult> workItem)
        {
            _workItem = workItem;
        }

        public void Init(IUnityContainer container)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteWorkAsync(TInput input, TResult result, CancellationToken token,
            bool configAwait = false)
        {
            return _workItem.ExecuteWorkAsync(input, result, token, configAwait);
        }
    }
}