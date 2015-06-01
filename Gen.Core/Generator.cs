using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gen.Core
{
    public interface IGenerator<S>
    {
        string Generate(S source);
    }

    public class Generator<S> where S : class
    {
        public string Template { get; set; }

        public Generator()
        {
            GenFiles = new List<GenFile>();
        }

        public List<GenFile> GenFiles { get; set; } 

        public string Gen(S source)
        {
            Code = BuildGeneratorClass();
            
            IGenerator<S> gen = Compile();

            return gen.Generate(source);
        }

        public string Code { get; set; }

        private string BuildGeneratorClass()
        {
            Type baseType = typeof (S);

            string[] parts = Regex.Split(Template, @"(<#|#>)");

            bool isCode = false;

            string functions = "";

            string code = String.Format(@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using Gen.Core;

namespace CodeGen
{{
    public partial class BuildersGenerator : {0}, IGenerator
    {{
        public string Generate()
        {{
            StringBuilder stringBuilder = new StringBuilder();
", baseType.FullName.Replace("+", "."));

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

        private IGenerator<S> Compile()
        {
            Type baseType = typeof(S);

            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");

            CompilerParameters compilerParameters = new CompilerParameters { GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("Gen.Core.dll");
            
            compilerParameters.ReferencedAssemblies.Add(baseType.Module.Name);

            compilerParameters.IncludeDebugInformation = false;

            string[] sources = { Code };

            CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sources);

            if (compilerResults.Errors.HasErrors)
            {
                //templateSource.Errors = compilerResults.Errors.Cast<CompilerError>().Select(item => item.Line + ": " + item.ErrorText).ToList();
                throw new Exception();
            }

            Type type = compilerResults.CompiledAssembly.GetType(String.Format("{0}.{1}", "CodeGen", "BuildersGenerator"));

            return (IGenerator<S>) Activator.CreateInstance(type);
        }
    }
}
