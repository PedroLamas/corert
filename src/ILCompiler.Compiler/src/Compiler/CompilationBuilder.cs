﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using ILCompiler.DependencyAnalysis;
using ILCompiler.DependencyAnalysisFramework;

namespace ILCompiler
{
    public abstract class CompilationBuilder
    {
        protected readonly CompilerTypeSystemContext _context;
        protected readonly CompilationModuleGroup _compilationGroup;

        // These need to provide reasonable defaults so that the user can optionally skip
        // calling the Use/Configure methods and still get something reasonable back.
        protected Logger _logger = Logger.Null;
        private DependencyTrackingLevel _dependencyTrackingLevel = DependencyTrackingLevel.None;
        protected IEnumerable<ICompilationRootProvider> _compilationRoots = Array.Empty<ICompilationRootProvider>();
        protected OptimizationMode _optimizationMode = OptimizationMode.None;
        protected bool _generateDebugInfo = false;
        private string _metadataLogFile = null;

        public CompilationBuilder(CompilerTypeSystemContext context, CompilationModuleGroup compilationGroup)
        {
            _context = context;
            _compilationGroup = compilationGroup;
        }

        public CompilationBuilder UseLogger(Logger logger)
        {
            _logger = logger;
            return this;
        }

        public CompilationBuilder UseDependencyTracking(DependencyTrackingLevel trackingLevel)
        {
            _dependencyTrackingLevel = trackingLevel;
            return this;
        }

        public CompilationBuilder UseCompilationRoots(IEnumerable<ICompilationRootProvider> compilationRoots)
        {
            _compilationRoots = compilationRoots;
            return this;
        }

        public CompilationBuilder UseOptimizationMode(OptimizationMode mode)
        {
            _optimizationMode = mode;
            return this;
        }

        public CompilationBuilder UseMetadataLogFile(string fileName)
        {
            _metadataLogFile = fileName;
            return this;
        }

        public CompilationBuilder UseDebugInfo(bool generateDebugInfo)
        {
            _generateDebugInfo = generateDebugInfo;
            return this;
        }

        public abstract CompilationBuilder UseBackendOptions(IEnumerable<string> options);

        protected DependencyAnalyzerBase<NodeFactory> CreateDependencyGraph(NodeFactory factory)
        {
            // Choose which dependency graph implementation to use based on the amount of logging requested.
            switch (_dependencyTrackingLevel)
            {
                case DependencyTrackingLevel.None:
                    return new DependencyAnalyzer<NoLogStrategy<NodeFactory>, NodeFactory>(factory, null);

                case DependencyTrackingLevel.First:
                    return new DependencyAnalyzer<FirstMarkLogStrategy<NodeFactory>, NodeFactory>(factory, null);

                case DependencyTrackingLevel.All:
                    return new DependencyAnalyzer<FullGraphLogStrategy<NodeFactory>, NodeFactory>(factory, null);

                default:
                    throw new InvalidOperationException();
            }
        }

        protected MetadataManager CreateMetadataManager()
        {
            return new CompilerGeneratedMetadataManager(_compilationGroup, _context, _metadataLogFile);
        }

        public abstract ICompilation ToCompilation();
    }

    /// <summary>
    /// Represents the level of dependency tracking within the dependency analysis system.
    /// </summary>
    public enum DependencyTrackingLevel
    {
        /// <summary>
        /// Tracking disabled. This is the most performant and memory efficient option.
        /// </summary>
        None,

        /// <summary>
        /// The graph keeps track of the first dependency.
        /// </summary>
        First,

        /// <summary>
        /// The graph keeps track of all dependencies.
        /// </summary>
        All
    }

    /// <summary>
    /// Represents the level of optimizations performed by the compiler.
    /// </summary>
    public enum OptimizationMode
    {
        /// <summary>
        /// Do not optimize.
        /// </summary>
        None,

        /// <summary>
        /// Minimize code space.
        /// </summary>
        PreferSize,

        /// <summary>
        /// Generate blended code. (E.g. favor size for rarely executed code such as class constructors.)
        /// </summary>
        Blended,

        /// <summary>
        /// Maximize execution speed.
        /// </summary>
        PreferSpeed,
    }
}
