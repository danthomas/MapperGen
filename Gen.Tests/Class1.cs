using CodeGen;
using Gen.Core;
using NUnit.Framework;

namespace Gen.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Tester()
        {
            const string template = "Name = '<#= Thing.Name #>'";
            var thing = new Thing {Name = "Hello"};

            var generator = new Generator<Thing>
            {
                Template = template
            };

            new ThingGenerator().Generate(thing);

     string sdfsdf =       generator.Gen(thing);

            Assert.That(generator.GenFiles[0].Text, Is.EqualTo("ThingName = 'Hello'"));
        }

        public class Thing
        {
            public string Name { get; set; }
        }
    }
}