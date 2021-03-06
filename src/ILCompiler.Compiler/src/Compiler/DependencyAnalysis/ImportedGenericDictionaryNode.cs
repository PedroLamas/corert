﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.TypeSystem;

namespace ILCompiler.DependencyAnalysis
{
    public sealed class ImportedMethodGenericDictionaryNode : ExternSymbolNode
    {
        private MethodDesc _owningMethod;

        public ImportedMethodGenericDictionaryNode(NodeFactory factory, MethodDesc owningMethod)
            : base("__imp_" + MethodGenericDictionaryNode.GetMangledName(factory.NameMangler, owningMethod))
        {
            _owningMethod = owningMethod;
        }

        public override bool RepresentsIndirectionCell => true;
    }

    public sealed class ImportedTypeGenericDictionaryNode : ExternSymbolNode
    {
        private TypeDesc _owningType;

        public ImportedTypeGenericDictionaryNode(NodeFactory factory, TypeDesc owningType)
            : base("__imp_" + TypeGenericDictionaryNode.GetMangledName(factory.NameMangler, owningType))
        {
            _owningType = owningType;
        }

        public override bool RepresentsIndirectionCell => true;
    }
}