using Microsoft.CodeAnalysis;

namespace Revrs.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ReverserGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}