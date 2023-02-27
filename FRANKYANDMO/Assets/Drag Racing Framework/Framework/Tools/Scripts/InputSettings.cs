using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DR_INPUT_SYSTEM
using UnityEngine.InputSystem.Controls;
#endif

namespace GercStudio.DragRacingFramework
{
    [CreateAssetMenu(fileName = "Input", menuName = "Input -")]
    public class InputSettings : ScriptableObject
    {
#if DR_INPUT_SYSTEM
        public List<ButtonControl> keyboardButtonsInUnityInputSystem = new List<ButtonControl>();
        public List<ButtonControl> gamepadButtonsInUnityInputSystem = new List<ButtonControl>();
#endif
        public InputHelper.GamepadButtons[] gamepadButtonsInProjectSettings = new InputHelper.GamepadButtons[5];
        public InputHelper.KeyboardCodes[] keyboardButtonsInProjectSettings = new InputHelper.KeyboardCodes[5];
    }
}
