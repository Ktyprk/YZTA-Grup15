using UnityEngine;
using UnityEngine.InputSystem;


public class ControlsManager : MonoBehaviour
{
        public static PlayerControls Controls;
        private static PlayerInput _playerInput;

        public static string CurrentDevice
        {
            get
            {
                string device = _playerInput.currentControlScheme;

                if (device == "Gamepad") device = "Xbox";

                return device;
            }
        }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            
            Controls = new PlayerControls();
            Controls.Player.Enable();
            Controls.UI.Enable();
        }

        private void OnDestroy()
        {
            Controls.Disable();
        }
}