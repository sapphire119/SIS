namespace SIS.MvcFramework.Tests.ViewEngineTests
{
    using SIS.MvcFramework.ViewEngine.Contracts;
    using SIS.MvcFramework.ViewEngine;

    using Xunit;

    using System.IO;

    public class ViewEngineTests
    {
        [Theory]
        [InlineData("IfForAndForeach")]
        [InlineData("ViewWithNoCode")]
        [InlineData("WorkWithViewModel")]
        public void RunTestViews(string testViewName)
        {
            var viewContent = File.ReadAllText($"ViewTests/{testViewName}.html");
            var expectedResult = File.ReadAllText($"ViewTests/{testViewName}_Result.html");
            IViewEngine viewEngine = new ViewEngine();

            var result = viewEngine.GetHtml(viewContent);

            Assert.Equal(expectedResult, result);
        }
    }
}
