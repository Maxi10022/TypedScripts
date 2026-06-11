using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFacts;

namespace TypedScripts.Scripts;

public class ScriptArgument(int position, string type, string name, bool required, string? @default)
{
    public int Position { get; } = position;
    
    public override string ToString()
    {
        // TODO fix spacing
        var syntaxType = GetTypeSyntax();
        
        var parameter = Parameter(Identifier(name))
            .WithType(syntaxType);

        if (@default is not null)
        {
            // TODO look into properly setting up the default expression, currently throws
            var kind = GetKeywordKind(type);
            var expression = LiteralExpression(kind, token: Literal(@default));
            parameter = parameter.WithDefault(EqualsValueClause(expression));
        }
        
        return parameter.ToFullString();
    }
    
    private TypeSyntax GetTypeSyntax() =>
        required 
            ? ParseTypeName(type) 
            : NullableType(ParseTypeName(type));
}