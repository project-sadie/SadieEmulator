using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sadie.Tests;

internal class MethodWalker : CSharpSyntaxWalker
{
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var lineSpan = node.GetLocation().GetLineSpan();
        var start = lineSpan.StartLinePosition.Line;
        var end = lineSpan.EndLinePosition.Line;
        var count = end - start + 1;

        if (count > TestSettings.MaxLinesForMethod)
        {
            throw new Exception($"{node.Identifier.ValueText} was {count} lines long");
        }
        
        base.VisitMethodDeclaration(node);
    }
}