// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using Atesh.MagicAutoLinker;
using Atesh.Waitress;
using Atesh.WeNeedCreatedMessage;
using UnityEngine;

namespace Atesh.WindowsAndWidgets
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Window : CreatedMessageReceiver
    {
        #region Inspector
        // ReSharper disable ConvertToConstant.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        public WindowFrame Style;
        public bool IsTemplate;
        public bool ShowOnCreate;
        public WindowState State;
        public bool CloseButton = false;
        public WindowBackgroundProperties BackgroundProperties;
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore ConvertToConstant.Global
        #endregion

        #region AutoLinks
        [AutoLink(Description = Strings.WindowMustHaveClientArea)]
        internal RectTransform ClientArea;
        #endregion

        public static Transform DefaultParent;

        RectTransform RectTransform;
        WindowFrame Frame;
        bool Loading;
        bool Initialized;
        bool Hidden;
        float SavedOpacity;
        WindowState LastState = WindowState.Normal;
        Vector2 SavedAnchorMin;
        Vector2 SavedAnchorMax;
        Vector2 SavedOffsetMin;
        Vector2 SavedOffsetMax;
        Rect LastRect = new Rect(float.MinValue, float.MinValue, float.MinValue, float.MinValue);

        [SerializeField]
        [HideInInspector]
        bool TemplateAutoLinkerWasSelfDestroy;

        CanvasGroup _CanvasGroup;
        CanvasGroup CanvasGroup
        {
            get
            {
                if (!_CanvasGroup)
                {
                    _CanvasGroup = GetComponent<CanvasGroup>();
                    if (!_CanvasGroup) throw new InvalidOperationException(Strings.MissingComponent(name, nameof(CanvasGroup)));
                }

                return _CanvasGroup;
            }
        }

        //todo: Inspector'den kontrol edilmesi gerekiyo mu?
        public float Opacity
        {
            get
            {
                return Hidden ? SavedOpacity : CanvasGroup.alpha;
            }
            set
            {
                if (Hidden) SavedOpacity = value;
                else CanvasGroup.alpha = value;
            }
        }

        public event EventHandler WindowCreated;
        public event EventHandler Showing;
        public event EventHandler Shown;
        public event EventHandler Updating;
        public event EventHandler Updated;
        public event EventHandler Resize;
        public event EventHandler<WindowClosingEventArgs> Closing;
        public event EventHandler Closed;

        protected virtual void OnWindowCreated() => WindowCreated?.Invoke(this, EventArgs.Empty);
        protected virtual void OnShowing() => Showing?.Invoke(this, EventArgs.Empty);
        protected virtual void OnShown() => Shown?.Invoke(this, EventArgs.Empty);
        protected virtual void OnUpdating() => Updating?.Invoke(this, EventArgs.Empty);
        protected virtual void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);
        protected virtual void OnResize() => Resize?.Invoke(this, EventArgs.Empty);
        protected virtual void OnClosing(WindowClosingEventArgs E) => Closing?.Invoke(this, E);
        protected virtual void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);

        protected override void Created(bool OnSceneLoad)
        {
            if (OnSceneLoad == false && gameObject.activeInHierarchy) throw new InvalidOperationException(Strings.WindowMustBeInactiveWhenCreated(name));

            var AutoLinker = GetComponent<AutoLinker>();
            if (!AutoLinker && (transform.IsPrefab() || !TemplateAutoLinkerWasSelfDestroy)) throw new InvalidOperationException(Strings.MissingComponent(name, nameof(AutoLinker)));

            // Prefab objects don't receive created message but template windows does (via Window.Create<> method)
            // In this case, we don't want to deactivate template window prefabs.
            if (!transform.IsPrefab()) gameObject.SetActive(false);

            if (IsTemplate)
            {
                TemplateAutoLinkerWasSelfDestroy = AutoLinker.SelfDestroy;
                if (!TemplateAutoLinkerWasSelfDestroy) LogOptional(Strings.AutoLinkerSelfDestroyIsGoodForTemplate);

                Initialized = true;

                //todo: WindowCreated gibi TemplateCreated event???  SingletonWindow için gerekli.

                return;
            }

            //todo: Bu AutoLink olabilir.
            RectTransform = GetComponent<RectTransform>();
            if (!RectTransform) throw new InvalidOperationException(Strings.MissingComponent(name, nameof(RectTransform)));

            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (GetComponentsInChildren<WindowFrame>(true).Length > 0) throw new Exception("Pencere kendisi frame barındıramaz");

            if (!Style) Style = Settings.WindowStyle;
            Frame = Style.Create(this);

            BackgroundProperties.Owner = this;
            BackgroundProperties.FrameImage = Frame.Image;

            Initialized = true;

            //todo: Loading adını değiştir
            //todo: Loading'in kontrol edildiği yerlerdeki stringleri değiştir.
            Loading = true;
            OnWindowCreated();
            Loading = false;

            if (ShowOnCreate)
            {
                if (OnSceneLoad) Show();
                // Wait for end of frame before show.
                else new Framer().Wait(DefaultHost.Instance, true, (Sender, E) => Show());
            }
        }

        //todo: Place this into a seperate class
        static void LogOptional(string Message)
        {
            if (!Settings.ShowOptionalDebugInfos) return;

            Message += $" {Strings.OptionalInfoLogsCanBeDisabled}";
            Debug.Log(Message);
        }

        protected void Start() => UpdateProperties();

        protected void Update()
        {
            OnUpdating();

            UpdateProperties();

            if (RectTransform.rect != LastRect)
            {
                LastRect = RectTransform.rect;
                OnResize();
            }

            OnUpdated();
        }

        void UpdateProperties()
        {
            Frame.CloseButton.gameObject.SetActive(CloseButton);

            //todo: Refactor this into a method
            if (State != LastState)
            {
                if (LastState == WindowState.Normal) SaveRect();

                switch (State)
                {
                case WindowState.Normal:
                    RestoreRect();
                    break;
                //todo: Minimized
                /*
                case WindowState.Minimized:
                    break;
                */
                case WindowState.Maximized:
                    RectTransform.Maximize();
                    break;
                }

                LastState = State;
            }

            BackgroundProperties.Update();
        }

        void SaveRect()
        {
            SavedAnchorMin = RectTransform.anchorMin;
            SavedAnchorMax = RectTransform.anchorMax;
            SavedOffsetMin = RectTransform.offsetMin;
            SavedOffsetMax = RectTransform.offsetMax;
        }

        void RestoreRect()
        {
            RectTransform.anchorMin = SavedAnchorMin;
            RectTransform.anchorMax = SavedAnchorMax;
            RectTransform.offsetMin = SavedOffsetMin;
            RectTransform.offsetMax = SavedOffsetMax;
        }

        public static T Create<T>(T Template, Transform Parent = null) where T : Window
        {
            //todo: Template null olamaz (yada default template kullanılmalı)
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Template.IsTemplate) throw new Exception("Template olmayan create edilemez");

            // Prefabs don't receive created message so we call it for initialization.
            if (Template.transform.IsPrefab()) Template.Created(false);

            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Template.Initialized) throw new Exception("Template valid değil. Lütfen hataları giderin");

            // Scene templates deactivate themselves on created event but prefab templates don't.
            // We deactivate them here temporarily for instantiating.
            var TemplateWasActive = false;
            if (Template.transform.IsPrefab())
            {
                TemplateWasActive = Template.gameObject.activeSelf;
                Template.gameObject.SetActive(false);
            }
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            else if (Template.gameObject.activeInHierarchy) throw new Exception("Template inaktif olmalı");

            var Result = Instantiate(Template);
            Result.transform.SetParent(Parent ?? (DefaultParent ?? Template.transform.parent), false);
            Result.IsTemplate = false;

            if (TemplateWasActive) Template.gameObject.SetActive(true);

            return Result;
        }

        public void Show()
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (gameObject.activeInHierarchy) throw new InvalidOperationException("Already active");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (Loading) throw new Exception("load event'inde show/hide yapılamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (IsTemplate) throw new Exception("Template can't be shown");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Initialized) throw new Exception("window initialization tamamlanmadan show edilemez. Create ve Show'un aynı frame'de çağırılmadığına emin olun");

            OnShowing();

            BringToFront();
            gameObject.SetActive(true);
            Unhide();

            OnShown();
        }

        //todo: .Net gibi Control class'ından gelsin bu.
        public void Hide()
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (Loading) throw new Exception("load event'inde show/hide yapılamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (IsTemplate) throw new Exception("Template hide edilemez");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Initialized) throw new Exception("initialized değil");

            if (Hidden) return;

            SavedOpacity = Opacity;
            Opacity = 0;
            Hidden = true;

            //todo: VisibilityChanged event???
        }

        public void Unhide()
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (Loading) throw new Exception("load event'inde show/hide yapılamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (IsTemplate) throw new Exception("Template hide edilemez");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!Initialized) throw new Exception("initialized değil");

            if (!Hidden) return;

            Hidden = false;
            Opacity = SavedOpacity;

            //todo: VisibilityChanged event???
        }

        //todo: OnDisable'da bunu çağır biz deactivate etmediysek
        public void Deactivate()
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (Loading) throw new Exception("load event'inde show/hide yapılamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (IsTemplate) throw new Exception("Template deactive edilemez");

            Hide();
            gameObject.SetActive(false);
        }

        public void Close()
        {
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (!gameObject.activeInHierarchy) throw new InvalidOperationException("Already inactive");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (Loading) throw new Exception("load event'inde close yapılamaz");
            //todo: Uygun bi exception olsun. Hata text'ini Strings'e taşı
            if (IsTemplate) throw new Exception("Template close edilemez");

            var E = new WindowClosingEventArgs { Destroy = true };
            OnClosing(E);

            if (E.Cancel) return;
            
            Deactivate();
            OnClosed();

            if (E.Destroy) Destroy(gameObject);
        }

        public void BringToFront() => transform.SetAsLastSibling();
    }
}