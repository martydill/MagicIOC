using System;
using NUnit.Framework;

namespace MagicIOC.Tests
{
    public class CachePolicyTests
    {
        [Test]
        public void TestCreateCachedObjectReturnsSameInstance()
        {
            var random = MagicIOC.Get<Random>(CachePolicy.Cached);
            var random2 = MagicIOC.Get<Random>(CachePolicy.Cached);
            Assert.That(random, Is.SameAs(random2));
        }

        [Test]
        public void TestCreateTwoUncachedObjectsReturnsDifferentInstance()
        {
            var random = MagicIOC.Get<Random>(CachePolicy.Cached);
            var random2 = MagicIOC.Get<Random>(CachePolicy.New);
            Assert.That(random, Is.Not.SameAs(random2));
        }

        [Test]
        public void TestCreateCachedObjectAfterCreatingUncachedObjectReturnsDifferentCachedInstance()
        {
            var random = MagicIOC.Get<Random>(CachePolicy.New);
            var random2 = MagicIOC.Get<Random>(CachePolicy.Cached);
            var random3 = MagicIOC.Get<Random>(CachePolicy.Cached);

            Assert.That(random, Is.Not.SameAs(random2));
            Assert.That(random2, Is.SameAs(random3));
        }

        [Test]
        public void TestCreateUncachedObjectUsesCachedParameters()
        {
            var foo1 = MagicIOC.Get<Foo>(CachePolicy.New);
            var foo2 = MagicIOC.Get<Foo>(CachePolicy.New);

            Assert.That(foo1, Is.Not.SameAs(foo2));
            Assert.That(foo1.Bar, Is.SameAs(foo2.Bar));
        }

        class Foo
        {
            public Bar Bar { get; set; }

            public Foo(Bar bar)
            {
                Bar = bar;
            }
        }

        class Bar
        {
        }
    }
}
