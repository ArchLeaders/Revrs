using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Revrs.SourceGenerator.Builders;

public class ReverserBuilder(SourceProductionContext context, Compilation compilation)
{
    private const string StructReverserInterfaceTypeName = "Revrs.IStructReverser";
    private const string DoNotRemoveAttributeTypeName = "Revrs.Attributes.DoNotReverseAttribute";
    private const string StructLayoutAttributeTypeName = "System.Runtime.InteropServices.StructLayoutAttribute";
    private const string FieldOffsetAttributeTypeName = "System.Runtime.InteropServices.FieldOffsetAttribute";

    private readonly SourceProductionContext _context = context;
    private readonly ISymbol _reverserAttribute = compilation.GetTypeByMetadataName(ReverserGenerator.AttributeTypeName)!;
    private readonly ISymbol _structReverserInterface = compilation.GetTypeByMetadataName(StructReverserInterfaceTypeName)!;
    private readonly ISymbol _doNotReverseAttribute = compilation.GetTypeByMetadataName(DoNotRemoveAttributeTypeName)!;
    private readonly ISymbol _structLayoutAttribute = compilation.GetTypeByMetadataName(StructLayoutAttributeTypeName)!;
    private readonly ISymbol _fieldOffsetAttribute = compilation.GetTypeByMetadataName(FieldOffsetAttributeTypeName)!;

    public void GenerateReverser(INamedTypeSymbol symbol)
    {
        if (!symbol.IsUnmanagedType) {
            _context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "REVRS-0001",
                    "Invalid reversable type",
                    "Types annotated with ReversableAttribute must be unmanaged structures",
                    "Reverser Generator",
                    DiagnosticSeverity.Error, true),
                symbol.Locations.FirstOrDefault())
            );
            return;
        }

        IEnumerable<IFieldSymbol> fields = symbol
            .GetMembers()
            .Select(member => (member as IFieldSymbol)!)
            .Where(member => member is not null);

        int packSize = GetPackSize(symbol);
        StringBuilder reverseCode = new();

        int currentBytePosition = 0;
        foreach (IFieldSymbol field in fields) {
            GenerateRevrsForField(reverseCode, field, packSize, ref currentBytePosition);
        }

        string code = $$"""
            // <auto-generated/>

            #nullable enable

            namespace {{symbol.ContainingNamespace}};

            public partial struct {{symbol.Name}} : global::Revrs.IStructReverser
            {
                public static void Reverse(in global::System.Span<global::System.Byte> slice)
                {{{reverseCode}}
                }
            }
            """;

        _context.AddSource(
            $"{symbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.MinimallyQualifiedFormat)}.g.cs",
            code);
    }

    private void GenerateRevrsForField(StringBuilder sb, IFieldSymbol field, int packSize, ref int pos)
    {
        Span<RevrsSlice> fieldSizes = GetFieldSizes(field, packSize);
        int totalFieldSize = Sum(fieldSizes);

        ImmutableArray<AttributeData> fieldAttributes = field.GetAttributes();

        if (fieldAttributes.FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, _fieldOffsetAttribute))
                is AttributeData fieldOffset
            && fieldOffset.ConstructorArguments[0].Value is int attributeFixedOffset) {
            pos = attributeFixedOffset;
        }

        if (fieldAttributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, _doNotReverseAttribute))) {
            pos += totalFieldSize;
            return;
        }

        ITypeSymbol fieldType = field.Type;
        ImmutableArray<AttributeData> fieldSymbolAttributes = field.Type.GetAttributes();

        bool isFieldTypeReverser =
            fieldSymbolAttributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, _reverserAttribute)) ||
            fieldType.AllInterfaces.Any(x => SymbolEqualityComparer.Default.Equals(x, _structReverserInterface));

        if (isFieldTypeReverser) {
            sb.Append($"""
                
                        {field.Type.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)}.Reverse(slice[{pos}..{pos += totalFieldSize}]);
                """);
            return;
        }

        foreach ((int fieldSize, int pack) in fieldSizes) {
            pos += AlignUp(pos, pack);
            sb.Append($"""
                
                        slice[{pos}..{pos += fieldSize}].Reverse();
                """);
        }
    }

    private int GetPackSize(INamedTypeSymbol symbol)
    {
        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        int packSize = 0;

        if (attributes.FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, _structLayoutAttribute))
                is AttributeData structLayout
            && structLayout.NamedArguments.FirstOrDefault(x => x.Key == "Pack").Value.Value is int attributePackSize) {
            packSize = attributePackSize;
        }

        return packSize;
    }

    private RevrsSlice[] GetFieldSizes(IFieldSymbol field, int packSize)
    {
        if (field.Type is IPointerTypeSymbol ptr) {
            RevrsSlice[] ptrTargetTypeSizes = GetTypeSizes((INamedTypeSymbol)ptr.PointedAtType, packSize);
            return Enumerable.Range(0, field.FixedSize)
                .SelectMany(_ => ptrTargetTypeSizes)
                .ToArray();
        }
        
        var symbol = (INamedTypeSymbol)field.Type;

        if (!symbol.IsUnmanagedType || !symbol.IsValueType) {
            _context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "REVRS-0002",
                    "Invalid reversable field type",
                    "Field types must be unmanaged structures (value types)",
                    "Reverser Generator",
                    DiagnosticSeverity.Error, true),
                field.Locations.FirstOrDefault())
            );

            return [];
        }
        
        return GetTypeSizes(symbol, packSize);
    }
    
    private RevrsSlice[] GetTypeSizes(INamedTypeSymbol symbol, int parentPackSize)
    {
        if (symbol.TypeKind is TypeKind.Enum && symbol.EnumUnderlyingType is INamedTypeSymbol enumUnderlyingType) {
            return [
                (GetSpecialTypeSize(enumUnderlyingType.SpecialType), parentPackSize)
            ];
        }

        if (GetSpecialTypeSize(symbol.SpecialType) is var size && size != -1) {
            return [(size, parentPackSize)];
        }

        int packSize = GetPackSize(symbol);
        IEnumerable<IFieldSymbol> fields = symbol
            .GetMembers()
            .Select(member => (member as IFieldSymbol)!)
            .Where(member => member is not null);
        
        return fields.SelectMany(x => GetFieldSizes(x, packSize)).ToArray();
    }

    private int GetSpecialTypeSize(SpecialType specialType)
    {
        return specialType switch {
            SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Boolean => 1,
            SpecialType.System_Char or SpecialType.System_Int16 or SpecialType.System_UInt16 => 2,
            SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Single => 4,
            SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Double
                or SpecialType.System_DateTime => 8,
            SpecialType.System_IntPtr or SpecialType.System_UIntPtr => IntPtr.Size,
            SpecialType.System_Decimal => 24,
            _ => -1
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AlignUp(int pos, int packSize)
    {
        if (packSize is 0) return 0;
        
        return (packSize - pos % packSize) % packSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Sum(Span<RevrsSlice> slices)
    {
        int totalSize = 0;
        foreach ((int size, _) in slices) totalSize += size;
        return totalSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private readonly struct RevrsSlice(int size, int pack)
    {
        public readonly int Size = size;
        public readonly int Pack = pack;
        
        public static implicit operator RevrsSlice((int, int) tuple) => new RevrsSlice(tuple.Item1, tuple.Item2);

        public void Deconstruct(out int size, out int pack)
        {
            size = Size;
            pack = Pack;
        }
    }
}