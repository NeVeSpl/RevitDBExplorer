using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class SymbolFinder : Microsoft.CodeAnalysis.SymbolVisitor
    {      
        private readonly string assemblyName;
        public LambdaToBe Result { get; private set; }


        public SymbolFinder(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }


        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            if (symbol.ContainingAssembly != null && symbol.ContainingAssembly.Name != assemblyName)
            {
                return;
            }         

            foreach (var memeber in symbol.GetMembers().OfType<INamespaceSymbol>())
            {
                memeber.Accept(this);
            }  
           
            foreach (var memeber in symbol.GetMembers().OfType<ITypeSymbol>())
            {
                memeber.Accept(this);
            }            
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.ContainingAssembly != null && symbol.ContainingAssembly.Name != assemblyName)
            {
                return;
            }           

            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.Name.StartsWith("<")) continue;
                if (method.MethodKind != MethodKind.Ordinary) continue;

                Result = new LambdaToBe(method);
                return;
            }
        }
    }

    internal class LambdaToBe
    {
        private static readonly SymbolDisplayFormat symbolDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining).WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.None);


        public string Name { get; }
        public string ReturnType { get; }
        public bool IsReturnTypeEnumerable { get; }
        public bool IsReturnTypeVoid { get; }
        public IEnumerable<string> Parameters { get; }


        public LambdaToBe(IMethodSymbol method)
        {
            Name = method.Name;
            ReturnType = method.ReturnType.ToDisplayString(symbolDisplayFormat);
            IsReturnTypeEnumerable = method.ReturnType.AllInterfaces.Any(x => x.ToString() == "System.Collections.IEnumerable");
            IsReturnTypeVoid = method.ReturnType.SpecialType == SpecialType.System_Void;
            Parameters = method.Parameters.Select(x => x.Type.ToDisplayString(symbolDisplayFormat)).ToArray();
        }
    }
}