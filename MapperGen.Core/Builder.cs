using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MapperGen.Core
{
    public class Builder
    {
        public T BuildGenerator<T>(string templateFile)
        {
            string template = GetTemplate(templateFile);

            string generatorCode = BuildGeneratorClass(template, typeof(T).FullName);

            return (T)Compile(generatorCode);
        }

        private string GetTemplate(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string BuildGeneratorClass(string template, string baseClass)
        {
            string[] parts = Regex.Split(template, @"(<#|#>)");

            bool isCode = false;

            string functions = "";

            string code = String.Format(@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using MapperGen.Core;

namespace CodeGen
{{
    public partial class BuildersGenerator : {0}
    {{
        public override string Generate()
        {{
            StringBuilder stringBuilder = new StringBuilder();
", baseClass);

            foreach (string part in parts)
            {
                if (part == "<#")
                {
                    isCode = true;
                }
                else if (part == "#>")
                {
                    isCode = false;
                }
                else if (isCode)
                {
                    if (part.StartsWith("="))
                    {
                        code += @"
            stringBuilder.Append(" + part.Substring(1).Trim() + ");";
                    }
                    else if (part.StartsWith("@"))
                    {
                        functions += Environment.NewLine + part.Substring(1).Trim();
                    }
                    else
                    {
                        code += part;
                    }
                }
                else
                {
                    if (part != "")
                    {
                        string text = part.Replace(@"""", @"""""");

                        code += @"
            stringBuilder.Append(@""" + text + @""");";
                    }
                }
            }

            code += @"
            return stringBuilder.ToString();
        }";


            code += functions;

            code += @"
    }
}";

            return code;
        }

        public object Compile(string generatorCode)
        {
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");

            CompilerParameters compilerParameters = new CompilerParameters { GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("MapperGen.Core.dll");

            compilerParameters.IncludeDebugInformation = false;

            string[] sources = { generatorCode };

            CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sources);

            if (compilerResults.Errors.HasErrors)
            {
                //templateSource.Errors = compilerResults.Errors.Cast<CompilerError>().Select(item => item.Line + ": " + item.ErrorText).ToList();
                throw new Exception();
            }

            Type type = compilerResults.CompiledAssembly.GetType(String.Format("{0}.{1}", "CodeGen", "BuildersGenerator"));

            return Activator.CreateInstance(type);
        }
    }
}