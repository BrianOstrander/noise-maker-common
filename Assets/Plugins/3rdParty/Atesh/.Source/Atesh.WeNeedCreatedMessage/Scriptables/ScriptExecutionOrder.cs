// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;

namespace Atesh.WeNeedCreatedMessage
{
    public class ScriptExecutionOrder : SingletonScriptable<ScriptExecutionOrder>
    {
        #region Inspector
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        // Normally, We don't create the list since Unity creates inspector collections but we have to create it here because we access it in Install method before Unity does its job.
        public List<string> Scripts = new List<string>();
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        #endregion

        public const string AssetName = "CreatedMessageScriptExecutionOrder";
        public const string AssetFolder = nameof(Atesh);

        static ScriptExecutionOrder()
        {
            AssetNameToLoad = $"{AssetFolder}/{AssetName}";
        }

        public static void Install()
        {
            // We access Instance to trigger it to create its resource asset.
            // ReSharper disable once ConvertMethodToExpressionBody
            if (!Instance.Scripts.Contains(Strings.DefaultTime)) Instance.Scripts.Add(Strings.DefaultTime);
        }
    }
}