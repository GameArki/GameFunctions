using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GameFunctions.CSharpGenerator {

    internal static class RoslynBaseTypeExtention {

        public static string GetName(this BaseTypeDeclarationSyntax method) {
            return method.Identifier.ValueText;
        }

    }

}