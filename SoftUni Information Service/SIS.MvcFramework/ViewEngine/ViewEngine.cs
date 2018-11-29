namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.ViewEngine.Contracts;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

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
    using SIS.MvcFramework.ViewEngine.Contracts;
    using" + typeof(T).Namespace + @";
    
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class " + viewTypeName + @" : IView<" + typeForCSharpCode + @">
    {
        public string GetHtml(" + typeForCSharpCode + @" model)
        {
            StringBuilder sb = new StringBuilder();

            " + cSharpMethodBody + @"

            return sb.ToString().Trim()
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
                .AddReferences(MetadataReference.CreateFromFile(
                    Assembly.Load(new AssemblyName("netstandard"))))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                //.AddReferences(
                //MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(cSharpCode));


            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> errors =
                    result.Diagnostics
                    .Where(diagnostic => diagnostic.IsWarninrError || diagnostic.Severity == DiagnosticSeverity.Error);

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
            return string.Empty;
        }
    }
}
