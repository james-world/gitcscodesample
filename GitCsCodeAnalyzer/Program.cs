using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GitCsCodeAnalyzer
{
    class Program
    {

        static void Main(string[] args)
        {
            var repo = new GitRepository(new DirectoryInfo(@"D:\Repos\Thomas"));
            ListNewMethods(repo);
        }

        private static IEnumerable<(string Name, int Count)> GetMethodCounts(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);

            return
                from node in tree.GetRoot().DescendantNodes()
                where node.IsKind(SyntaxKind.MethodDeclaration)
                let methodName = ((MethodDeclarationSyntax) node).Identifier.ValueText
                group node by methodName
                into g
                orderby g.Key
                select (Name: g.Key, Count: g.Count());
        }


        private static void ListNewMethods(GitRepository repo)
        {
            var files = repo.GetCsFilesNewOrChangedAtHead().First();

            var oldMethodCounts = GetMethodCounts(files.OldFile);
            var newMethodCounts = GetMethodCounts(files.NewFile);

            var diffVariableCounts =
                from newMethod in newMethodCounts
                join oldMethod in oldMethodCounts
                on newMethod.Name equals oldMethod.Name into diffs
                from diff in diffs.DefaultIfEmpty()
                let delta = (Name: newMethod.Name, Count: newMethod.Count - diff.Count)
                where delta.Count > 0
                select delta.Name;

            foreach (var d in diffVariableCounts)
            {
                Console.WriteLine(d);
            }
        }
    }
}
