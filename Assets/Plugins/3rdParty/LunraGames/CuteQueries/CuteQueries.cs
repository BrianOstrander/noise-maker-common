using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LunraGames.CuteQueries
{
    public class CuteQueries
    {
        public static void Log(string query, string response, Dictionary<string, string> headers)
        {
            try
            {
            	var colorize = false;// Application.isEditor;
            	var colorizeResponse = colorize && Application.HasProLicense();
                string message = query + "\n";
                if (colorize) message += "<color=green>~~~~ Header ~~~~</color>\n";
				else message += "~~~~ Header ~~~~\n";
                foreach (var kv in headers) message += kv.Key + " : " + kv.Value + "\n";
                if (colorize) message += "<color=green>~~~~ Response ~~~~</color>\n";
				else message += "~~~~ Response ~~~~\n";
                var parsed = JsonConvert.DeserializeObject(response);
                if (colorizeResponse) message += "<color=white>";
				message += JsonConvert.SerializeObject(parsed, Formatting.Indented);
				if (colorizeResponse) message += "</color>";
                Debug.Log(message);
            }
            catch (Exception)
            {
                // catch any possible errors from parsing naughty responses!
                Debug.Log(query + "\n" + response);
            }
        }
    }
}