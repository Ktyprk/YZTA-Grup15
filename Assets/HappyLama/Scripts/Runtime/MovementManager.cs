using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HappyLama
{
    /// <summary>
    /// Manages player movement during dialogues. Automatically finds and controls movement controllers.
    /// Works with any controller that implements IMovementController or has common movement fields.
    /// </summary>
    public class MovementManager : MonoBehaviour
    {
        [Header("Auto-Detection Settings")]
        [SerializeField] private bool autoFindControllers = true;
        [SerializeField] private bool enableCursorDuringDialogue = true;
        [SerializeField] private bool disableControllerComponents = false;

        [Header("Manual Controller Assignment")]
        [SerializeField] private List<MonoBehaviour> manualControllers = new List<MonoBehaviour>();

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private List<IMovementController> interfaceControllers = new List<IMovementController>();
        private List<ControllerInfo> reflectionControllers = new List<ControllerInfo>();
        private bool isDialogueActive = false;

        // Public property for checking dialogue state
        public bool IsDialogueActive => isDialogueActive;
        private CursorLockMode originalCursorLockMode;
        private bool originalCursorVisible;

        public static MovementManager Instance;

        private struct ControllerInfo
        {
            public MonoBehaviour controller;
            public FieldInfo canMoveField;
            public PropertyInfo canMoveProperty;
            public bool originalValue;
            public bool wasEnabled;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Store original cursor state
            originalCursorLockMode = Cursor.lockState;
            originalCursorVisible = Cursor.visible;
        }

        private void Start()
        {
            FindControllers();
            SubscribeToDialogueEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromDialogueEvents();
        }

        private void SubscribeToDialogueEvents()
        {
            DialogManager.OnBotMessage += OnDialogueMessage;
            DialogManager.OnPlayerChoices += OnDialogueChoices;
            DialogManager.OnDialogueEnd += OnDialogueEnd;
        }

        private void UnsubscribeFromDialogueEvents()
        {
            DialogManager.OnBotMessage -= OnDialogueMessage;
            DialogManager.OnPlayerChoices -= OnDialogueChoices;
            DialogManager.OnDialogueEnd -= OnDialogueEnd;
        }

        private void OnDialogueMessage(string message, bool closeAfter)
        {
            // Her NPC mesajýnda hareketi durdur (ilk mesaj dahil)
            SetMovementEnabled(false);
        }

        private void OnDialogueChoices(List<string> choices, List<int> reputationChanges)
        {
            // Player seçimlerinde de hareketi durdur
            SetMovementEnabled(false);
        }

        private void OnDialogueEnd()
        {
            SetMovementEnabled(true);
        }

        private void FindControllers()
        {
            interfaceControllers.Clear();
            reflectionControllers.Clear();

            if (autoFindControllers)
            {
                // Find all IMovementController implementations
                var interfaceControllerComponents = FindObjectsOfType<MonoBehaviour>();
                foreach (var component in interfaceControllerComponents)
                {
                    if (component is IMovementController movementController)
                    {
                        interfaceControllers.Add(movementController);
                        LogDebug($"Found IMovementController: {component.name}");
                    }
                }

                // Find controllers through reflection (for common controller patterns)
                FindControllersViaReflection();
            }

            // Add manually assigned controllers
            foreach (var controller in manualControllers)
            {
                if (controller != null)
                {
                    if (controller is IMovementController iController && !interfaceControllers.Contains(iController))
                    {
                        interfaceControllers.Add(iController);
                        LogDebug($"Added manual IMovementController: {controller.name}");
                    }
                    else
                    {
                        AddControllerViaReflection(controller);
                    }
                }
            }

            LogDebug($"Total controllers found: Interface={interfaceControllers.Count}, Reflection={reflectionControllers.Count}");
        }

        private void FindControllersViaReflection()
        {
            // Common controller script names and field patterns
            string[] commonControllerNames = {
                "FirstPersonController", "FPSController", "ThirdPersonController", "TPSController",
                "PlayerController", "CharacterController", "PlayerMovement", "PlayerMotor",
                "MouseLook", "FPSWalker", "RigidbodyFirstPersonController"
            };

            string[] commonFieldNames = {
                "canMove", "enabled", "movementEnabled", "allowMovement", "lockMovement",
                "disableMovement", "playerCanMove", "controlsEnabled"
            };

            var allComponents = FindObjectsOfType<MonoBehaviour>();

            foreach (var component in allComponents)
            {
                // Skip if already added as interface controller
                if (component is IMovementController) continue;

                var componentType = component.GetType();
                var typeName = componentType.Name;

                // Check if it matches common controller names
                bool isLikelyController = false;
                foreach (var controllerName in commonControllerNames)
                {
                    if (typeName.Contains(controllerName))
                    {
                        isLikelyController = true;
                        break;
                    }
                }

                // Or if it has player tag and movement-related fields
                if (!isLikelyController && component.gameObject.CompareTag("Player"))
                {
                    isLikelyController = HasMovementFields(componentType, commonFieldNames);
                }

                if (isLikelyController)
                {
                    AddControllerViaReflection(component);
                }
            }
        }

        private bool HasMovementFields(System.Type type, string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var property = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if ((field != null && field.FieldType == typeof(bool)) ||
                    (property != null && property.PropertyType == typeof(bool)))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddControllerViaReflection(MonoBehaviour controller)
        {
            var componentType = controller.GetType();

            // Try to find a boolean field/property that controls movement
            string[] possibleFieldNames = {
                "canMove", "enabled", "movementEnabled", "allowMovement", "lockMovement",
                "disableMovement", "playerCanMove", "controlsEnabled"
            };

            FieldInfo canMoveField = null;
            PropertyInfo canMoveProperty = null;

            foreach (var fieldName in possibleFieldNames)
            {
                canMoveField = componentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (canMoveField != null && canMoveField.FieldType == typeof(bool))
                {
                    break;
                }

                canMoveProperty = componentType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (canMoveProperty != null && canMoveProperty.PropertyType == typeof(bool) && canMoveProperty.CanWrite)
                {
                    break;
                }

                canMoveField = null;
                canMoveProperty = null;
            }

            if (canMoveField != null || canMoveProperty != null)
            {
                var controllerInfo = new ControllerInfo
                {
                    controller = controller,
                    canMoveField = canMoveField,
                    canMoveProperty = canMoveProperty,
                    wasEnabled = controller.enabled
                };

                // Get original value
                if (canMoveField != null)
                {
                    controllerInfo.originalValue = (bool)canMoveField.GetValue(controller);
                }
                else if (canMoveProperty != null)
                {
                    controllerInfo.originalValue = (bool)canMoveProperty.GetValue(controller);
                }

                reflectionControllers.Add(controllerInfo);
                LogDebug($"Added reflection controller: {controller.name} (Field: {canMoveField?.Name}, Property: {canMoveProperty?.Name})");
            }
        }

        public void SetMovementEnabled(bool enabled)
        {
            if (isDialogueActive == !enabled)
            {
                LogDebug($"Movement state unchanged: already {(enabled ? "enabled" : "disabled")}");
                return; // No change needed
            }

            isDialogueActive = !enabled;
            LogDebug($"Setting movement to: {(enabled ? "ENABLED" : "DISABLED")}");

            // Handle interface controllers
            foreach (var controller in interfaceControllers)
            {
                if (controller?.GameObject != null)
                {
                    controller.CanMove = enabled;
                    LogDebug($"Set {controller.GameObject.name} CanMove to {enabled}");
                }
            }

            // Handle reflection controllers
            for (int i = 0; i < reflectionControllers.Count; i++)
            {
                var controllerInfo = reflectionControllers[i];
                if (controllerInfo.controller == null) continue;

                if (enabled)
                {
                    // Restore original state
                    if (controllerInfo.canMoveField != null)
                    {
                        controllerInfo.canMoveField.SetValue(controllerInfo.controller, controllerInfo.originalValue);
                        LogDebug($"Restored {controllerInfo.controller.name}.{controllerInfo.canMoveField.Name} to {controllerInfo.originalValue}");
                    }
                    else if (controllerInfo.canMoveProperty != null)
                    {
                        controllerInfo.canMoveProperty.SetValue(controllerInfo.controller, controllerInfo.originalValue);
                        LogDebug($"Restored {controllerInfo.controller.name}.{controllerInfo.canMoveProperty.Name} to {controllerInfo.originalValue}");
                    }

                    if (disableControllerComponents)
                    {
                        controllerInfo.controller.enabled = controllerInfo.wasEnabled;
                    }
                }
                else
                {
                    // Disable movement
                    if (controllerInfo.canMoveField != null)
                    {
                        controllerInfo.canMoveField.SetValue(controllerInfo.controller, false);
                        LogDebug($"Set {controllerInfo.controller.name}.{controllerInfo.canMoveField.Name} to false");
                    }
                    else if (controllerInfo.canMoveProperty != null)
                    {
                        controllerInfo.canMoveProperty.SetValue(controllerInfo.controller, false);
                        LogDebug($"Set {controllerInfo.controller.name}.{controllerInfo.canMoveProperty.Name} to false");
                    }

                    if (disableControllerComponents)
                    {
                        controllerInfo.controller.enabled = false;
                    }
                }
            }

            // Handle cursor
            if (enableCursorDuringDialogue)
            {
                if (!enabled) // Dialogue started
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    LogDebug("Cursor unlocked and made visible");
                }
                else // Dialogue ended
                {
                    Cursor.lockState = originalCursorLockMode;
                    Cursor.visible = originalCursorVisible;
                    LogDebug("Cursor restored to original state");
                }
            }

            LogDebug($"Movement control completed. Controllers: Interface={interfaceControllers.Count}, Reflection={reflectionControllers.Count}");
        }

        /// <summary>
        /// Manually add a controller to be managed by the dialogue system
        /// </summary>
        public void AddController(MonoBehaviour controller)
        {
            if (controller == null) return;

            if (controller is IMovementController iController)
            {
                if (!interfaceControllers.Contains(iController))
                {
                    interfaceControllers.Add(iController);
                }
            }
            else
            {
                AddControllerViaReflection(controller);
            }
        }

        /// <summary>
        /// Remove a controller from management
        /// </summary>
        public void RemoveController(MonoBehaviour controller)
        {
            if (controller == null) return;

            if (controller is IMovementController iController)
            {
                interfaceControllers.Remove(iController);
            }
            else
            {
                for (int i = reflectionControllers.Count - 1; i >= 0; i--)
                {
                    if (reflectionControllers[i].controller == controller)
                    {
                        reflectionControllers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Force refresh all controllers
        /// </summary>
        [ContextMenu("Refresh Controllers")]
        public void RefreshControllers()
        {
            FindControllers();
        }

        private void LogDebug(string message)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[MovementManager] {message}");
            }
        }

        // Editor helper
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                RefreshControllers();
            }
        }
    }
}