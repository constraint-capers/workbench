﻿using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;
using Workbench.Core.Models;

namespace Workbench.Core.Solvers
{
    /// <summary>
    /// Extract the snapshot from the or-tools solution.
    /// </summary>
    internal class OrSnapshotExtractor
    {
        private readonly OrToolsCache orToolsCache;
        private readonly OrValueMapper valueMapper;
        private readonly SolutionSnapshot snapshot;

        /// <summary>
        /// Initialize a snapshot extractor with an or-tools cache and a value mapper.
        /// </summary>
        /// <param name="theOrToolsCache">or-tools cache.</param>
        /// <param name="theValueMapper">Value mapper between domain and solver values.</param>
        internal OrSnapshotExtractor(OrToolsCache theOrToolsCache, OrValueMapper theValueMapper)
        {
            this.orToolsCache = theOrToolsCache;
            this.valueMapper = theValueMapper;
            this.snapshot = new SolutionSnapshot();
        }

        /// <summary>
        /// Extract the snapshot from the solution collector.
        /// </summary>
        /// <param name="theSolutionCollector">Or-tools solution collector.</param>
        /// <returns>Solution snapshot model.</returns>
        internal SolutionSnapshot ExtractValuesFrom(SolutionCollector theSolutionCollector)
        {
            ExtractSingletonLabelsFrom(theSolutionCollector);
            ExtractAggregateLabelsFrom(theSolutionCollector);
            ExtractBucketLabelsFrom(theSolutionCollector);

            return this.snapshot;
        }

        private void ExtractAggregateLabelsFrom(SolutionCollector theSolutionCollector)
        {
            foreach (var aggregateTuple in this.orToolsCache.AggregateVariableMap)
            {
                var newValueBindings = new List<ValueModel>();
                var orVariables = aggregateTuple.Value.Item2;
                foreach (var orVariable in orVariables)
                {
                    var solverValue = theSolutionCollector.Value(0, orVariable);
                    var modelValue = ConvertSolverValueToModel(aggregateTuple.Value.Item1, solverValue);
                    newValueBindings.Add(new ValueModel(modelValue));
                }
                var newCompoundLabel = new AggregateVariableLabelModel(aggregateTuple.Value.Item1, newValueBindings);
                this.snapshot.AddAggregateLabel(newCompoundLabel);
            }
        }

        private void ExtractSingletonLabelsFrom(SolutionCollector theSolutionCollector)
        {
            foreach (var variableTuple in this.orToolsCache.SingletonVariableMap)
            {
                var solverValue = theSolutionCollector.Value(0, variableTuple.Value.Item2);
                var modelValue = ConvertSolverValueToModel(variableTuple.Value.Item1, solverValue);
                var newValue = new SingletonVariableLabelModel(variableTuple.Value.Item1, new ValueModel(modelValue));
                this.snapshot.AddSingletonLabel(newValue);
            }
        }

        private void ExtractBucketLabelsFrom(SolutionCollector solutionCollector)
        {
            foreach (var bucketTuple in this.orToolsCache.BucketMap)
            {
                ExtractBucketLabelFrom(solutionCollector, bucketTuple.Value);
            }
        }

        private void ExtractBucketLabelFrom(SolutionCollector solutionCollector, OrBucketVariableMap bucketVariableMap)
        {
            var bundleLabels = new List<BundleLabelModel>();
            foreach (var bundleMap in bucketVariableMap.GetBundleMaps())
            {
                var variableLabels = new List<SingletonVariableLabelModel>();
                foreach (var variableMap in bundleMap.GetVariableMaps())
                {
                    var solverValue = solutionCollector.Value(0, variableMap.SolverVariable);
                    var modelValue = ConvertSolverValueToModel(bucketVariableMap.Bucket, solverValue);
                    variableLabels.Add(new SingletonVariableLabelModel(variableMap.ModelVariable, new ValueModel(modelValue)));
                }
                bundleLabels.Add(new BundleLabelModel(bucketVariableMap.Bucket.Bundle, variableLabels));
            }

            var bucketLabel = new BucketLabelModel(bucketVariableMap.Bucket, bundleLabels);
            this.snapshot.AddBucketLabel(bucketLabel);
        }

        private object ConvertSolverValueToModel(BucketVariableModel bucket, long solverValue)
        {
            var variableDomainValue = this.valueMapper.GetDomainValueFor(bucket);
            return variableDomainValue.MapFrom(solverValue);
        }

        private object ConvertSolverValueToModel(SingletonVariableModel theVariable, long solverValue)
        {
            var variableDomainValue = this.valueMapper.GetDomainValueFor(theVariable);
            return variableDomainValue.MapFrom(solverValue);
        }

        private object ConvertSolverValueToModel(AggregateVariableModel theVariable, long solverValue)
        {
            var variableDomainValue = this.valueMapper.GetDomainValueFor(theVariable);
            return variableDomainValue.MapFrom(solverValue);
        }
    }
}
