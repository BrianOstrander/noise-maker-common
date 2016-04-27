// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

namespace Atesh.WindowsAndWidgets.Extras
{
    public abstract class SingletonWindow<InstanceType> : ProtectedSingletonWindow<InstanceType> where InstanceType : Window
    {
        public static new InstanceType Instance => ProtectedSingletonWindow<InstanceType>.Instance;
    }
}