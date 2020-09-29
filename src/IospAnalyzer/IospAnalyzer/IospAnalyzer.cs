using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Iosp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace Iosp
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class IospAnalyzer : DiagnosticAnalyzer
  {

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return new[]
        {
          IntegrationDiagnostics.MustNotCallApiDiagnostic,
          IntegrationDiagnostics.MustNotUseForLoopDiagnostic,
          IntegrationDiagnostics.MustNotUseWhileLoopDiagnostic,
          OperationDiagnostics.MustNotCallIntegrationDiagnostic,
          OperationDiagnostics.MustNotCallOperationsDiagnostic,
        }.ToImmutableArray();
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      
      var kinds = new[]
      {
        OperationKind.Loop,
        OperationKind.Conditional,
        OperationKind.Switch,
        OperationKind.Invocation
      };

      context.RegisterOperationAction(AnalyseOperation, kinds);
    }

    private static void AnalyseOperation(OperationAnalysisContext context)
    {
      var op = context.Operation;

      var method = GetDeclaringMethod(op);
      if (method == null)
        return;

      var symbol = op.SemanticModel.GetDeclaredSymbol(method);
        
      if (symbol == null)
        return;
      
      var methodCategory = MethodCategorizer.CategorizeMethod(symbol, op.SemanticModel);

      var diagnostic = DiagnoseMethod(methodCategory, op);

      if (diagnostic != null)
      {
        context.ReportDiagnostic(diagnostic);
      }
    }

    private static Diagnostic DiagnoseMethod(MethodCategory methodCategory, IOperation op)
    {
      Diagnostic diagnostic = null;
      switch (methodCategory)
      {
        case MethodCategory.None:
          break;
        case MethodCategory.Integration:
          diagnostic = IntegrationRules.Analyse(op);
          break;
        case MethodCategory.Operation:
          diagnostic = OperationRules.Analyse(op);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return diagnostic;
    }

      public static MethodDeclarationSyntax GetDeclaringMethod(IOperation op)
      {
        var method = op.Syntax.Ancestors().FirstOrDefault(x => x.Kind() == SyntaxKind.MethodDeclaration);

        return (MethodDeclarationSyntax) method;
      }

  }
}
