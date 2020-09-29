using System;
using Microsoft.CodeAnalysis;

namespace Iosp
{
  internal static class OperationDiagnostics
  {

    public static DiagnosticDescriptor MustNotCallIntegrationDiagnostic = new DiagnosticDescriptor("IOSP2001", "Operation must not call Integration", "Operation must not call Integration", "Operation", DiagnosticSeverity.Error, true);
    public static DiagnosticDescriptor MustNotCallOperationsDiagnostic = new DiagnosticDescriptor("IOSP2002", "Operation must not call Operation", "Operation must not call Operation", "Operation", DiagnosticSeverity.Error, true);


    public static Diagnostic MustNotCallIntegration(Location location)
    {
      return Diagnostic.Create(MustNotCallIntegrationDiagnostic, location);
    }

    public static Diagnostic MustNotCallOperations(Location location)
    {
      return Diagnostic.Create(MustNotCallOperationsDiagnostic, location);
    }

  }
}