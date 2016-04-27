// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;

namespace Atesh.WindowsAndWidgets.Extras
{
    public abstract class ProtectedSingletonWindow<InstanceType> : Window where InstanceType : Window
    {
        static InstanceType _Instance;
        protected static InstanceType Instance
        {
            get
            {
                if (!_Instance) throw new InvalidOperationException(WeNeedCreatedMessage.Strings.InstanceIsNotCreatedYet(typeof(InstanceType).Name));

                return _Instance;
            }
        }

        protected ProtectedSingletonWindow()
        {
            WindowCreated += ProtectedSingletonWindowCreated;
            //todo: TemplateCreated event'inde düzgün hata mesajı ile şunu yap: LogOptional("Singleton window 'name' is a template. Make sure it's created before using it")
            //TemplateCreated += TemplateCreated;
            Closing += WindowClosing;
        }

        public static void ShowInstance() => Instance.Show();

        public static void CloseInstance() => Instance.Close();

        void ProtectedSingletonWindowCreated(object Sender, EventArgs E)
        {
            if (_Instance) Debug.LogError(WeNeedCreatedMessage.Strings.OnlyOneInstanceAllowed(typeof(InstanceType).Name));
            else _Instance = GetComponent<InstanceType>();
        }

        static void WindowClosing(object Sender, WindowClosingEventArgs E) => E.Destroy = false;
    }
}