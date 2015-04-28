using System;
using MapperGen.Core;
using NUnit.Framework;

namespace MapperGen.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void XXX()
        {
            string mapper = new GeneratorOld().GenerateMapperNew(typeof(Entity), typeof(Model));
            
        }

        [Test]
        public void Test()
        {
            GeneratorOld generator = new GeneratorOld();

            string mapper = generator.GenerateMapper(typeof(Entity), typeof(Model));
            string test = generator.GenerateTest(typeof(Entity), typeof(Model));
        }

        public class Entity
        {
            public string StringProperty { get; set; }
            public int Int32Property { get; set; }
            public bool BoolProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public EntityThing Thing { get; set; }
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
            public ModelThing Thing { get; set; }
            public class ModelThing
            {
                public string Name { get; set; }
            }
        }


        [Test]
        public void MapEntityToModel()
        {
            Entity entity = new Entity
            {
                StringProperty = "ABCD",
                Int32Property = 42,
                BoolProperty = true,
                DateTimeProperty = DateTime.Today
            };

            Model model = new Mapper().MapEntityToModel(entity);


            Assert.That(entity.StringProperty, Is.EqualTo(model.StringProperty));
            Assert.That(entity.Int32Property, Is.EqualTo(model.Int32Property));
            Assert.That(entity.BoolProperty, Is.EqualTo(model.BoolProperty));
            Assert.That(entity.DateTimeProperty, Is.EqualTo(model.DateTimeProperty));
        }
    }

    public class Mapper
    {
        public Class1.Model MapEntityToModel(Class1.Entity entity)
        {
            Class1.Model model = new Class1.Model
            {
                StringProperty = entity.StringProperty,
                Int32Property = entity.Int32Property,
                BoolProperty = entity.BoolProperty,
                DateTimeProperty = entity.DateTimeProperty,
                Thing = MapEntityThingToModelThing(entity.Thing)
            };

            return model;
        }

        public Class1.Model.ModelThing MapEntityThingToModelThing(Class1.Entity.EntityThing entityThing)
        {
            Class1.Model.ModelThing modelThing = new Class1.Model.ModelThing
            {
                Name = entityThing.Name
            };

            return modelThing;
        }
    }
}
