using System.Linq;
using System.Reflection;

namespace SIS.MvcFramework.ObjectMapper
{
    public static class ObjectMapper
    {
        //Cannot Map IEnumerables
        public static T To<T>(this object source)
            where T : class, new()
            //new() означава, че това, което подавам трябва да има празен констркутор
        {
            var destination = new T();
            var destinationProperties = destination.GetType().GetProperties();

            foreach (var destinationProperty in destinationProperties)
            {
                if (destinationProperty.SetMethod == null) continue;


                var sourceProperty = source.GetType()
                    .GetProperties()
                    .FirstOrDefault(x =>
                        x.Name.ToLower() == destinationProperty.Name.ToLower());

                if (sourceProperty?.GetMethod != null)
                {
                    var sourceValue = sourceProperty.GetMethod.Invoke(source, new object[] { });
                    destinationProperty.SetValue(destination, new object[] { sourceValue });
                }
            }

            return destination;
        }
    }
}
