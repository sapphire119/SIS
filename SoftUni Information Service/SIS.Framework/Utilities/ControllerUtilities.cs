namespace SIS.Framework.Utilities
{
    public static class ControllerUtilities
    {
        public static string GetControllerName(object controller)
            => controller
                .GetType().Name
                .Replace(MvcContext.Get.ControllerSuffix, string.Empty);

        public static string GetViewFullQualifiedName(
            string controllerName,
            string action)
            => string.Format("{0}//{1}//{2}", MvcContext.Get.ViewsFolder, controllerName, action);
    }
}
