namespace Waterfall4Api.Lib.PerfCounter
{
    public sealed class PerfData
    {
        public long TimeTicks;
        public long Count;
        public long SampledMaxTimeTicks;
        public long SampledMinTimeTicks;

        public PerfData()
        {
            SampledMinTimeTicks = long.MaxValue;
        }
    }
}