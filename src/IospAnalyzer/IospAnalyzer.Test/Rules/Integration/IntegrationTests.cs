using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iosp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace IospAnalyzer.Test.Rules
{

  [TestClass]
  public class IntegrationTests : DiagnosticVerifier
  {

    [TestMethod]
    public void MustNotCallApi()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            Api();
        }
        
        public void Api()
        {

        }
    }";


      var result = Diagnose(test);

      Assert.IsTrue(result.Id == IntegrationDiagnostics.MustNotCallApiDiagnostic.Id);
    }


    [TestMethod]
    public void CallToIntegrationIsAllowed()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            Integration();
        }

        [Integration]
        public void Integration2()
        {

        }
    }";


      var result = Diagnose(test);

      Assert.IsNull(result);
    }


    [TestMethod]
    public void CallToOperationIsAllowed()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            Operation();
        }

        [Operation]
        public void Operation()
        {

        }
    }";


      var result = Diagnose(test);

      Assert.IsNull(result);
    }

    [TestMethod]
    public void MustNotUseForLoop()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            for(int i = 0; i < 1; i++) {}
        }

    }";


      var result = Diagnose(test);

      Assert.IsTrue(result.Id == IntegrationDiagnostics.MustNotUseForLoopDiagnostic.Id);
    }


    [TestMethod]
    public void MustNotUseWhileLoop()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            while(true) {}
        }

    }";


      var result = Diagnose(test);

      Assert.IsTrue(result.Id == IntegrationDiagnostics.MustNotUseWhileLoopDiagnostic.Id);
    }


    [TestMethod]
    public void ForEachLoopIsAllowed()
    {
      var test = @"
        [Integration]
        public void Integration()
        {
            foreach(var c in "") {}
        }

    }";


      var result = Diagnose(test);

      Assert.IsNull(result);
    }


    [TestMethod]
    public void ConditionIsAllowed()
    {
      var test = @"
        [Operation]
        public void Operation()
        {
            if(true) {return;}
        }

    }";

      var result = Diagnose(test);

      Assert.IsNull(result);
    }



    [TestMethod]
    public void SwitchIsAllowed()
    {
      var test = @"
        [Operation]
        public void Operation()
        {
            System.TypeCode e = System.TypeCode.Boolean;
            switch(e){}
        }

    }";


      var result = Diagnose(test);

      Assert.IsNull(result);
    }

    private Diagnostic Diagnose(string test)
    {
      var source = TestClass(test);

      var diagnostics = GetDiagnostics(source);

      var result = diagnostics.FirstOrDefault();
      return result;
    }


    private Diagnostic[] GetDiagnostics(string source)
    {
      return GetSortedDiagnostics(new[] {source}, LanguageNames.CSharp, GetCSharpDiagnosticAnalyzer());
    }


    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
    {
      return new Iosp.IospAnalyzer();
    }


    public string TestClass(string content)
    {
      var testClass = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Iosp;


    namespace Iosp
    {
        class TestClass
        {   
";

      testClass += content;


      testClass += @"
    }
    }";

      return testClass;

    }
  }
}

