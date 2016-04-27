// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using Atesh.MagicAutoLinker;
using Atesh.WeNeedCreatedMessage;
using UnityEngine;
using UnityEngine.UI;

namespace Atesh.WindowsAndWidgets
{
    public class WindowFrame : CreatedMessageReceiver
    {
        #region AutoLinks
#pragma warning disable 649
        [AutoLink] //todo:(Description = Strings.WindowMustHaveClientArea)]
        internal Button CloseButton;
#pragma warning restore 649
        #endregion

        Window Parent;
        RectTransform RectTransform;

        Image _Image;
        internal Image Image
        {
            get
            {
                //todo: Bulunamazsa log.error yada exception göstermesi lazım
                if (!_Image) _Image = GetComponent<Image>();
                return _Image;
            }
        }

        //todo: Created her yerde events region'ın öncesinde olsun.
        protected override void Created(bool OnSceneLoad)
        {
            gameObject.SetActive(Parent);

            if (!Parent) return;

            //todo: Bulunamazsa log.error yada exception göstermesi lazım
            RectTransform = GetComponent<RectTransform>();
            RectTransform.Maximize();

            CloseButton.onClick.AddListener(CloseButtonClick);
        }

        internal WindowFrame Create(Window Parent)
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Parent) throw new Exception("parent null olamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (gameObject.activeInHierarchy) throw new Exception("style aktif olmamalı");
            //todo: Yukardaki hata Debug.LogError'mu olmalı yoksa LogError yapılan başka yerler de throw exception'mu olmalı? Cevap: Her yeri kontrol et. Gereken yerleri burdaki gibi exception yap.

            var Result = Instantiate(this);
            Result.transform.SetParent(Parent.transform, false);
            Result.transform.SetAsFirstSibling();
            Result.name = "Frame";
            Result.Parent = Parent;

            return Result;
        }

        void CloseButtonClick() => Parent.Close();
    }
}