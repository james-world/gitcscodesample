using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GitCsCodeAnalyzer
{
    public class MyVisitor : CSharpSyntaxWalker
    {
        private readonly Dictionary<string, int> variableCounts = new Dictionary<string, int>();

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var variableName = node.Identifier.ValueText;

            if (!variableCounts.ContainsKey(variableName))
                variableCounts.Add(variableName, 0);

            variableCounts[variableName]++;

            base.VisitVariableDeclarator(node);
        }

        public Dictionary<string, int> VariableCounts => variableCounts;
    }
}