﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace LunraGames.Toggler {
	public class TogglerButtonGroup : UIBehaviour
	{
		[SerializeField] private bool m_AllowSwitchOff = false;
		public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

		private List<TogglerButton> m_Toggles = new List<TogglerButton>();

		private void ValidateToggleIsInGroup(TogglerButton toggle)
		{
			if (toggle == null || !m_Toggles.Contains(toggle))
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", toggle, this));
		}

		public void NotifyToggleOn(TogglerButton toggle)
		{
			ValidateToggleIsInGroup(toggle);

			// disable all toggles in the group
			for (var i = 0; i < m_Toggles.Count; i++)
			{
				if (m_Toggles[i] == toggle)
					continue;

				m_Toggles[i].isOn = false;
			}
		}

		public void UnregisterToggle(TogglerButton toggle)
		{
			if (m_Toggles.Contains(toggle))
				m_Toggles.Remove(toggle);
		}

		public void RegisterToggle(TogglerButton toggle)
		{
			if (!m_Toggles.Contains(toggle))
				m_Toggles.Add(toggle);
		}

		public bool AnyTogglesOn()
		{
			return m_Toggles.Find(x => x.isOn) != null;
		}

		public IEnumerable<TogglerButton> ActiveToggles()
		{
			return m_Toggles.Where(x => x.isOn);
		}

		public void SetAllTogglesOff()
		{
			bool oldAllowSwitchOff = m_AllowSwitchOff;
			m_AllowSwitchOff = true;

			for (var i = 0; i < m_Toggles.Count; i++)
				m_Toggles[i].isOn = false;

			m_AllowSwitchOff = oldAllowSwitchOff;
		}
	}
}