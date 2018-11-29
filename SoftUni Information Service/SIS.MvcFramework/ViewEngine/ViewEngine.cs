namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.ViewEngine.Contracts;
    using System;
    using System.Text;

    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var cSharpMethodBody = this.GenerateCSharpMethodBody(viewCode);

            var viewCodeAsCSharpCode = @"
namespace MyAppViews
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class " + viewName + @"View : IView<" + typeof(T).FullName + @">
    {
        public string GetHtml("+ typeof(T).FullName + @" model)
        {
            StringBuilder sb = new StringBuilder();

            " + cSharpMethodBody + @"

            return sb.ToString().Trim()
        }
    }
}

";
            var instaceOfViewClass = this.GetInstance(viewCodeAsCSharpCode) as IView<T>;

            if (instaceOfViewClass == null)
            {
                throw new Exception($@"Instace of class: {viewName}View is null!");
            }

            var html = instaceOfViewClass.GetHtml(model);
            

            return html;
        }

        private object GetInstance(string viewCodeAsCSharpCode)
        {
            throw new NotImplementedException();
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            return string.Empty;
        }
    }
}
