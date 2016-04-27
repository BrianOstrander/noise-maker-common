// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atesh.Waitress.Extras
{
    public class WWW
    {
        internal const int DefaultTrialCount = 1;
        internal const float DefaultTrialInterval = 0;

        DoneCallback Done;
        CanceledCallback Canceled;
        UnityEngine.WWW Response;
        int TrialCount;
        float TrialInterval;
        Timer Timer;
        readonly Timer TimeoutTimer = new Timer();
        bool TimedOut;

        /// <summary>
        /// Unity uses default OS timeout duration for web requests. This field can be used to reduce the default timeout value but it can't be used to increase it.
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once ConvertToConstant.Global
        public static float Timeout = float.MaxValue;

        public bool Active => Response != null;

        WWW()
        {
        }

        IEnumerator Run(MonoBehaviour Host, string Url, WWWForm Form, Dictionary<string, string> Headers)
        {
            var TrialNo = 0;
            Trial:
            TrialNo++;
            if (TrialNo > TrialCount) yield break;

            if (Form == null) Response = new UnityEngine.WWW(Url, null, Headers);
            else
            {
                // Add form headers to custom headers.
                foreach (var FormHeader in Form.headers)
                {
                    Headers.Add(FormHeader.Key,FormHeader.Value);
                }

                Response = new UnityEngine.WWW(Url, Form.data, Headers);
            }

            TimeoutTimer.Wait(Host, Timeout, UnscaledTime: true, Done: (Sender, E) =>
            {
                if (E.Canceled) return;

                TimedOut = true;
                Cancel();
            });
            try
            {
                while (!Response.isDone)
                {
                    yield return null;

                    if (Response == null)
                    {
                        Canceled?.Invoke(TimedOut);
                        yield break;
                    }
                }

                if (Response.error == null) Done?.Invoke(Response);
                else
                {
                    if (TrialNo == TrialCount) Done?.Invoke(Response);
                    else
                    {
                        Debug.Log(Strings.WebRequestHasFailedAndWillRetry(Url, Response.error));

                        if (TrialInterval <= 0) yield return null;
                        else
                        {
                            Timer = new Timer();
                            yield return Timer.Wait(Host, TrialInterval, UnscaledTime: true);
                        }
                        goto Trial;
                    }
                }
            }
            finally
            {
                if (TimeoutTimer.Active) TimeoutTimer.Cancel();
            }
        }

        static WWW InternalRequest(MonoBehaviour Host, string Url, WWWForm Form = null, Dictionary<string, string> Headers = null, DoneCallback Done = null, CanceledCallback Canceled = null, int TrialCount = DefaultTrialCount, float TrialInterval = DefaultTrialInterval)
        {
            if (!Host) Host = DefaultHost.Instance;

            var Result = new WWW { Done = Done, Canceled = Canceled, TrialCount = TrialCount, TrialInterval = TrialInterval };
            Host.StartCoroutine(Result.Run(Host, Url, Form, Headers));

            return Result;
        }

        [ValidateParameters]
        public static WWW Request([NotNull] MonoBehaviour Host, string Url, WWWForm Form = null, Dictionary<string, string> Headers = null, DoneCallback Done = null, CanceledCallback Canceled = null, [NotLessThan(1)] int TrialCount = DefaultTrialCount, [NotLessThan(0f)] float TrialInterval = DefaultTrialInterval) => InternalRequest(Host, Url, Form, Headers, Done, Canceled, TrialCount, TrialInterval);
        [ValidateParameters]
        public static WWW Request(string Url, WWWForm Form = null, Dictionary<string, string> Headers = null, DoneCallback Done = null, CanceledCallback Canceled = null, [NotLessThan(1)] int TrialCount = DefaultTrialCount, [NotLessThan(0f)] float TrialInterval = DefaultTrialInterval) => InternalRequest(null, Url, Form, Headers, Done, Canceled, TrialCount, TrialInterval);

        public void Cancel()
        {
            if (Response == null) throw new InvalidOperationException(Atesh.Strings.NotActive($"{nameof(WWW)} request"));

            if (Timer != null && Timer.Active) Timer.Cancel();
            if (TimeoutTimer.Active) TimeoutTimer.Cancel();
            Response.Dispose();
            Response = null;
        }
    }
}