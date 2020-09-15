using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Iosp
{

  public static class IntegrationRules
  {

    public static Diagnostic Analyse(IOperation op)
    {
      switch (op.Kind)
      {
        case OperationKind.Loop:
          return IntegrationRules.Loop((ILoopOperation)op);
        case OperationKind.Conditional:
          return IntegrationRules.Conditional((IConditionalOperation)op);
        case OperationKind.Switch:
          return IntegrationRules.Switch((ISwitchOperation)op);
        case OperationKind.Invocation:
          return IntegrationRules.Invocation((IInvocationOperation)op);
      }

      return null;
    }

    public static Diagnostic Invocation(IInvocationOperation op)
    {
      var methodCategory = MethodCategorizer.CategorizeMethod(op.TargetMethod,op.SemanticModel);

      if (methodCategory != MethodCategory.Integration && methodCategory != MethodCategory.Operation)
      {
        return IntegrationDiagnostics.MustNotCallApi(op.Syntax.GetLocation());
      }

      return null;

    }

    public static Diagnostic Loop(ILoopOperation op)
    {
      switch (op.LoopKind)
      {
        case LoopKind.None:
          return null;
        case LoopKind.While:
          return While((IWhileLoopOperation)op);
        case LoopKind.For:
          return For((IForLoopOperation)op);
        case LoopKind.ForTo:
          return null;
        case LoopKind.ForEach:
          return ForEach((IForEachLoopOperation)op);
      }

      return null;
    }

    public static Diagnostic While(IWhileLoopOperation op)
    {
      return IntegrationDiagnostics.WhileLoopIsNotAllowed(op.Syntax.GetLocation());
    }

    public static Diagnostic For(IForLoopOperation op)
    {
      return IntegrationDiagnostics.ForLoopIsNotAllowed(op.Syntax.GetLocation());
    }

    public static Diagnostic ForEach(IForEachLoopOperation op)
    {
      if (op.Collection.Kind != OperationKind.Invocation)
      {
        return  IntegrationDiagnostics.CollectionMustBeAMethodCall(op.Syntax.GetLocation());
      }

      var invocation = (IInvocationOperation)op.Collection;

      var methodCategory = MethodCategorizer.CategorizeMethod(invocation.TargetMethod, op.SemanticModel);
      if (methodCategory == MethodCategory.Operation)
      {
        return IntegrationDiagnostics.CollectionMustCallToOperation(op.Syntax.GetLocation());
      }

      return null;
    }

    public static Diagnostic Conditional(IConditionalOperation op)
    {
      if (op.Condition.Kind != OperationKind.Invocation)
      {
        return IntegrationDiagnostics.ConditionalMustBeAMethodCall(op.Syntax.GetLocation());
      }

      var invocation = (IInvocationOperation) op.Condition;

      var methodCategory = MethodCategorizer.CategorizeMethod(invocation.TargetMethod,op.SemanticModel);
      if (methodCategory != MethodCategory.Operation)
      {
        return IntegrationDiagnostics.ConditionalMustCallToOperation(op.Syntax.GetLocation());
      }

      return null;
    }

    public static Diagnostic Switch(ISwitchOperation op)
    {
      var retunType = GetReturnType(op);

      if (!IsEnum(op.SemanticModel, retunType))
      {
        return IntegrationDiagnostics.SwitchValueMustBeOfTypeEnum(op.Syntax.GetLocation());
      }

      return null;
    }

    private static ITypeSymbol GetReturnType(ISwitchOperation op)
    {
      switch (op.Value.Kind)
      {
        case OperationKind.Invocation:
          return ((IInvocationOperation)op.Value).TargetMethod.ReturnType;
      }

      return null;
    }

    private static bool IsEnum(SemanticModel semanticModel, ITypeSymbol type)
    {
      var EnumType = semanticModel.Compilation.GetTypeByMetadataName("System.Enum");
      bool isEnum = SymbolEqualityComparer.Default.Equals(type, EnumType);
      return isEnum;
    }


  }
}