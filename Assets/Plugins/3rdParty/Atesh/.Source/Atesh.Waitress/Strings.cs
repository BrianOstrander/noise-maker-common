// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.Waitress.Extras;

namespace Atesh.Waitress
{
    public static class Strings
    {
        public static string WebRequestHasFailedAndWillRetry(string Url, string Error) => $"{nameof(WWW)} request has failed. It will retry.\nError: {Error}\nUrl: {Url}";
    }
}