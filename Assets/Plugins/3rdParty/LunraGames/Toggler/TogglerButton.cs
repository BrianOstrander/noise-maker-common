using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
using Atesh;

namespace LunraGames.Toggler {

	public class TogglerButton : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement 
	{
		public enum TogglerButtonTransition
		{
			None,
			Fade
		}

		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{}

		[Serializable]
		public class OnClickEvent : UnityEvent
		{}

		/// <summary>
		/// Transition type.
		/// </summary>
		public TogglerButtonTransition ToggleOnTransition = TogglerButtonTransition.Fade;
		public TogglerButtonTransition ToggleOffTransition = TogglerButtonTransition.Fade;
		public ColorBlock ToggleOnColors = new ColorBlock {normalColor = Color.white, highlightedColor = Color.white, pressedColor = Color.gray, disabledColor = Color.gray.NewA(0.5f), colorMultiplier = 1f, fadeDuration = 0.1f};
		public ColorBlock ToggleOffColors = new ColorBlock {normalColor = Color.white, highlightedColor = Color.white, pressedColor = Color.gray, disabledColor = Color.gray.NewA(0.5f), colorMultiplier = 1f, fadeDuration = 0.1f};

		public TogglerButtonTransition ToggleTransition
		{
			get 
			{
				if (isOn) return ToggleOnTransition;
				else return ToggleOffTransition;
			}
		}

		public ColorBlock Colors
		{
			get
			{
				if (isOn) return ToggleOnColors;
				else return ToggleOffColors;
			}
		}


		// group that this toggle can belong to
		[SerializeField]
		TogglerButtonGroup m_Group;

		public TogglerButtonGroup Group
		{
			get { return m_Group; }
			set
			{
				m_Group = value;
				#if UNITY_EDITOR
				if (Application.isPlaying)
				#endif
				{
					SetToggleGroup(m_Group, true);
					PlayEffect(true);
				}
			}
		}

		/// <summary>
		/// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
		/// </summary>
		public ToggleEvent OnValueChanged = new ToggleEvent();
		public OnClickEvent OnClick = new OnClickEvent();

		// Whether the toggle is on
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField]
		bool m_IsOn;

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			Set(m_IsOn, false);
			PlayEffect(ToggleTransition == TogglerButtonTransition.None);

			var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
			if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		#endif // if UNITY_EDITOR

		public virtual void Rebuild(CanvasUpdate executing)
		{
			#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout)
				OnValueChanged.Invoke(m_IsOn);
			#endif
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetToggleGroup(m_Group, false);
			PlayEffect(true);
		}

		protected override void OnDisable()
		{
			SetToggleGroup(null, false);
			base.OnDisable();
		}

		private void SetToggleGroup(TogglerButtonGroup newGroup, bool setMemberValue)
		{
			TogglerButtonGroup oldGroup = m_Group;

			// Sometimes IsActive returns false in OnDisable so don't check for it.
			// Rather remove the toggle too oftem than too little.
			if (m_Group != null)
				m_Group.UnregisterToggle(this);

			// At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
			// That's why we use the setMemberValue parameter.
			if (setMemberValue)
				m_Group = newGroup;

			// Only register to the new group if this Toggle is active.
			if (m_Group != null && IsActive())
				m_Group.RegisterToggle(this);

			// If we are in a new group, and this toggle is on, notify group.
			// Note: Don't refer to m_Group here as it's not guaranteed to have been set.
			if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
				m_Group.NotifyToggleOn(this);
		}

		/// <summary>
		/// Whether the toggle is currently active.
		/// </summary>
		public bool isOn
		{
			get { return m_IsOn; }
			set
			{
				
				Set(value);
			}
		}

		void Set(bool value)
		{
			Set(value, true);
		}

		void Set(bool value, bool sendCallback)
		{
			if (m_IsOn == value)
				return;

			// if we are in a group and set to true, do group logic
			m_IsOn = value;
			if (m_Group != null && IsActive())
			{
				if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
				{
					m_IsOn = true;
					m_Group.NotifyToggleOn(this);
				}
			}

			// Always send event when toggle is clicked, even if value didn't change
			// due to already active toggle in a toggle group being clicked.
			// Controls like SelectionList rely on this.
			// It's up to the user to ignore a selection being set to the same value it already was, if desired.
			PlayEffect(ToggleTransition == TogglerButtonTransition.None);
			if (sendCallback)
				OnValueChanged.Invoke(m_IsOn);
		}

		/// <summary>
		/// Play the appropriate effect.
		/// </summary>
		private void PlayEffect(bool instant)
		{
			DoStateTransition(currentSelectionState, instant);
		}

		/// <summary>
		/// Assume the correct visual state.
		/// </summary>
		protected override void Start()
		{
			PlayEffect(true);
		}

		void InternalToggle()
		{
			if (IsActive() || IsInteractable()) isOn = !isOn;
		}

		/// <summary>
		/// React to clicks.
		/// </summary>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
            if (!interactable) return;

			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			InternalToggle();
			OnClick.Invoke();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			InternalToggle();
		}


		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			Color tintColor;
			Sprite transitionSprite;
			string triggerName;

			switch (state)
			{
			case SelectionState.Normal:
				tintColor = Colors.normalColor;
				transitionSprite = null;
				triggerName = animationTriggers.normalTrigger;
				break;
			case SelectionState.Highlighted:
				tintColor = Colors.highlightedColor;
				transitionSprite = spriteState.highlightedSprite;
				triggerName = animationTriggers.highlightedTrigger;
				break;
			case SelectionState.Pressed:
				tintColor = Colors.pressedColor;
				transitionSprite = spriteState.pressedSprite;
				triggerName = animationTriggers.pressedTrigger;
				break;
			case SelectionState.Disabled:
				tintColor = Colors.disabledColor;
				transitionSprite = spriteState.disabledSprite;
				triggerName = animationTriggers.disabledTrigger;
				break;
			default:
				tintColor = Color.black;
				transitionSprite = null;
				triggerName = string.Empty;
				break;
			}

			if (gameObject.activeInHierarchy)
			{
				switch (transition)
				{
				case Transition.ColorTint:
					StartColorTween(tintColor * Colors.colorMultiplier, instant);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(transitionSprite);
					break;
				case Transition.Animation:
					TriggerAnimation(triggerName);
					break;
				}
			}
		}

		void StartColorTween(Color targetColor, bool instant)
		{
			if (targetGraphic == null)
				return;

			targetGraphic.CrossFadeColor(targetColor, instant ? 0f : Colors.fadeDuration, true, true);
		}

		void DoSpriteSwap(Sprite newSprite)
		{
			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		void TriggerAnimation(string triggername)
		{
			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(animationTriggers.normalTrigger);
			animator.ResetTrigger(animationTriggers.pressedTrigger);
			animator.ResetTrigger(animationTriggers.highlightedTrigger);
			animator.ResetTrigger(animationTriggers.disabledTrigger);
			animator.SetTrigger(triggername);
		}

		public void LayoutComplete()
        {
			return;
        }

        public void GraphicUpdateComplete()
        {
			return;
        }
    }
}
