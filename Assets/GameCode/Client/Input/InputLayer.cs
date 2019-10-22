using System;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Input
{
	public abstract class InputLayer
	{
		public bool isActive;
		// if true then all input layers below this layer will be blocked
		public bool isBlocking = false;

		private Dictionary<KeyCode, string> m_KeyActionMappings;
		private Dictionary<MouseButton, string> m_MouseActionMappings;
		private Dictionary<GamepadButton, string> m_GamepadActionMappings;

		private List<Tuple<string, InputActionType, Action>> m_KeyActionEvents;
		private List<Tuple<string, InputActionType, Action>> m_MouseActionEvents;
		private List<Tuple<string, InputActionType, Action>> m_GamepadActionEvents;

		private Dictionary<string, Action<float>> m_AxisEventMappings;

		//private Dictionary<string, List<Tuple<KeyCode, float>>> m_AxisMappings;

		private List<Tuple<string, KeyCode, float>> m_KeyAxisMappings;
		private List<Tuple<string, string, float>> m_AxisMappings;

		public InputLayer(bool isBlocking)
		{
			this.isBlocking = isBlocking;

			isActive = false;

			m_KeyActionMappings = new Dictionary<KeyCode, string>();
			m_MouseActionMappings = new Dictionary<MouseButton, string>();
			m_GamepadActionMappings = new Dictionary<GamepadButton, string>();

			m_KeyActionEvents = new List<Tuple<string, InputActionType, Action>>();
			m_MouseActionEvents = new List<Tuple<string, InputActionType, Action>>();
			m_GamepadActionEvents = new List<Tuple<string, InputActionType, Action>>();

			m_KeyAxisMappings = new List<Tuple<string, KeyCode, float>>();
			m_AxisEventMappings = new Dictionary<string, Action<float>>();

			AddActionMappings();
			BindActions();
		}

		protected abstract void AddActionMappings();

		protected virtual void BindActions() { }

		public virtual void OnActivated() { isActive = true; }

		public virtual void OnDeactivated() { isActive = false; }

		public void AddActionMapping(string actionName, KeyCode keyCode)
		{
			if (!m_KeyActionMappings.ContainsKey(keyCode))
			{
				m_KeyActionMappings.Add(keyCode, actionName);
			}
			else
			{
				Debug.LogError("Action mapping already exists: " + keyCode);
			}
		}

		public void AddActionMapping(string actionName, MouseButton button)
		{
			if (!m_MouseActionMappings.ContainsKey(button))
			{
				m_MouseActionMappings.Add(button, actionName);
			}
			else
			{
				Debug.LogError("Action mapping already exists: " + button);
			}
		}

		public void AddActionMapping(string actionName, GamepadButton button)
		{
			if (!m_GamepadActionMappings.ContainsKey(button))
			{
				m_GamepadActionMappings.Add(button, actionName);
			}
			else
			{
				Debug.LogError("Action mapping already exists: " + button);
			}
		}

		public void AddAxisMapping(string axisName, KeyCode keyCode, float axisValue)
		{
			foreach (var mapping in m_KeyAxisMappings)
			{
				if (mapping.Item1 == axisName && mapping.Item2 == keyCode
					&& mapping.Item3 == axisValue)
				{
					Debug.LogError("Axismapping already exists!");
					return;
				}
			}

			m_KeyAxisMappings.Add(new Tuple<string, KeyCode, float>(axisName, keyCode, axisValue));
		}

		public void BindAxis(string axisName, Action<float> func)
		{
			foreach (var axisBinding in m_KeyAxisMappings)
			{
				if (axisBinding.Item1 == axisName)
				{
					m_AxisEventMappings.Add(axisName, func);
					break;
				}
			}
		}

		public void BindAction(string actionName, InputActionType actionType, Action func)
		{
			if (!IsActionAlreadyBound(actionName, actionType, m_KeyActionEvents))
			{
				var mapping = new Tuple<string, InputActionType, Action>(actionName, actionType, func);
				m_KeyActionEvents.Add(mapping);
			}

			if (!IsActionAlreadyBound(actionName, actionType, m_MouseActionEvents))
			{
				var mapping = new Tuple<string, InputActionType, Action>(actionName, actionType, func);
				m_MouseActionEvents.Add(mapping);
			}

			if (!IsActionAlreadyBound(actionName, actionType, m_GamepadActionEvents))
			{
				var mapping = new Tuple<string, InputActionType, Action>(actionName, actionType, func);
				m_GamepadActionEvents.Add(mapping);
			}
		}

		public bool InvokeKeyAction(KeyCode keyCode, InputActionType actionType)
		{
			// TODO(Anders): if action was hold or press then check if the keycode is bound to an axis
			// and invoke the action mapped to it

			//if (actionType == InputActionType.HOLD 
			//    || actionType == InputActionType.PRESS)
			//{
			//    foreach (var mapping in m_KeyAxisMappings)
			//    {
			//        if (mapping.Item2 == keyCode)
			//        {
			//            m_AxisEventMappings[mapping.Item1](mapping.Item3);
			//            break;
			//        }
			//    }
			//}

			foreach (var eventMapping in m_KeyActionEvents)
			{
				if (m_KeyActionMappings.ContainsKey(keyCode))
				{
					if (eventMapping.Item1 == m_KeyActionMappings[keyCode] &&
						eventMapping.Item2 == actionType)
					{
						eventMapping.Item3();
						return true;
					}
				}
			}

			return false;
		}

		public bool InvokeMouseAction(MouseButton button, InputActionType actionType)
		{
			foreach (var eventMapping in m_MouseActionEvents)
			{
				if (m_MouseActionMappings.ContainsKey(button))
				{
					if (m_MouseActionMappings[button] == eventMapping.Item1
						&& eventMapping.Item2 == actionType)
					{
						eventMapping.Item3();
						return true;
					}
				}
			}

			return false;
		}

		public bool InvokeGamepadAction(GamepadButton button, InputActionType actionType)
		{
			foreach (var eventMapping in m_GamepadActionEvents)
			{
				if (m_GamepadActionMappings.ContainsKey(button))
				{
					if (m_GamepadActionMappings[button] == eventMapping.Item1
						&& eventMapping.Item2 == actionType)
					{
						eventMapping.Item3();
						return true;
					}
				}
			}

			return false;
		}

		private bool IsActionAlreadyBound(string actionName, InputActionType actionType,
			List<Tuple<string, InputActionType, Action>> eventMappings)
		{
			foreach (var elem in eventMappings)
			{
				string name = elem.Item1;
				InputActionType type = elem.Item2;

				if (actionName == name && actionType == type)
				{
					return true;
				}
			}

			return false;
		}
	}
}

