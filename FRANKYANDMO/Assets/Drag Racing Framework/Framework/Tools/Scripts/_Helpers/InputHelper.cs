using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DR_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif


namespace GercStudio.DragRacingFramework
{
    public static class InputHelper
    {

        public enum GamepadButtons
        {
            NotUse,
            NorthButton,
            SouthButton,
            WestButton,
            EastButton,
            LeftShoulder,
            RightShoulder,
            LeftTrigger,
            RightTrigger,
            DpadUpButton,
            DpadDownButton,
            DpadLeftButton,
            DpadRightButton,
            LeftStickButton,
            RightStickButton,
            StartButton,
            SelectButton
        }

        public enum KeyboardCodes
        {
            LeftMouseButton,
            RightMouseButton,
            MiddleMouseButton,
            Q, W, E, R, T, Y, U, I,
            O, P, A, S, D, F, G, H,
            J, K, L, Z, X, C, V, B,
            N, M, _1, _2, _3, _4, _5,
            _6, _7, _8, _9, _0, 
            Space, Backspace, LeftShift,
            RightShift, LeftCtrl, RightCtrl,
            LeftAlt, RightAlt, Tab,
            Escape, Enter, UpArrow,
            DownArrow, LeftArrow, RightArrow
        }
        
#if DR_INPUT_SYSTEM
        public static bool WasKeyboardButtonPressed(ButtonControl button)
        {
	        return Keyboard.current != null && button != null && button.wasPressedThisFrame;
        }

        public static bool IsKeyboardButtonPressed(ButtonControl button)
        {
	        return Keyboard.current != null && button != null && button.isPressed;
        }
        
        public static bool WasGamepadButtonPressed(ButtonControl button)
        {
	        return Gamepad.current != null && button != null && button.wasPressedThisFrame;
        }

        public static bool IsGamepadButtonPressed(ButtonControl button)
        {
	        return Gamepad.current != null && button != null && button.isPressed;
        }
#endif
        
        public static void InitializeGamepadButtons(InputSettings inputSettings)
		{
			
#if DR_INPUT_SYSTEM
			inputSettings.gamepadButtonsInUnityInputSystem.Clear();

			for (int i = 0; i < 5; i++)
			{
				inputSettings.gamepadButtonsInUnityInputSystem.Add(new ButtonControl());

				if (Gamepad.current == null) continue;
				
				switch (inputSettings.gamepadButtonsInProjectSettings[i])
				{
					case GamepadButtons.NotUse:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = null;
						break;
					case GamepadButtons.NorthButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.buttonNorth;
						break;
					case GamepadButtons.SouthButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.buttonSouth;
						break;
					case GamepadButtons.WestButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.buttonWest;
						break;
					case GamepadButtons.EastButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.buttonEast;
						break;
					case GamepadButtons.LeftShoulder:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.leftShoulder;
						break;
					case GamepadButtons.RightShoulder:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.rightShoulder;
						break;
					case GamepadButtons.LeftTrigger:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.leftTrigger;
						break;
					case GamepadButtons.RightTrigger:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.rightTrigger;
						break;
					case GamepadButtons.DpadUpButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.dpad.up;
						break;
					case GamepadButtons.DpadDownButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.dpad.down;
						break;
					case GamepadButtons.DpadLeftButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.dpad.left;
						break;
					case GamepadButtons.DpadRightButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.dpad.right;
						break;
					case GamepadButtons.LeftStickButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.leftStickButton;
						break;
					case GamepadButtons.RightStickButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.rightStickButton;
						break;
					case GamepadButtons.StartButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.startButton;
						break;
					case GamepadButtons.SelectButton:
						inputSettings.gamepadButtonsInUnityInputSystem[i] = Gamepad.current.selectButton;
						break;
				}
			}
#endif
		}

		public static void InitializeKeyboardAndMouseButtons(InputSettings inputSettings)
		{
			
#if DR_INPUT_SYSTEM
			inputSettings.keyboardButtonsInUnityInputSystem.Clear();
			
			for (int i = 0; i < 5; i++)
			{
				inputSettings.keyboardButtonsInUnityInputSystem.Add(new ButtonControl());

				if (Keyboard.current == null || Mouse.current == null) continue;
				
				switch (inputSettings.keyboardButtonsInProjectSettings[i])
				{
					// case KeyboardCodes.RightMouseButton:
					// 	inputSettings.keyboardButtonsInUnityInputSystem[i] = Mouse.current.rightButton;
					// 	break;
					// case KeyboardCodes.LeftMouseButton:
					// 	inputSettings.keyboardButtonsInUnityInputSystem[i] = Mouse.current.leftButton;
					// 	break;
					// case KeyboardCodes.MiddleMouseButton:
					// 	inputSettings.keyboardButtonsInUnityInputSystem[i] = Mouse.current.middleButton;
					// 	break;
					case KeyboardCodes.Q:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.qKey;
						break;
					case KeyboardCodes.W:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.wKey;
						break;
					case KeyboardCodes.E:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.eKey;
						break;
					case KeyboardCodes.R:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.rKey;
						break;
					case KeyboardCodes.T:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.tKey;
						break;
					case KeyboardCodes.Y:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.yKey;
						break;
					case KeyboardCodes.U:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.uKey;
						break;
					case KeyboardCodes.I:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.iKey;
						break;
					case KeyboardCodes.O:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.oKey;
						break;
					case KeyboardCodes.P:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.pKey;
						break;
					case KeyboardCodes.A:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.aKey;
						break;
					case KeyboardCodes.S:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.sKey;
						break;
					case KeyboardCodes.D:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.dKey;
						break;
					case KeyboardCodes.F:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.fKey;
						break;
					case KeyboardCodes.G:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.gKey;
						break;
					case KeyboardCodes.H:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.hKey;
						break;
					case KeyboardCodes.J:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.jKey;
						break;
					case KeyboardCodes.K:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.kKey;
						break;
					case KeyboardCodes.L:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.lKey;
						break;
					case KeyboardCodes.Z:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.zKey;
						break;
					case KeyboardCodes.X:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.xKey;
						break;
					case KeyboardCodes.C:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.cKey;
						break;
					case KeyboardCodes.V:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.vKey;
						break;
					case KeyboardCodes.B:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.bKey;
						break;
					case KeyboardCodes.N:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.nKey;
						break;
					case KeyboardCodes.M:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.mKey;
						break;
					case KeyboardCodes._0:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit0Key;
						break;
					case KeyboardCodes._1:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit1Key;
						break;
					case KeyboardCodes._2:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit2Key;
						break;
					case KeyboardCodes._3:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit3Key;
						break;
					case KeyboardCodes._4:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit4Key;
						break;
					case KeyboardCodes._5:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit5Key;
						break;
					case KeyboardCodes._6:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit6Key;
						break;
					case KeyboardCodes._7:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit7Key;
						break;
					case KeyboardCodes._8:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit8Key;
						break;
					case KeyboardCodes._9:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.digit9Key;
						break;
					case KeyboardCodes.Space:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.spaceKey;
						break;
					case KeyboardCodes.Backspace:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.backspaceKey;
						break;
					case KeyboardCodes.LeftShift:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.leftShiftKey;
						break;
					case KeyboardCodes.RightShift:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.rightShiftKey;
						break;
					case KeyboardCodes.LeftCtrl:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.leftCtrlKey;
						break;
					case KeyboardCodes.RightCtrl:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.rightCtrlKey;
						break;
					case KeyboardCodes.LeftAlt:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.leftAltKey;
						break;
					case KeyboardCodes.RightAlt:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.rightAltKey;
						break;
					case KeyboardCodes.Tab:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.tabKey;
						break;
					case KeyboardCodes.Enter:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.enterKey;
						break;
					case KeyboardCodes.Escape:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.escapeKey;
						break;
					case KeyboardCodes.UpArrow:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.upArrowKey;
						break;
					case KeyboardCodes.DownArrow:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.downArrowKey;
						break;
					case KeyboardCodes.LeftArrow:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.leftArrowKey;
						break;
					case KeyboardCodes.RightArrow:
						inputSettings.keyboardButtonsInUnityInputSystem[i] = Keyboard.current.rightArrowKey;
						break;
				}
			}
#endif
		}
    }
}
