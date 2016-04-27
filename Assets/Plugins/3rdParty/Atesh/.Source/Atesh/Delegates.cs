// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public delegate DataType TransformProcessCallback<DataType>(Transform Transform, DataType Data);
    public delegate void InstanceCreatedEventHandler<in InstanceType>(InstanceType Instance);
}