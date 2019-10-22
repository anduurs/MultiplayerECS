using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FNZ.Client.Input
{
	public enum InputActionType
	{
		PRESS,
		RELEASE,
		HOLD,
		NONE
	}

	public enum MouseButton
	{
		LEFT = 0,
		RIGHT = 1,
		MIDDLE = 2
	}

	public enum GamepadButton
	{
		FACE_BTN_DOWN,
		FACE_BTN_RIGHT,
		FACE_BTN_LEFT,
		FACE_BTN_UP,
		RIGHT_BUMPER,
		LEFT_BUMPER,
		START,
		BACK,
		LEFT_STICK_BTN,
		RIGHT_STICK_BTN,
		RIGHT_TRIGGER,
		LEFT_TRIGGER
	}

	public class InputManager
	{
		private List<InputLayer> m_LayerStack;

		private KeyCode[] m_KeyCodes;
		private MouseButton[] m_MouseButtons;
		private GamepadButton[] m_GamepadButtons;

		private Dictionary<GamepadButton, string> m_GamepadMappings;

		public void Initialize()
		{
			m_KeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
			m_MouseButtons = (MouseButton[])Enum.GetValues(typeof(MouseButton));
			m_GamepadButtons = (GamepadButton[])Enum.GetValues(typeof(GamepadButton));

			m_GamepadMappings = new Dictionary<GamepadButton, string>
			{
				{ GamepadButton.FACE_BTN_DOWN,   "Face_Btn_Down" },
				{ GamepadButton.FACE_BTN_RIGHT,  "Face_Btn_Right"},
				{ GamepadButton.FACE_BTN_LEFT,   "Face_Btn_Left" },
				{ GamepadButton.FACE_BTN_UP,     "Face_Btn_Up" },
				{ GamepadButton.LEFT_BUMPER,     "Left_Bumper" },
				{ GamepadButton.RIGHT_BUMPER,    "Right_Bumper"},
				{ GamepadButton.BACK,            "Back" },
				{ GamepadButton.START,           "Start" },
				{ GamepadButton.LEFT_STICK_BTN,  "Left_Stick_Btn"},
				{ GamepadButton.RIGHT_STICK_BTN, "Right_Stick_Btn" },
				{ GamepadButton.RIGHT_TRIGGER,   "Right_Trigger" },
				{ GamepadButton.LEFT_TRIGGER,    "Left_Trigger" }
			};

			m_LayerStack = new List<InputLayer>();
		}


	}
}

