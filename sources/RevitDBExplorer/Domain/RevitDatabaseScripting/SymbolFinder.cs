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
       
        public string Name { get; }
        public RoslynTypeInfo ReturnType { get; }      
        public IEnumerable<RoslynTypeInfo> Parameters { get; }


        public LambdaToBe(IMethodSymbol method)
        {
            Name = method.Name;
            ReturnType = new RoslynTypeInfo(method.ReturnType);          
            Parameters = method.Parameters.Select(x => new RoslynTypeInfo(x.Type)).ToArray();
        }
    }

    public class RoslynTypeInfo
    {
        private static readonly SymbolDisplayFormat symbolDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining).WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.None);


        public string Name { get; }
        public string FirstTypeArgumentName { get; }
        public bool IsEnumerable { get; }
        public bool IsVoid { get; }


        public RoslynTypeInfo(ITypeSymbol symbol)
        {
            Name = symbol.ToDisplayString(symbolDisplayFormat);
            IsEnumerable = symbol.AllInterfaces.Any(x => x.ToString() == "System.Collections.IEnumerable");
            IsVoid = symbol.SpecialType == SpecialType.System_Void;
            if (IsEnumerable && (symbol is INamedTypeSymbol namedSymbol))
            {
                FirstTypeArgumentName = namedSymbol.TypeArguments.FirstOrDefault()?.ToDisplayString(symbolDisplayFormat);
            }
        }
    }
}