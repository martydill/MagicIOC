using System;
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

        [Test]
        public void TestCreateInterfaceDependentOnOtherInterfacesReturnsImplementation()
        {
            var foo = MagicIOC.Get<IFoo>();
            Assert.That(foo, Is.InstanceOf<Foo>());
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

        interface IBar
        {
        }

        class Bar : IBar
        {
        }

        interface IBaz
        {
        }

        class Baz : IBaz
        {
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
            public Foo(IBar bar, IBaz baz)
            {
            }
        }
    }
}
