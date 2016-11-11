using System;

namespace Waterfall4Api.Contracts
{
    public enum WaterfallErrorType
    {
        Unknown = 0,
        MissingWaterfallMapAttributeAttribute,
        MissingWaterfallWorkAttributeAttribute,
        WaterfallAttributeSuppliedTypeIsNull,
        WaterfallWorkIsNotDerivedCorrectly,
        MapMemberIsNotIntType,
        MapMemberMustBeReadOnly,
        UnknownTypeOfWaterfallMap,
        WaterfallWorkDoesNotDefineDefaultParameterLessCtor,
        TwoOrMoreWaterfallWorksReturnsSameUniqueIdentifier,
        WaterfallServiceDisposedOrCancellationDemanded,
        EnumNotInheritedFromInt,
        EnumDoesNotDefineEndOfWaterfallAsMinusOne,
        EnumValuesAreNotContinuouslyIncreasing,
        WaterfallExceededMaxIterations,
        WaterfallNotDefined,
        RedundancyDetected
    }

    public sealed class WaterfallException : Exception
    {
        internal WaterfallException(WaterfallErrorType errorType, string message, Exception inner = null)
            : base($"Reason:{errorType}. {message}", inner)
        {
            ErrorType = errorType;
        }

        public WaterfallErrorType ErrorType { get; private set; }
    }
}