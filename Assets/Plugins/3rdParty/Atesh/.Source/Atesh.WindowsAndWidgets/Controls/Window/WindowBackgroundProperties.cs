using System;
using UnityEngine;
using UnityEngine.UI;

namespace Atesh.WindowsAndWidgets
{
    //todo: Drawer yöntemi ile felan bu properties class'ı için custom inspector yapılabiliyosa yap. OverrideColor'ı Color ile aynı satırda göster.
    [Serializable]
    public class WindowBackgroundProperties
    {
        #region Inspector
        //todo: Bunlar get yapıldığında override değilse Style'dan gelen bilgileri döndürmeliler.
        public bool OverrideImage;
        public Sprite Image;
        public bool OverrideColor;
        public Color Color;
        //todo: Material
        public bool OverrideImageType;
        public Image.Type ImageType;
        //todo: FillCenter
        #endregion

        internal Window Owner;
        internal Image FrameImage;

        bool LastOverrideImage;
        bool LastOverrideColor;
        bool LastOverrideImageType;

        internal void Update()
        {
            if (OverrideImage)
            {
                if (FrameImage.sprite != Image) FrameImage.sprite = Image;
            }
            else if (LastOverrideImage) FrameImage.sprite = Owner.Style.Image.sprite;

            if (OverrideColor)
            {
                if (FrameImage.color != Color) FrameImage.color = Color;
            }
            else if (LastOverrideColor) FrameImage.color = Owner.Style.Image.color;

            if (OverrideImageType)
            {
                if (FrameImage.type != ImageType) FrameImage.type = ImageType;
            }
            else if (LastOverrideImageType) FrameImage.type = Owner.Style.Image.type;

            LastOverrideImage = OverrideImage;
            LastOverrideColor = OverrideColor;
            LastOverrideImageType = OverrideImageType;
        }
    }
}