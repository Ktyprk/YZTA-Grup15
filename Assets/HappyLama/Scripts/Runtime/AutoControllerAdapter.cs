using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace HappyLama
{
    /// <summary>
    /// Automatic adapter that works with any player controller without code changes
    /// </summary>
    public class AutoControllerAdapter : MonoBehaviour, IMovementController
    {
        [Header("Auto-Detected Controller")]
        [SerializeField] private MonoBehaviour detectedController;
        [SerializeField] private string detectedField;
        [SerializeField] private bool controllerFound = false;

        [Header("Manual Override (if auto-detection fails)")]
        [SerializeField] private MonoBehaviour manualController;
        [SerializeField] private string manualFieldName = "canMove";

        [Header("Fallback Options")]
        [SerializeField] private bool disableEntireControllerIfNeeded = true;
        [SerializeField] private bool controlCameraRotation = true;

        private FieldInfo movementField;
        private PropertyInfo movementProperty;
        private MonoBehaviour activeController;
        private bool originalValue = true;
        private bool wasControllerEnabled = true;
        private bool canMove = true;
        private Camera playerCamera;
        private float originalMouseSensitivity = 2f;

        public bool CanMove
        {
            get => canMove;
            set
            {
                if (canMove != value)
                {
                    canMove = value;
                    ApplyMovementState(value);
                    Debug.Log($"[AutoAdapter] CanMove set to: {value}");
                }
            }
        }

        public GameObject GameObject => gameObject;

        private void Start()
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;

            SetupController();
        }

        private void SetupController()
        {
            // Try manual controller first
            if (manualController != null)
            {
                if (TrySetupController(manualController, manualFieldName))
                {
                    Debug.Log($"[AutoAdapter] Manual controller setup successful: {manualController.name}");
                    return;
                }
            }

            // Try detected controller
            if (detectedController != null)
            {
                if (TrySetupController(detectedController, detectedField))
                {
                    Debug.Log($"[AutoAdapter] Auto-detected controller setup successful: {detectedController.name}");
                    return;
                }
            }

            // Try to find any controller on this GameObject
            AutoDetectController();
        }

        private void AutoDetectController()
        {
            var controllers = GetComponents<MonoBehaviour>();

            foreach (var controller in controllers)
            {
                if (controller == this) continue; // Skip self

                var type = controller.GetType();
                var typeName = type.Name.ToLower();

                // Check if it looks like a controller
                if (typeName.Contains("controller") || typeName.Contains("movement") ||
                    typeName.Contains("player") || typeName.Contains("fps") || typeName.Contains("character"))
                {
                    // Try common field names
                    string[] commonFields = { "canMove", "enabled", "movementEnabled", "allowMovement",
                                            "playerCanMove", "controlsEnabled", "lockMovement" };

                    foreach (var fieldName in commonFields)
                    {
                        if (TrySetupController(controller, fieldName))
                        {
                            detectedController = controller;
                            detectedField = fieldName;
                            controllerFound = true;
                            Debug.Log($"[AutoAdapter] Auto-detected: {controller.name}.{fieldName}");
                            return;
                        }
                    }
                }
            }

            Debug.LogWarning($"[AutoAdapter] No suitable controller found on {gameObject.name}. Using fallback mode.");
        }

        private bool TrySetupController(MonoBehaviour controller, string fieldName)
        {
            if (controller == null || string.IsNullOrEmpty(fieldName)) return false;

            var type = controller.GetType();

            // Try field first
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool))
            {
                movementField = field;
                activeController = controller;
                originalValue = (bool)field.GetValue(controller);
                wasControllerEnabled = controller.enabled;
                return true;
            }

            // Try property
            var property = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(bool) && property.CanWrite)
            {
                movementProperty = property;
                activeController = controller;
                originalValue = (bool)property.GetValue(controller);
                wasControllerEnabled = controller.enabled;
                return true;
            }

            return false;
        }

        private void ApplyMovementState(bool enabled)
        {
            if (activeController != null)
            {
                // Apply to field or property
                if (movementField != null)
                {
                    movementField.SetValue(activeController, enabled ? originalValue : false);
                }
                else if (movementProperty != null)
                {
                    movementProperty.SetValue(activeController, enabled ? originalValue : false);
                }

                // If needed, disable entire controller
                if (disableEntireControllerIfNeeded && !enabled)
                {
                    activeController.enabled = false;
                }
                else if (disableEntireControllerIfNeeded && enabled)
                {
                    activeController.enabled = wasControllerEnabled;
                }
            }

            // Control camera rotation
            if (controlCameraRotation && playerCamera != null)
            {
                // This is a simple approach - might need adjustment for specific controllers
                if (!enabled)
                {
                    // Store and reduce mouse sensitivity if possible
                    TryReduceMouseSensitivity();
                }
                else
                {
                    // Restore mouse sensitivity
                    TryRestoreMouseSensitivity();
                }
            }

            // Handle cursor
            if (!enabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void TryReduceMouseSensitivity()
        {
            if (activeController == null) return;

            var type = activeController.GetType();

            // Common sensitivity field names
            string[] sensitivityFields = { "lookSpeed", "mouseSensitivity", "sensitivity", "mouseSpeed", "rotateSpeed" };

            foreach (var fieldName in sensitivityFields)
            {
                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && field.FieldType == typeof(float))
                {
                    originalMouseSensitivity = (float)field.GetValue(activeController);
                    field.SetValue(activeController, 0f);
                    break;
                }
            }
        }

        private void TryRestoreMouseSensitivity()
        {
            if (activeController == null) return;

            var type = activeController.GetType();

            string[] sensitivityFields = { "lookSpeed", "mouseSensitivity", "sensitivity", "mouseSpeed", "rotateSpeed" };

            foreach (var fieldName in sensitivityFields)
            {
                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && field.FieldType == typeof(float))
                {
                    field.SetValue(activeController, originalMouseSensitivity);
                    break;
                }
            }
        }

        [ContextMenu("Test Disable Movement")]
        public void TestDisableMovement()
        {
            CanMove = false;
        }

        [ContextMenu("Test Enable Movement")]
        public void TestEnableMovement()
        {
            CanMove = true;
        }

        [ContextMenu("Refresh Controller Detection")]
        public void RefreshDetection()
        {
            SetupController();
        }

        // For inspector display
        private void OnValidate()
        {
            if (Application.isPlaying) return;

            // Auto-detect in editor
            if (detectedController == null)
            {
                var controllers = GetComponents<MonoBehaviour>();
                foreach (var controller in controllers)
                {
                    if (controller == this) continue;

                    var typeName = controller.GetType().Name.ToLower();
                    if (typeName.Contains("controller") || typeName.Contains("movement") ||
                        typeName.Contains("player") || typeName.Contains("fps"))
                    {
                        detectedController = controller;
                        break;
                    }
                }
            }
        }
    }
}