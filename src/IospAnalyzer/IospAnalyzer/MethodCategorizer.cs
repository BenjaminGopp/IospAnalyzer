using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Iosp
{
  static class MethodCategorizer
  {

    public static MethodCategory CategorizeMethod(IMethodSymbol method, SemanticModel semanticModel)
    {

      foreach (var attribute in method.GetAttributes())
      {

        if (IsIntegrationAttribute(semanticModel, attribute.AttributeClass))
          return MethodCategory.Integration;

        if (IsOperationAttribute(semanticModel, attribute.AttributeClass))
          return MethodCategory.Operation;

      }

      return MethodCategory.None;
    }

    public static bool IsIntegrationAttribute(SemanticModel semanticModel, INamedTypeSymbol attributeType)
    {
      var integrationAttributeType = semanticModel.Compilation.GetTypeByMetadataName("Iosp.IntegrationAttribute");
      bool isIntegrationAttribute = CompareSymbolEquality(attributeType, integrationAttributeType);
      return isIntegrationAttribute;
    }

    public static bool IsOperationAttribute(SemanticModel semanticModel, INamedTypeSymbol attributeType)
    {
      var operationAttributeType = semanticModel.Compilation.GetTypeByMetadataName("Iosp.OperationAttribute");
      bool isOperationAttribute = CompareSymbolEquality(attributeType, operationAttributeType);
      return isOperationAttribute;
    }

    private static bool CompareSymbolEquality(INamedTypeSymbol namedType, INamedTypeSymbol otherNamedType)
    {
      //return SymbolEqualityComparer.Default.Equals(attributeType, operationAttributeType); //ab roslyn 3.3.1

      return namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
             otherNamedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
  }
}
