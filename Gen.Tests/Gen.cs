using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using Gen.Core;
using Gen.Tests;

namespace CodeGen
{
    public partial class ThingGenerator : IGenerator<Class1.Thing>
    {
        public string Generate(Class1.Thing thing)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(@"Name = '");
            stringBuilder.Append(thing.Name);
            stringBuilder.Append(@"'");
            return stringBuilder.ToString();
        }
    }
}