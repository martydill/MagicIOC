using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MagicIOC.Tests
{
    public class InterfaceTests
    {
        [Test]
        public void TestCreateInterfaceWithNoImplementationThrowsException()
        {
            Assert.Throws<ArgumentException>(() => MagicIOC.Get<INotImplemented>());
        }

        [Test]
        public void TestCreateInterfaceWithImplementationReturnsImplementation()
        {
            var impl = MagicIOC.Get<IImplemented>();
            Assert.That(impl, Is.InstanceOf<Implemented>());
        }

        [Test]
        public void TestCreateInterfaceMultipleTimesReturnsSameImplementation()
        {
            var impl = MagicIOC.Get<IImplemented>();
            var impl2 = MagicIOC.Get<IImplemented>();

            Assert.That(impl, Is.SameAs(impl2));
        }

        interface INotImplemented
        {
        }

        interface IImplemented
        {
        }

        class Implemented : IImplemented
        {
        }
    }
}
