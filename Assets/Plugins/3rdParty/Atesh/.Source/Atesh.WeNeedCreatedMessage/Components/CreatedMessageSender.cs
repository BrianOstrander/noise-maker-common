// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Atesh.WeNeedCreatedMessage.Extras;

namespace Atesh.WeNeedCreatedMessage
{
    public class CreatedMessageSender : ProtectedSingletonBehaviour<CreatedMessageSender>
    {
        internal class Receiver
        {
            internal string ScriptFullName;
            internal Action<bool> Created;
        }

        internal static bool Present => Instance;

        static readonly Dictionary<int, Receiver> Receivers = new Dictionary<int, Receiver>();

        protected override void CreatedMessageReceiverAwake() => SendMessages(true);

        internal static void RegisterReceiver(int Id, Receiver Receiver)
        {
            // Replace the receiver by id if it's registered already. Unity creates the object more than once! We only keep the last one.
            Receivers.Remove(Id);
            Receivers.Add(Id, Receiver);
        }

        internal static void SendMessages(bool CallingAfterLateUpdate)
        {
            while (Receivers.Count > 0)
            {
                var ReceiversToBeProcessed = Receivers.ToList();

                // We need to clear the receivers here in case of new receivers may be created in message loop below.
                // We'll process them with main while loop.
                Receivers.Clear();

                foreach (var Receiver in Sort(ReceiversToBeProcessed))
                {
                    try
                    {
                        Receiver.Value.Created(CallingAfterLateUpdate);
                    }
                    catch (Exception E)
                    {
                        Debug.LogError(Strings.ExceptionInCreatedMessage(Receiver.Value.ScriptFullName, E));
                    }
                }
            }
        }

        static IEnumerable<KeyValuePair<int, Receiver>> Sort(List<KeyValuePair<int, Receiver>> Receivers)
        {
            var Scripts = ScriptExecutionOrder.Instance.Scripts;
            var IndexOfDefaultTime = Scripts.IndexOf(Strings.DefaultTime);

            Receivers.Sort((A, B) =>
            {
                #region Nested Methods
                var CheckSpecialIndices = new Func<Receiver, int>(Receiver =>
                 {
                     if (Receiver.ScriptFullName == typeof(CreatedMessageSender).FullName) return int.MinValue;
                     if (Receiver.ScriptFullName == typeof(LateCreatedMessageSender).FullName) return int.MinValue + 1;
                     return IndexOfDefaultTime;
                 });
                #endregion

                var IndexOfA = Scripts.IndexOf(A.Value.ScriptFullName);
                var IndexOfB = Scripts.IndexOf(B.Value.ScriptFullName);

                if (IndexOfA < 0) IndexOfA = CheckSpecialIndices(A.Value);
                if (IndexOfB < 0) IndexOfB = CheckSpecialIndices(B.Value);

                return IndexOfA.CompareTo(IndexOfB);
            });

            return Receivers;
        }
    }
}