using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webjobs.ImportSurveys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace CommonDotNetHelpers.Tests.Strings
{
    [TestClass()]
    public class StringFormatExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void EscapeCurlyBraces_NoCurlyBracesTest()
        {
            string test = "no curly braces";
            var actual = test.EscapeCurlyBraces();
            actual.Should().Be(test);
        }
        [TestMethod()]
        public void EscapeCurlyBraces_OpeningAndClosingBraces()
        {
            string test = "{curly braces}";
            var actual = test.EscapeCurlyBraces();
            actual.Should().Be("{{curly braces}}");
        }
        [TestMethod()]
        public void EscapeCurlyBraces_MultipeOpeningAndClosingBraces()
        {
            string test = "{{curly{{{}}}braces}}";
            var actual = test.EscapeCurlyBraces();
            actual.Should().Be("{{{{curly{{{{{{}}}}}}braces}}}}");
        }
    }
}
