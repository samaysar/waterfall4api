using System;

namespace Waterfall4Api.Contracts
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class WaterfallMapAttribute : Attribute
    {
        public WaterfallMapAttribute(Type startUpWork)
        {
            Type = startUpWork;
        }

        public Type Type { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class WaterfallWorkAttribute : Attribute
    {
        public WaterfallWorkAttribute(Type waterfallWork)
        {
            WorkType = waterfallWork;
        }

        public Type WorkType { get; private set; }
    }
}