using System;
using System.Linq;
using System.Reflection;

namespace MapperGen.Core
{
    public class GeneratorOld
    {
        public string GenerateMapper(Type sourceType, Type targetType)
        {
            var targetTypeName = targetType.Name;
            var sourceTypeName = sourceType.Name;
            var targetTypeFullName = targetType.FullName.Replace("+", ".");
            var sourceTypeFullName = sourceType.FullName.Replace("+", ".");

            string sourceVariableName = Char.ToLower(sourceTypeName[0]) + sourceTypeName.Substring(1);
            string targetVariableName = Char.ToLower(targetTypeName[0]) + targetTypeName.Substring(1);

            string mapper = String.Format(@"
public class Mapper
{{
    public {0} Map{4}To{1}({3} {5})
    {{
        {0} {2} = new {0}
        {{", targetTypeFullName, targetTypeName, targetVariableName, sourceTypeFullName, sourceTypeName, sourceVariableName);

            bool first = true;

            foreach (PropertyInfo sourcePropertyInfo in sourceType.GetProperties())
            {
                PropertyInfo targetPropertyInfo = targetType.GetProperties()
                    .SingleOrDefault(x => x.Name == sourcePropertyInfo.Name && x.PropertyType == sourcePropertyInfo.PropertyType);

                if (targetPropertyInfo != null)
                {
                    mapper += String.Format(@"{0}
            {1} = {2}.{1}",  first ? "" : ",", sourcePropertyInfo.Name, sourceVariableName);
                }

                first = false;
            }

            mapper += String.Format(@"
        }};

        return {0};
    }}
}}", targetVariableName);

            return mapper;
        }

        public string GenerateTest(Type sourceType, Type targetType)
        {
            var targetTypeName = targetType.Name;
            var sourceTypeName = sourceType.Name;
            var targetTypeFullName = targetType.FullName.Replace("+", ".");
            var sourceTypeFullName = sourceType.FullName.Replace("+", ".");

            string sourceVariableName = Char.ToLower(sourceTypeName[0]) + sourceTypeName.Substring(1);
            string targetVariableName = Char.ToLower(targetTypeName[0]) + targetTypeName.Substring(1);

            string test = String.Format(@"
        [Test]
        public void Map{4}To{1}()
        {{
            {3} {5} = new {3}
            {{", targetTypeFullName, targetTypeName, targetVariableName, sourceTypeFullName, sourceTypeName, sourceVariableName);


            bool first = true;

            foreach (PropertyInfo sourcePropertyInfo in sourceType.GetProperties())
            {
                PropertyInfo targetPropertyInfo = targetType.GetProperties()
                    .SingleOrDefault(x => x.Name == sourcePropertyInfo.Name && x.PropertyType == sourcePropertyInfo.PropertyType);

                if (targetPropertyInfo != null)
                {
                    test += String.Format(@"{0}
                {1} = {2}", first ? "" : ",", sourcePropertyInfo.Name, GetValue(sourcePropertyInfo.PropertyType));
                }

                first = false;
            }


            
            test += String.Format(@"
            }};

            {0} {2} = new Mapper().Map{4}To{1}({5});

", targetTypeFullName, targetTypeName, targetVariableName, sourceTypeFullName, sourceTypeName, sourceVariableName);

            

            foreach (PropertyInfo sourcePropertyInfo in sourceType.GetProperties())
            {
                PropertyInfo targetPropertyInfo = targetType.GetProperties()
                    .SingleOrDefault(x => x.Name == sourcePropertyInfo.Name && x.PropertyType == sourcePropertyInfo.PropertyType);

                if (targetPropertyInfo != null)
                {
                    test += String.Format(@"
            Assert.That({5}.{6}, Is.EqualTo({2}.{6}));", targetTypeFullName, targetTypeName, targetVariableName, sourceTypeFullName, sourceTypeName, sourceVariableName, sourcePropertyInfo.Name);
                }

            }




            test += String.Format(@"
        }}", targetTypeFullName, targetTypeName, targetVariableName, sourceTypeFullName, sourceTypeName, sourceVariableName);

            return test;

        }

        private string GetValue(Type type)
        {
            if (type == typeof(Boolean))
            {
                    return "true";
            }
            
            if (type == typeof (String))
            {
                return "\"ABCD\"";
            }
            
            if (type == typeof (Int32))
            {
                return "42";
            }
            
            if (type == typeof (DateTime))
            {
                return "DateTime.Today";
            }

            throw new Exception();
        }

        public string GenerateMapperNew(Type sourceType, Type targetType)
        {
            MapperGenBase mapperGenBase = new Generator().BuildGenerator<MapperGenBase>("MapperGen.Core.MapperTemplate.txt");

            return mapperGenBase.Generate(sourceType, targetType);
        }
    }
}
