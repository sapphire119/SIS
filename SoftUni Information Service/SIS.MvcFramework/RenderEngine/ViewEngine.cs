namespace SIS.MvcFramework.RenderEngine
{
    using SIS.MvcFramework.RenderEngine.Contracts;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;


    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var viewTypeName = string.Concat(viewName, "View");
            var cSharpMethodBody = this.GenerateCSharpMethodBody(viewCode);

            var typeForCSharpCode = typeof(T).FullName.Replace("+", ".");

            var viewCodeAsCSharpCode = @"
namespace MyAppViews
{
    using SIS.MvcFramework.RenderEngine.Contracts;
    using " + typeof(T).Namespace + @";
    
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class " + viewTypeName + @" : IView<" + typeForCSharpCode + @">
    {
        public string GetHtml(" + typeForCSharpCode + @" model)
        {
            StringBuilder html = new StringBuilder();
            var Model = model;

            " + cSharpMethodBody + @"

            return html.ToString().Trim();
        }
    }
}
";
            var instaceOfViewClass =
                this.GetInstance(viewCodeAsCSharpCode, "MyAppViews." + viewTypeName, typeof(T)) as IView<T>;

            if (instaceOfViewClass == null)
            {
                throw new Exception($@"Instace of class: {viewName}View is null!");
            }

            var html = instaceOfViewClass.GetHtml(model);


            return html;
        }

        private object GetInstance(string cSharpCode, string typeName, Type viewModelType)
        {
            var tempFile = Path.GetRandomFileName() + ".dll";

            var compilation = CSharpCompilation
                .Create(tempFile)
                .WithOptions(new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary /*.dll*/))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location))
                .AddReferences(MetadataReference.CreateFromFile(
                    Assembly.Load(new AssemblyName("netstandard")).Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                //.AddReferences(
                //MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(cSharpCode));

            var netStandardReferences = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();
            foreach (var assemblyName in netStandardReferences)
            {
                compilation = compilation.AddReferences(
                    MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location));
            }

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> errors =
                    result.Diagnostics
                    .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in errors)
                    {
                        Console.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    //TODO: Exception
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                Type viewType = assembly.GetType(typeName);

                return Activator.CreateInstance(viewType);
            }
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            var lines = this.GetLines(viewCode);

            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("{") || line.Trim().StartsWith("}") ||
                    line.Trim().StartsWith("@for") || line.Trim().StartsWith("@if") ||
                    line.Trim().StartsWith("@else")) 
                {
                    //CsharpCode
                    var firstAtSymbolIndex = line.IndexOf('@', StringComparison.InvariantCulture);
                    sb.AppendLine(this.RemoveAt(line, firstAtSymbolIndex));
                }
                else
                {
                    var htmlLine = line.Replace("\"", "\\\"");
                    while (htmlLine.Contains("@"))
                    {
                        var specialSymbolIndex = htmlLine.IndexOf("@", StringComparison.InvariantCulture);
                        var endOfCode = new Regex(@"[\s<\\]+").Match(htmlLine, specialSymbolIndex).Index;
                        //htmlLine = htmlLine.Substring(0, endOfCode);
                        string expression = null;
                        if (endOfCode == 0 || endOfCode == -1)
                        {
                            expression = htmlLine.Substring(specialSymbolIndex + 1);
                            htmlLine =
                            htmlLine.Substring(0, specialSymbolIndex) +
                            "\" + " + expression + " + \"";
                        }
                        else
                        {
                            expression = htmlLine.Substring
                                (specialSymbolIndex + 1, endOfCode - specialSymbolIndex - 1);

                            htmlLine =
                            htmlLine.Substring(0, specialSymbolIndex) +
                            "\" + " + expression + " + \"" +
                            htmlLine.Substring(endOfCode);
                        }

                    }
                    sb.AppendLine($"html.AppendLine(\"{htmlLine}\");");
                }
            }
            return sb.ToString().Trim();
        }

        private IEnumerable<string> GetLines(string input)
        {
            var stringReader = new StringReader(input);

            var lines = new List<string>();

            string line;
            while ((line = stringReader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines;
        }

        private string RemoveAt(string input, int index)
        {
            if (index == -1)
            {
                return input;
            }
            return input.Substring(0, index) + input.Substring(index + 1);
        }
    }
}
