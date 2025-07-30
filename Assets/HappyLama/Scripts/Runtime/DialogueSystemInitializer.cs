using UnityEngine;

namespace HappyLama
{
    
    public class DialogueSystemInitializer : MonoBehaviour
    {
        [Header("Auto-Setup Options")]
        [SerializeField] private bool autoCreateMovementManager = true;
        [SerializeField] private bool showSetupMessages = true;
        [SerializeField] private bool validateOnStart = true;

        [Header("Fallback Controller Settings")]
        [SerializeField] private bool enableFallbackControllerDetection = true;
        [SerializeField]
        private string[] fallbackControllerNames = {
            "FirstPersonController", "FPSController", "ThirdPersonController",
            "PlayerController", "CharacterController", "PlayerMovement"
        };

        private void Awake()
        {
            if (autoCreateMovementManager)
            {
                EnsureMovementManagerExists();
            }
        }

        private void Start()
        {
            if (validateOnStart)
            {
                ValidateSetup();
            }
        }

        private void EnsureMovementManagerExists()
        {
            MovementManager existingManager = FindObjectOfType<MovementManager>();
            if (existingManager == null)
            {
    
                GameObject managerGO = new GameObject("MovementManager (Auto-Created)");
                MovementManager manager = managerGO.AddComponent<MovementManager>();

                if (showSetupMessages)
                {
                    Debug.Log("[Dialogue System] MovementManager was automatically created for dialogue movement control.");
                }

           
                if (enableFallbackControllerDetection)
                {
                    DetectAndAssignControllers(manager);
                }
            }
        }

        private void DetectAndAssignControllers(MovementManager manager)
        {
            var allComponents = FindObjectsOfType<MonoBehaviour>();
            int foundControllers = 0;

            foreach (var component in allComponents)
            {
                // Skip if it's already an IMovementController
                if (component is IMovementController) continue;

                var typeName = component.GetType().Name;
                bool isLikelyController = false;

                // Check against fallback controller names
                foreach (var controllerName in fallbackControllerNames)
                {
                    if (typeName.Contains(controllerName))
                    {
                        isLikelyController = true;
                        break;
                    }
                }

                // Also check if it's on a Player tagged object
                if (!isLikelyController && component.gameObject.CompareTag("Player"))
                {
                    if (typeName.ToLower().Contains("movement") ||
                        typeName.ToLower().Contains("controller") ||
                        typeName.ToLower().Contains("player"))
                    {
                        isLikelyController = true;
                    }
                }

                if (isLikelyController)
                {
                    manager.AddController(component);
                    foundControllers++;

                    if (showSetupMessages)
                    {
                        Debug.Log($"[Dialogue System] Auto-detected controller: {component.name} ({typeName})");
                    }
                }
            }

            if (showSetupMessages && foundControllers == 0)
            {
                Debug.LogWarning("[Dialogue System] No controllers were auto-detected. You may need to manually configure the MovementManager or implement IMovementController in your player controller.");
            }
        }

        private void ValidateSetup()
        {
            bool hasMovementManager = FindObjectOfType<MovementManager>() != null;
            bool hasDialogManager = FindObjectOfType<DialogManager>() != null;
            bool hasDialogUI = FindObjectOfType<DialogUIManager>() != null;

            if (!hasMovementManager)
            {
                Debug.LogError("[Dialogue System] MovementManager is missing! Movement will not be controlled during dialogues.");
            }

            if (!hasDialogManager)
            {
                Debug.LogError("[Dialogue System] DialogManager is missing! Dialogues will not work.");
            }

            if (!hasDialogUI)
            {
                Debug.LogError("[Dialogue System] DialogUIManager is missing! Dialog UI will not be displayed.");
            }

            if (hasMovementManager && hasDialogManager && hasDialogUI)
            {
                if (showSetupMessages)
                {
                    Debug.Log("[Dialogue System] All core components found. System is ready!");
                }
            }
        }

        /// <summary>
        /// Call this method to manually trigger system validation
        /// </summary>
        public void ValidateSystemSetup()
        {
            ValidateSetup();
        }

        /// <summary>
        /// Call this method to manually ensure MovementManager exists
        /// </summary>
        public void EnsureMovementManager()
        {
            EnsureMovementManagerExists();
        }

        private void OnValidate()
        {
            // Ensure fallback controller names are not empty
            if (fallbackControllerNames == null || fallbackControllerNames.Length == 0)
            {
                fallbackControllerNames = new string[] {
                    "FirstPersonController", "FPSController", "ThirdPersonController",
                    "PlayerController", "CharacterController", "PlayerMovement"
                };
            }
        }
    }

    /// <summary>
    /// Attribute to automatically add DialogueSystemInitializer to GameObjects
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class RequireDialogueSystemAttribute : System.Attribute
    {
        public RequireDialogueSystemAttribute()
        {
            // This attribute can be used to mark classes that require the dialogue system
        }
    }

    /// <summary>
    /// Static utility for ensuring dialogue system is set up
    /// </summary>
    public static class DialogueSystemUtility
    {
        /// <summary>
        /// Ensures all dialogue system components are present in the scene
        /// Call this from your game manager or at scene start
        /// </summary>
        public static void EnsureDialogueSystemSetup()
        {
            // Find or create initializer
            DialogueSystemInitializer initializer = Object.FindObjectOfType<DialogueSystemInitializer>();
            if (initializer == null)
            {
                GameObject initializerGO = new GameObject("DialogueSystemInitializer");
                initializer = initializerGO.AddComponent<DialogueSystemInitializer>();
            }

            // Validate setup
            initializer.ValidateSystemSetup();
            initializer.EnsureMovementManager();
        }

        /// <summary>
        /// Check if dialogue system is properly set up
        /// </summary>
        public static bool IsDialogueSystemReady()
        {
            return Object.FindObjectOfType<MovementManager>() != null &&
                   Object.FindObjectOfType<DialogManager>() != null &&
                   Object.FindObjectOfType<DialogUIManager>() != null;
        }

        /// <summary>
        /// Get or create MovementManager instance
        /// </summary>
        public static MovementManager GetOrCreateMovementManager()
        {
            MovementManager manager = Object.FindObjectOfType<MovementManager>();
            if (manager == null)
            {
                GameObject managerGO = new GameObject("MovementManager");
                manager = managerGO.AddComponent<MovementManager>();
            }
            return manager;
        }
    }
}