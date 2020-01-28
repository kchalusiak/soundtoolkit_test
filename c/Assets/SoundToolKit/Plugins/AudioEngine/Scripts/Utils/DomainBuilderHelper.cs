/// \author Marcin Misiek
/// \date 05.09.2018

using System.Globalization;
using System.Text.RegularExpressions;

namespace SoundToolKit.Unity.Utils
{
    public static class DomainBuilderHelper
    {
        public static string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }

        public static bool InDomain(string rootDomain, string other)
        {
            return other.StartsWith(rootDomain) &&
                rootDomain.Split('.').Length == other.Split('.').Length;
        }

        public static string GetDomainPrefix(string domain, string name)
        {
            var domainPrefix = domain.Replace(name, "");

            return domainPrefix.Remove(domainPrefix.Length - 1);
        }

        public static string GetDomainTitle(string domain)
        {
            var title = domain.Replace(".", " ").Replace("_", " ");
            title = Regex.Replace(title, "(\\B[A-Z])", " $1");

            return title;
        }

        public static string FirstLetterToUpper(string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
