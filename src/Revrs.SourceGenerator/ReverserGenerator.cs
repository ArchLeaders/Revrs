using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Revrs.SourceGenerator.Builders;

namespace Revrs.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ReverserGenerator : IIncrementalGenerator
{
    internal const string AttributeTypeName = "Revrs.Attributes.ReversableAttribute";
    private const string DoNotRemoveAttributeTypeName = "Revrs.Attributes.DoNotRemoveAttribute";
    private const string StructLayoutAttributeTypeName = "System.Runtime.InteropServices.StructLayoutAttribute";
    private const string FieldOffsetAttributeTypeName = "System.Runtime.InteropServices.FieldOffsetAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<StructDeclarationSyntax> attributedClassProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(AttributeTypeName,
                predicate: (_, _) => true,
                transform: (n, _) => (StructDeclarationSyntax)n.TargetNode
            );

        IncrementalValueProvider<(Compilation Left, ImmutableArray<StructDeclarationSyntax> Right)> compilation
            = context.CompilationProvider.Combine(attributedClassProvider.Collect());

        context.RegisterSourceOutput(compilation,
            (spc, source) => { Execute(spc, source.Left, source.Right); }
        );
    }

    private void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<StructDeclarationSyntax> types)
    {
        ReverserBuilder builder = new(context, compilation);

        IEnumerable<INamedTypeSymbol> symbols = types
            .Select(x => (compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x) as INamedTypeSymbol)!)
            .Where(x => x is not null);

        foreach (INamedTypeSymbol symbol in symbols) {
            builder.GenerateReverser(symbol);
        }
    }
}