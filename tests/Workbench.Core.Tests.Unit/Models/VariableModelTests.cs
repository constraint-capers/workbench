﻿using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Nodes;

namespace Workbench.Core.Tests.Unit.Models
{
    [TestFixture]
    public class VariableModelTests
    {
        [Test]
        public void InitializeVariableWithValidNameSetsExpectedNameInVariable()
        {
            var sut = new VariableGraphicModel("x");
            Assert.That(sut.Name, Is.EqualTo("x"));
        }

        [Test]
        public void InitializeVariableWithEmptyExpressionWoutWhitespace()
        {
            var sut = new VariableGraphicModel("x", "");
            Assert.That(sut.DomainExpression.IsEmpty, Is.True);
        }

        [Test]
        public void InitializeVariableWithDomainReferenceRawExpressionWithWhitespace()
        {
            var sut = new VariableGraphicModel("x", "   A    ");
            Assert.That(sut.DomainExpression.DomainReference.DomainName.Name, Is.EqualTo("A"));
        }

        [Test]
        public void InitializeVariableWithDomainReferenceRawExpressionWoutWhitespace()
        {
            var sut = new VariableGraphicModel("x", "A");
            Assert.That(sut.DomainExpression.DomainReference.DomainName.Name, Is.EqualTo("A"));
        }

        [Test]
        public void InitializeVariableWithInlineRawExpressionWoutWhitespace()
        {
            var sut = new VariableGraphicModel("x", "1..10");
            Assert.That(sut.DomainExpression.InlineDomain, Is.InstanceOf<DomainExpressionNode>());
        }
    }
}
