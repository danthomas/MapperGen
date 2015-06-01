using System;
using System.Collections.Generic;
using System.Linq;
using MapperGen.Core;
using NUnit.Framework;

namespace MapperGen.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        public class Entity
        {
            public string StringProperty { get; set; }
            public int Int32Property { get; set; }
            public bool BoolProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public EntityThing BigThing { get; set; }
            public EntityThing SmallThing { get; set; }
            public EntityThing[] ArrayOfThings { get; set; }
            public List<EntityThing> ListOfThings { get; set; }
            public class EntityThing
            {
                public string Name { get; set; }
            }
        }

        public class Model
        {
            public string StringProperty { get; set; }
            public int Int32Property { get; set; }
            public bool BoolProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public ModelThing BigThing { get; set; }
            public ModelThing SmallThing { get; set; }
            public ModelThing[] ArrayOfThings { get; set; }
            public List<ModelThing> ListOfThings { get; set; }
            public class ModelThing
            {
                public string Name { get; set; }
            }
        }

        public class DeepSource
        {
            public string Name { get; set; }
            public DeepSource Child { get; set; }
        }

        public class DeepTarget
        {
            public string Name { get; set; }
            public DeepTarget Child { get; set; }
        }

        [Test]
        public void MapDeepSourceToDeepTarget()
        {
            DeepSource deepSource = new DeepSource
            {
                Name = "One",
                Child = new DeepSource
                {
                    Name = "Two",
                    Child = new DeepSource
                    {
                        Name = "Three",
                        Child = new DeepSource
                        {
                            Name = "Four",
                            Child = new DeepSource()
                        }
                    }
                }
            };


            object mapper = new Generator().CompileMapper(typeof(DeepSource), typeof(DeepTarget));

            DeepTarget deepTarget = (DeepTarget) mapper.GetType().GetMethod("MapDeepSourceToDeepTarget").Invoke(mapper, new object[] { deepSource });

            Assert.That(deepSource.Name, Is.EqualTo(deepTarget.Name));
            Assert.That(deepSource.Child.Name, Is.EqualTo(deepTarget.Child.Name));
            Assert.That(deepSource.Child.Child.Name, Is.EqualTo(deepTarget.Child.Child.Name));
            Assert.That(deepSource.Child.Child.Child.Name, Is.EqualTo(deepTarget.Child.Child.Child.Name));
        }

        [Test]
        public void MapEntityToModel()
        {
            Entity entity = new Entity
            {
                StringProperty = "ABCD",
                Int32Property = 42,
                BoolProperty = true,
                DateTimeProperty = DateTime.Today,
                BigThing = new Entity.EntityThing { Name = "ABC" },
                SmallThing = new Entity.EntityThing { Name = "XYZ" },
                ArrayOfThings = new[] { new Entity.EntityThing { Name = "qwer" } },
                ListOfThings = new[] { new Entity.EntityThing { Name = "asdf" } }.ToList()
            };

            object mapper = new Generator().CompileMapper(typeof(Entity), typeof(Model));
            
            Model model = (Model)mapper.GetType().GetMethod("MapEntityToModel").Invoke(mapper, new object[] { entity });

            Assert.That(entity.StringProperty, Is.EqualTo(model.StringProperty));
            Assert.That(entity.Int32Property, Is.EqualTo(model.Int32Property));
            Assert.That(entity.BoolProperty, Is.EqualTo(model.BoolProperty));
            Assert.That(entity.DateTimeProperty, Is.EqualTo(model.DateTimeProperty));
            Assert.That(entity.BigThing.Name, Is.EqualTo(model.BigThing.Name));
            Assert.That(entity.SmallThing.Name, Is.EqualTo(model.SmallThing.Name));
            Assert.That(entity.ArrayOfThings[0].Name, Is.EqualTo(model.ArrayOfThings[0].Name));
            Assert.That(entity.ListOfThings[0].Name, Is.EqualTo(model.ListOfThings[0].Name));
        }

        [Test]
        public void MapEntityToModelNulls()
        {
            Entity entity = new Entity
            {
                StringProperty = "ABCD",
                Int32Property = 42,
                BoolProperty = true,
                DateTimeProperty = DateTime.Today
            };

            object mapper = new Generator().CompileMapper(typeof(Entity), typeof(Model));

            Model model = (Model)mapper.GetType().GetMethod("MapEntityToModel").Invoke(mapper, new object[] { entity });

            Assert.That(entity.StringProperty, Is.EqualTo(model.StringProperty));
            Assert.That(entity.Int32Property, Is.EqualTo(model.Int32Property));
            Assert.That(entity.BoolProperty, Is.EqualTo(model.BoolProperty));
            Assert.That(entity.DateTimeProperty, Is.EqualTo(model.DateTimeProperty));
        }
    }
}
