using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Waterfall4Api.Contracts;

namespace Waterfall4Api.Lib.PerfCounter
{
    internal sealed class TimedWork<TInput, TResult> : IMeasurableWork<TInput, TResult> where TResult : class
    {
        private readonly IWaterfallWork<TInput, TResult> _workItem;
        private PerfData _perfData = new PerfData();

        internal TimedWork(IWaterfallWork<TInput, TResult> workItem)
        {
            _workItem = workItem;
        }

        public void Init(IUnityContainer container)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ExecuteWorkAsync(TInput input, TResult result, CancellationToken token, bool configAwait = false)
        {
            var currTicks = DateTime.UtcNow.Ticks;
            var workResult =
                await _workItem.ExecuteWorkAsync(input, result, token, configAwait).ConfigureAwait(configAwait);
            AddToStats(DateTime.UtcNow.Ticks-currTicks);
            return workResult;
        }

        private void AddToStats(long workTicks)
        {
            if (workTicks<=0) return;

            var perfData = Volatile.Read(ref _perfData);
            Interlocked.Add(ref perfData.TimeTicks, workTicks);

            //todo: currently sampled freq is fixed to 10.
            if (Interlocked.Increment(ref perfData.Count)%10 != 1) return;

            Interlocked.Exchange(ref perfData.SampledMaxTimeTicks,
                Math.Max(workTicks, Volatile.Read(ref perfData.SampledMaxTimeTicks)));
            Interlocked.Exchange(ref perfData.SampledMinTimeTicks,
                Math.Min(workTicks, Volatile.Read(ref perfData.SampledMinTimeTicks)));
        }
    }
}