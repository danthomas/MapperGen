using System;
using System.CodeDom.Compiler;

namespace MapperGen.Core
{
    public class Generator
    {
        public string GenerateMapper(Type sourceType, Type targetType)
        {
            MapperGenBase mapperGenBase = new Builder().BuildGenerator<MapperGenBase>("MapperGen.Core.MapperTemplate.txt");

            return mapperGenBase.Generate(sourceType, targetType);
        }

        public object CompileMapper(Type sourceType, Type targetType)
        {
            MapperGenBase mapperGenBase = new Builder().BuildGenerator<MapperGenBase>("MapperGen.Core.MapperTemplate.txt");

            string code = mapperGenBase.Generate(sourceType, targetType);
       
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");

            CompilerParameters compilerParameters = new CompilerParameters { GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("MapperGen.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("MapperGen.Tests.dll");

            compilerParameters.IncludeDebugInformation = false;

            string[] sources = { code };

            CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sources);

            if (compilerResults.Errors.HasErrors)
            {
                //templateSource.Errors = compilerResults.Errors.Cast<CompilerError>().Select(item => item.Line + ": " + item.ErrorText).ToList();
                throw new Exception();
            }

            Type type = compilerResults.CompiledAssembly.GetType("Mapper");

            return Activator.CreateInstance(type);
        }
    }
}
