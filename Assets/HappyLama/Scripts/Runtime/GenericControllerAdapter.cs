using UnityEngine;

namespace HappyLama
{

    [System.Serializable]
    public class GenericControllerAdapter : MonoBehaviour, IMovementController
    {
        [Header("Controller Settings")]
        [SerializeField] private MonoBehaviour targetController;
        [SerializeField] private string movementFieldName = "canMove";

        private System.Reflection.FieldInfo movementField;
        private System.Reflection.PropertyInfo movementProperty;
        private bool originalValue = true;

        public bool CanMove
        {
            get => GetMovementValue();
            set => SetMovementValue(value);
        }

        public GameObject GameObject => gameObject;

        private void Start()
        {
            if (targetController != null)
            {
                var type = targetController.GetType();

                movementField = type.GetField(movementFieldName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (movementField == null)
                {
                    movementProperty = type.GetProperty(movementFieldName,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                }

                originalValue = GetMovementValue();

                if (movementField == null && movementProperty == null)
                {
                    Debug.LogError($"Could not find field or property '{movementFieldName}' in {targetController.GetType().Name}");
                }
            }
        }

        private bool GetMovementValue()
        {
            if (targetController == null) return false;

            try
            {
                if (movementField != null)
                {
                    return (bool)movementField.GetValue(targetController);
                }
                else if (movementProperty != null && movementProperty.CanRead)
                {
                    return (bool)movementProperty.GetValue(targetController);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting movement value: {e.Message}");
            }

            return false;
        }

        private void SetMovementValue(bool value)
        {
            if (targetController == null) return;

            try
            {
                if (movementField != null)
                {
                    movementField.SetValue(targetController, value);
                }
                else if (movementProperty != null && movementProperty.CanWrite)
                {
                    movementProperty.SetValue(targetController, value);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error setting movement value: {e.Message}");
            }
        }

        private void OnValidate()
        {
            if (targetController == null)
            {
                targetController = GetComponent<MonoBehaviour>();
            }
        }
    }

    public class StandardAssetsFirstPersonAdapter : MonoBehaviour, IMovementController
    {
        private MonoBehaviour firstPersonController;
        private MonoBehaviour mouseLook;
        private System.Reflection.FieldInfo walkingField;
        private System.Reflection.PropertyInfo mouseSensitivityProperty;
        private float originalMouseSensitivity;

        public bool CanMove { get; set; } = true;
        public GameObject GameObject => gameObject;

        private void Start()
        {

            firstPersonController = GetComponent("FirstPersonController") as MonoBehaviour;
            mouseLook = GetComponent("MouseLook") as MonoBehaviour;

            if (firstPersonController != null)
            {
                var type = firstPersonController.GetType();
                walkingField = type.GetField("m_Walking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            if (mouseLook != null)
            {
                var type = mouseLook.GetType();
                mouseSensitivityProperty = type.GetProperty("XSensitivity") ?? type.GetProperty("YSensitivity");
                if (mouseSensitivityProperty != null && mouseSensitivityProperty.CanRead)
                {
                    originalMouseSensitivity = (float)mouseSensitivityProperty.GetValue(mouseLook);
                }
            }
        }

        private void Update()
        {
            if (firstPersonController != null && walkingField != null)
            {
                walkingField.SetValue(firstPersonController, CanMove);
            }

            if (mouseLook != null && mouseSensitivityProperty != null && mouseSensitivityProperty.CanWrite)
            {
                float sensitivity = CanMove ? originalMouseSensitivity : 0f;
                mouseSensitivityProperty.SetValue(mouseLook, sensitivity);
            }
        }
    }


    public class CharacterControllerAdapter : MonoBehaviour, IMovementController
    {
        private CharacterController characterController;
        private MonoBehaviour[] movementScripts;
        private bool[] originalEnabledStates;

        [Header("Scripts to Control")]
        [SerializeField] private MonoBehaviour[] scriptsToDisable;

        public bool CanMove
        {
            get => characterController != null ? characterController.enabled : true;
            set => SetControllerEnabled(value);
        }

        public GameObject GameObject => gameObject;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            if (scriptsToDisable == null || scriptsToDisable.Length == 0)
            {
           
                var allScripts = GetComponents<MonoBehaviour>();
                var movementScriptsList = new System.Collections.Generic.List<MonoBehaviour>();

                foreach (var script in allScripts)
                {
                    var typeName = script.GetType().Name.ToLower();
                    if (typeName.Contains("movement") || typeName.Contains("controller") ||
                        typeName.Contains("player") || typeName.Contains("fps") || typeName.Contains("tps"))
                    {
                        movementScriptsList.Add(script);
                    }
                }

                scriptsToDisable = movementScriptsList.ToArray();
            }

     
            originalEnabledStates = new bool[scriptsToDisable.Length];
            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null)
                {
                    originalEnabledStates[i] = scriptsToDisable[i].enabled;
                }
            }
        }

        private void SetControllerEnabled(bool enabled)
        {
            if (characterController != null)
            {
                characterController.enabled = enabled;
            }

            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null)
                {
                    scriptsToDisable[i].enabled = enabled ? originalEnabledStates[i] : false;
                }
            }
        }
    }


    public class MessageBasedControllerAdapter : MonoBehaviour, IMovementController
    {
        [Header("Message Settings")]
        [SerializeField] private string enableMovementMessage = "EnableMovement";
        [SerializeField] private string disableMovementMessage = "DisableMovement";
        [SerializeField] private MonoBehaviour targetController;

        private bool canMove = true;

        public bool CanMove
        {
            get => canMove;
            set
            {
                if (canMove != value)
                {
                    canMove = value;
                    SendMovementMessage(value);
                }
            }
        }

        public GameObject GameObject => gameObject;

        private void Start()
        {
            if (targetController == null)
            {
                targetController = this;
            }
        }

        private void SendMovementMessage(bool enable)
        {
            if (targetController != null)
            {
                string message = enable ? enableMovementMessage : disableMovementMessage;
                targetController.SendMessage(message, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}