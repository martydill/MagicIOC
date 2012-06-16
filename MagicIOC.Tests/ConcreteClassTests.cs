using System;
using NUnit.Framework;

namespace MagicIOC.Tests
{
    public class ConcreteClassTests
    {
        [Test]
        public void TestCreateObjectWithParameterlessConstructor()
        {
            var random = MagicIOC.Get<Random>();
            Assert.That(random, Is.InstanceOf<Random>());
        }

        [Test]
        public void TestCreateClassThatDependsOnAnotherClass()
        {
            var foo = MagicIOC.Get<Foo>();
            Assert.That(foo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void TestCreateClassTwiceReturnsSameObject()
        {
            var foo1 = MagicIOC.Get<Foo>();
            var foo2 = MagicIOC.Get<Foo>();

            Assert.That(foo1, Is.SameAs(foo2));
        }

        [Test]
        public void TestCreateClassTwiceUsesSameObjectAsParameters()
        {
            var foo1 = MagicIOC.Get<Foo>();
            var foo2 = MagicIOC.Get<Foo>();

            Assert.That(foo1.Bar, Is.SameAs(foo2.Bar));
        }

        [Test]
        public void TestValidParametersPassedToConstructor()
        {
            var foo = MagicIOC.Get<Foo>();
            Assert.That(foo.Bar, Is.InstanceOf<Bar>());
        }

        [Test]
        public void TestCreateObjectWithNoConstructorsThrowsException()
        {
            Assert.Throws<ArgumentException>(() => MagicIOC.Get<CantCreate>());
        }

        [Test]
        public void TestCreateObjectWithUnsatisfiableConstructorParametersThrowsException()
        {
            Assert.Throws<ArgumentException>(() => MagicIOC.Get<DependsOnInterface>());
        }

        public class Bar
        {
        }

        public class Foo
        {
            public Bar Bar { get; set; }

            public Foo(Bar bar)
            {
                Bar = bar;
            }
        }

        class CantCreate
        {
            private CantCreate()
            {
            }
        }

        interface INotImplemented
        {
        }

        class DependsOnInterface
        {
            public DependsOnInterface(INotImplemented notImplemented)
            {
            }
        }
    }
}
