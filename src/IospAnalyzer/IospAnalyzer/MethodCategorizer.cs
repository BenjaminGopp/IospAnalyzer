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
      return attributeType.Name == "Integration";
      //var integrationAttributeType = semanticModel.Compilation.GetTypeByMetadataName("Iosp.IntegrationAttribute");
      //bool isIntegrationAttribute = SymbolEqualityComparer.Default.Equals(attributeType, integrationAttributeType);
      //return isIntegrationAttribute;
    }

    public static bool IsOperationAttribute(SemanticModel semanticModel, INamedTypeSymbol attributeType)
    {
      return attributeType.Name == "Operation";
      var operationAttributeType = semanticModel.Compilation.GetTypeByMetadataName("Iosp.OperationAttribute");
      bool isIntegrationAttribute = SymbolEqualityComparer.Default.Equals(attributeType, operationAttributeType);
      return isIntegrationAttribute;
    }
  }
}
