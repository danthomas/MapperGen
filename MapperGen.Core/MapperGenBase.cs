using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MapperGen.Core
{
    public abstract class MapperGenBase
    {
        public abstract string Generate();

        public string Generate(Type sourceType, Type targetType)
        {
            ClassMaps = new List<ClassMap>();

            AddClassMaps(sourceType, targetType);

            return Generate();
        }

        private void AddClassMaps(Type sourceType, Type targetType)
        {
            if (!ClassMaps.Any(item => item.SourceType == sourceType && item.TargetType == targetType))
            {
                ClassMap classMap = new ClassMap(sourceType, targetType);

                ClassMaps.Add(classMap);

                foreach (PropMap propMap in classMap.PropMaps.Where(x => x.IsComposite))
                {
                    if (typeof(IEnumerable).IsAssignableFrom(propMap.SourceProp.Type))
                    {
                        if (propMap.SourceProp.Type.IsGenericType)
                        {
                            AddClassMaps(propMap.SourceProp.Type.GetGenericArguments()[0], propMap.TargetProp.Type.GetGenericArguments()[0]);
                        }
                        else if (propMap.SourceProp.Type.IsArray)
                        {
                            AddClassMaps(propMap.SourceProp.Type.GetElementType(), propMap.TargetProp.Type.GetElementType());
                        }
                    }
                    else
                    {
                        AddClassMaps(propMap.SourceProp.Type, propMap.TargetProp.Type);
                    }
                }
            }
        }

        public List<ClassMap> ClassMaps { get; set; }
    }

    public class ClassMap
    {
        public ClassMap(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
            SourceTypeName = sourceType.Name;
            TargetTypeName = targetType.Name;
            SourceTypeFullName = sourceType.FullName.Replace("+", ".");
            TargetTypeFullName = targetType.FullName.Replace("+", ".");
            SourceVariable = Char.ToLower(SourceTypeName[0]) + SourceTypeName.Substring(1);
            TargetVariable = Char.ToLower(TargetTypeName[0]) + TargetTypeName.Substring(1);

            var sourceProperties = SourceType.GetProperties().Select(x => new Prop(x));
            var targetProperties = TargetType.GetProperties().Select(x => new Prop(x));

            PropMaps = sourceProperties
                .Join(targetProperties, s => s.Name, t => t.Name, (s, t) => new PropMap(s, t))
                .Select(x => x)
                .ToList();

        }

        public List<PropMap> PropMaps { get; set; }

        public string SourceVariable { get; set; }

        public string TargetVariable { get; set; }

        public string SourceTypeFullName { get; set; }

        public string TargetTypeFullName { get; set; }

        public string SourceTypeName { get; set; }

        public string TargetTypeName { get; set; }

        public Type SourceType { get; set; }

        public Type TargetType { get; set; }
    }

    public class PropMap
    {
        public PropMap(Prop sourceProp, Prop targetProp)
        {
            SourceProp = sourceProp;
            TargetProp = targetProp;
            IsComposite = sourceProp.Type.IsClass && sourceProp.Type != typeof(String);
        }

        public Prop SourceProp { get; set; }
        public Prop TargetProp { get; set; }
        public bool IsComposite { get; set; }
    }

    public class Prop
    {
        public Prop(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            TypeName = propertyInfo.PropertyType.Name;
            FullTypeName = propertyInfo.PropertyType.FullName.Replace("+", ".");

            if (propertyInfo.PropertyType.IsArray)
            {
                ElementType = propertyInfo.PropertyType.GetElementType();
                EnumerableType = EnumerableType.Array;
            }
            else if (typeof (IList).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(String))
            {
                ElementType = propertyInfo.PropertyType.GetGenericArguments()[0];
                EnumerableType = EnumerableType.List;
            }

        }

        public EnumerableType EnumerableType { get; set; }
        public Type ElementType { get; set; }

        public string TypeName { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public string FullTypeName { get; set; }
    }

    public enum EnumerableType
    {
        Array,
        List
    }
}
