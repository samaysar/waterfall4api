using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Waterfall4Api.Contracts;

namespace Waterfall4Api.Lib
{
    internal sealed class Waterfall<TMap, TInput, TResult> : IWaterfall<TInput, TResult>
        where TResult : class where TMap : MapBase
    {
        private readonly int _maxCalls;
        private readonly IWaterfallWork<TInput, TResult> _waterFallRoot;
        private readonly Dictionary<int, IWaterfallWork<TInput, TResult>> _workBranches;

        public Waterfall(IUnityContainer container, bool measurePerf)
        {
            _waterFallRoot = CreateStartUpInstance(container);
            _workBranches = CreateWorkItems(container, _waterFallRoot.GetType());
            _maxCalls = _workBranches.Count+1;
        }

        public Task ExecuteAsync(TInput input, TResult result, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        #region Private Methods

        private static IWaterfallWork<TInput, TResult> CreateStartUpInstance(IUnityContainer container)
        {
            var mapType = typeof(TMap);
            if (!mapType.IsClass)
                throw new WaterfallException(WaterfallErrorType.UnknownTypeOfWaterfallMap,
                    $"Waterfall Map must be a class. Error for {mapType.FullName}");
            var attribute =
                Attribute.GetCustomAttribute(mapType, typeof(WaterfallMapAttribute)) as WaterfallMapAttribute;
            if (attribute == null)
                throw new WaterfallException(WaterfallErrorType.MissingWaterfallMapAttributeAttribute,
                    $"Map ({mapType.FullName}) missing WaterfallAttribute.");

            if (attribute.Type == null)
                throw new WaterfallException(WaterfallErrorType.WaterfallAttributeSuppliedTypeIsNull,
                    $"Null type for StartUpWork. Map ({mapType.FullName}) on WaterfallAttribute.");

            var instance = container.Resolve(attribute.Type) as IWaterfallWork<TInput, TResult>;

            if (instance == null)
                throw new WaterfallException(WaterfallErrorType.WaterfallWorkIsNotDerivedCorrectly,
                    $"StartUpWork ({attribute.Type.FullName}) instance EITHER not registered in the "+
                    $"container OR not derived from {typeof (IWaterfallWork<TInput, TResult>).Name}.");

            instance.Init(container);
            return instance;
        }

        private static Dictionary<int, IWaterfallWork<TInput, TResult>> CreateWorkItems(IUnityContainer container,
            Type startUpType)
        {
            var workItems = new Dictionary<int, IWaterfallWork<TInput, TResult>>();

            var mapType = typeof (TMap);
            var fields = mapType.GetFields(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Public);
            if (fields.Length == 0) return workItems;

            var typeSet = new HashSet<Type> {startUpType};
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType != typeof (int))
                    throw new WaterfallException(WaterfallErrorType.MapMemberIsNotIntType,
                        $"Map ({mapType.FullName}) is not allowed to contain non-int public static members ({fieldInfo.Name}).");

                if (!fieldInfo.IsInitOnly)
                    throw new WaterfallException(WaterfallErrorType.MapMemberMustBeReadOnly,
                        $"Map ({mapType.FullName}) member ({fieldInfo.Name}) must be static read-only.");

                var attribute =
                    Attribute.GetCustomAttribute(fieldInfo, typeof (WaterfallWorkAttribute)) as WaterfallWorkAttribute;

                if (attribute == null)
                    throw new WaterfallException(WaterfallErrorType.MissingWaterfallWorkAttributeAttribute,
                        $"Map ({mapType.FullName}) member ({fieldInfo.Name}) must be decorated with {nameof(WaterfallWorkAttribute)}.");

                if (attribute.WorkType == null)
                    throw new WaterfallException(WaterfallErrorType.WaterfallAttributeSuppliedTypeIsNull,
                        $"Map ({mapType.FullName}) member ({fieldInfo.Name}) supplies null type in {nameof(WaterfallWorkAttribute)}.");

                if (!typeSet.Add(attribute.WorkType))
                    throw new WaterfallException(WaterfallErrorType.RedundancyDetected,
                        $"WorkType ({attribute.WorkType.FullName}) defined on at least 2 map members (map:{mapType.FullName}).");

                var fieldValue = (int)fieldInfo.GetValue(null);
                IWaterfallWork<TInput, TResult> outVal;
                if (workItems.TryGetValue(fieldValue, out outVal))
                    throw new WaterfallException(WaterfallErrorType.TwoOrMoreWaterfallWorksReturnsSameUniqueIdentifier,
                        $"Map ({mapType.FullName}) member ({fieldInfo.Name}) has exact same int-value (value:{fieldValue}) "+
                        $"as another member defining work-item type as {outVal.GetType().FullName}.");

                var instance = container.Resolve(attribute.WorkType) as IWaterfallWork<TInput, TResult>;
                if (instance == null)
                    throw new WaterfallException(WaterfallErrorType.WaterfallWorkIsNotDerivedCorrectly,
                        $"StartUpWork ({attribute.WorkType.FullName}) instance EITHER not registered in the "+
                        $"container OR not derived from {typeof (IWaterfallWork<TInput, TResult>).Name}.");

                instance.Init(container);
                workItems.Add(fieldValue, instance);
            }
            return workItems;
        }

        #endregion
    }
}