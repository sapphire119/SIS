namespace SIS.MvcFramework.Tests.ViewEngineTests
{
    using SIS.MvcFramework.RenderEngine.Contracts;
    using SIS.MvcFramework.RenderEngine;

    using Xunit;

    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

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

            TestModel model = new TestModel()
            {
                String = "Username",
                List = new List<string> { "Item1", "item2", "test", "123", "" }
            };

            var result = viewEngine.GetHtml(testViewName, viewContent, model);

            Assert.Equal(expectedResult, result);
        }

        public class TestModel
        {
            public string String { get; set; }

            public IEnumerable<string> List { get; set; }
        }
    }
}
