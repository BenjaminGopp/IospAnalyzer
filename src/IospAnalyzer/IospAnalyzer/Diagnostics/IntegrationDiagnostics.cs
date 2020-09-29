using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Iosp
{
  internal static class IntegrationDiagnostics
  {

    internal static DiagnosticDescriptor MustNotCallApiDiagnostic = new DiagnosticDescriptor("IOSP1001", "Integration must not call API", "Integration must not call API", "Integration", DiagnosticSeverity.Error, true);
    internal static DiagnosticDescriptor MustNotUseForLoopDiagnostic = new DiagnosticDescriptor("IOSP1002", "Integration must not use for loop", "Integration must not use for loop", "Integration", DiagnosticSeverity.Error, true);
    internal static DiagnosticDescriptor MustNotUseWhileLoopDiagnostic = new DiagnosticDescriptor("IOSP1003", "Integration must not use while loop", "Integration must not use while loop", "Integration", DiagnosticSeverity.Error, true);


    public static Diagnostic MustNotCallApi(Location location)
    {
      return Diagnostic.Create(MustNotCallApiDiagnostic, location);
    }


    public static Diagnostic ForLoopIsNotAllowed(Location location)
    {
      return Diagnostic.Create(MustNotUseForLoopDiagnostic, location);
    }

    public static Diagnostic WhileLoopIsNotAllowed(Location location)
    {
      return Diagnostic.Create(MustNotUseWhileLoopDiagnostic, location);
    }

    public static Diagnostic ConditionalMustCallToOperation(Location location)
    {
      return null;
    }

    public static Diagnostic ConditionalMustBeAMethodCall(Location location)
    {
      return null;
    }

    public static Diagnostic SwitchValueMustBeOfTypeEnum(Location location)
    {
      return null;
    }

    public static Diagnostic CollectionMustBeAMethodCall(Location location)
    {
      return null;
    }

    public static Diagnostic CollectionMustCallToOperation(Location location)
    {
      return null;
    }



  }
}