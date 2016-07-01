using System.Linq;
using FluentAssertions;
using Microsoft.SDC.Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Data
{
    [TestClass()]
    public class EmbeddedSqlScriptsHelperTests
    {
        [TestMethod()]
        public void SplitSqlStatements_GOonSeparateLine_CorrectSeparator()
        {
            string script = @"SELECT * FROM information_schema.tables
  GO
SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var commands = EmbeddedSqlScriptsHelper.SplitSqlStatements(script);
            commands.Count().Should().Be(2);
        }
        [TestMethod()]
        public void SplitSqlStatements_GOWithSemicolonAfter_IncorrectSeparator()
        {
            string script = @"SELECT * FROM information_schema.tables
  GO;
SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var commands = EmbeddedSqlScriptsHelper.SplitSqlStatements(script);
            commands.Count().Should().Be(1);
        }
        [TestMethod()]
        public void SplitSqlStatements_GOWithSemicolonBefore_IncorrectSeparator()
        {
            string script = @"SELECT * FROM information_schema.tables
 ; GO
SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var commands = EmbeddedSqlScriptsHelper.SplitSqlStatements(script);
            commands.Count().Should().Be(1);
        }
    }
}
