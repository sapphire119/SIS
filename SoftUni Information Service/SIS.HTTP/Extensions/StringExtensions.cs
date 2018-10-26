using SIS.HTTP.Extensions.Interfaces;
using System.Text;

namespace SIS.HTTP.Extensions
{
    public class StringExtensions : IStringExtensions
    {
        private string wordToCapitlize;

        public StringExtensions(string wordToCapitlize)
        {
            this.wordToCapitlize = wordToCapitlize;
        }

        public string Capitalize()
        {
            StringBuilder sb = new StringBuilder();
            var firstLetter = this.wordToCapitlize[0].ToString().ToUpper();
            var restOfString = this.wordToCapitlize.Substring(1).ToLower();
            sb.Append(firstLetter).Append(restOfString);

            return sb.ToString().Trim();
        }
    }
}
