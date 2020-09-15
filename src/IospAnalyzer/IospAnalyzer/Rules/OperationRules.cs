using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Iosp
{
  public static class OperationRules
  {

    public static Diagnostic Analyse(IOperation op)
    {
      switch (op.Kind)
      {
        case OperationKind.Invocation:
          return OperationRules.Invocation((IInvocationOperation)op);
      }

      return null;
    }

    private static Diagnostic Invocation(IInvocationOperation op)
    {
      var methodCategory = MethodCategorizer.CategorizeMethod(op.TargetMethod, op.SemanticModel);

      if (methodCategory == MethodCategory.Integration)
      {
        return OperationDiagnostics.MustNotCallIntegration(op.Syntax.GetLocation());
      }

      if (methodCategory == MethodCategory.Operation)
      {
        return OperationDiagnostics.MustNotCallOperations(op.Syntax.GetLocation());
      }

      return null;
    }

  }
}