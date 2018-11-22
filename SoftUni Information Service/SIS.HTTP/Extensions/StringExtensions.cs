namespace SIS.HTTP.Extensions
{
    using System.Text;

    public static class StringExtensions
    {
        public static string Capitalize(this string wordToCapitlize)
        {
            StringBuilder sb = new StringBuilder();
            var firstLetter = wordToCapitlize[0].ToString().ToUpper();
            var restOfString = wordToCapitlize.Substring(1).ToLower();
            sb.Append(firstLetter).Append(restOfString);

            return sb.ToString().Trim();
        }
    }
}
