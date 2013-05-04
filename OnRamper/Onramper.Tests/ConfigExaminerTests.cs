using NUnit.Framework;
using OnRamper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;

namespace Onramper.Tests
{
    [TestFixture]
    public class ConfigExaminerTests
    {
        private ConfigExaminer target;

        [SetUp]
        public void SetUp()
        {
            target = new ConfigExaminer();
        }

        [Test]
        public void Should_Populate_XmlDocument_With_Simple_Nodes()
        {
            // Arrange
            var package = "Demo";
            var sXml = "<config><a><b>123</b></a><!--[[Demo]]--><c>Cool</c></config>";

            // Act
            var xDoc = target.Populate(package, sXml);

            // Assert
            xDoc.ToString().Should().Be("<config>\r\n  <c>Cool</c>\r\n</config>");
        }
    }
}
 