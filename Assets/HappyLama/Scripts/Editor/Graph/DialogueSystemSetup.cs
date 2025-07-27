using UnityEngine;
using UnityEditor;

namespace HappyLama
{
    /// <summary>
    /// Automatic setup tools for the dialogue system movement integration
    /// </summary>
    public class DialogueSystemSetup
    {
        [MenuItem("HappyLama/Setup/Auto Setup Movement Integration")]
        public static void AutoSetupMovementIntegration()
        {
            // Check if MovementManager already exists
            MovementManager existingManager = Object.FindObjectOfType<MovementManager>();
            if (existingManager != null)
            {
                EditorUtility.DisplayDialog("Setup Complete",
                    "MovementManager already exists in the scene!", "OK");
                Selection.activeGameObject = existingManager.gameObject;
                return;
            }

            // Create MovementManager
            GameObject managerGO = new GameObject("MovementManager");
            MovementManager manager = managerGO.AddComponent<MovementManager>();

            // Position it at origin
            managerGO.transform.position = Vector3.zero;

            // Select it in hierarchy
            Selection.activeGameObject = managerGO;

            // Mark scene as dirty
            EditorUtility.SetDirty(managerGO);

            Debug.Log("[Dialogue System] MovementManager created successfully! Auto-detection is enabled by default.");
            EditorUtility.DisplayDialog("Setup Complete",
                "MovementManager has been added to your scene!\n\n" +
                "Auto-detection is enabled by default and will find most controllers automatically.\n\n" +
                "Check the console for detection results when you play the scene.", "OK");
        }

        [MenuItem("HappyLama/Setup/Add Generic Controller Adapter")]
        public static void AddGenericControllerAdapter()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Please select a GameObject with a player controller first.", "OK");
                return;
            }

            // Check if adapter already exists
            var existing = selected.GetComponent<HappyLama.GenericControllerAdapter>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Info",
                    "Generic Controller Adapter already exists on this GameObject.", "OK");
                Selection.activeObject = existing;
                return;
            }

            // Add adapter
            var adapter = selected.AddComponent<HappyLama.GenericControllerAdapter>();
            EditorUtility.SetDirty(selected);
           Selection.activeObject = adapter;

            EditorUtility.DisplayDialog("Adapter Added",
                "Generic Controller Adapter has been added!\n\n" +
                "Please configure the 'Target Controller' and 'Movement Field Name' in the inspector.", "OK");
        }

        [MenuItem("HappyLama/Setup/Add Character Controller Adapter")]
        public static void AddCharacterControllerAdapter()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Please select a GameObject first.", "OK");
                return;
            }

            var existing = selected.GetComponent<HappyLama.CharacterControllerAdapter>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Info",
                    "Character Controller Adapter already exists on this GameObject.", "OK");
                return;
            }

            var adapter = selected.AddComponent<HappyLama.CharacterControllerAdapter>();
            EditorUtility.SetDirty(selected);
            Selection.activeObject = adapter;

            EditorUtility.DisplayDialog("Adapter Added",
                "Character Controller Adapter has been added!\n\n" +
                "It will automatically find and control movement scripts on this GameObject.", "OK");
        }

        [MenuItem("HappyLama/Setup/Validate Setup")]
        public static void ValidateSetup()
        {
            bool hasMovementManager = Object.FindObjectOfType<MovementManager>() != null;
            bool hasDialogManager = Object.FindObjectOfType<DialogManager>() != null;
            bool hasDialogUI = Object.FindObjectOfType<DialogUIManager>() != null;

            string report = "=== Dialogue System Setup Validation ===\n\n";

            report += $"✓ MovementManager: {(hasMovementManager ? "FOUND" : "MISSING")}\n";
            report += $"✓ DialogManager: {(hasDialogManager ? "FOUND" : "MISSING")}\n";
            report += $"✓ DialogUIManager: {(hasDialogUI ? "FOUND" : "MISSING")}\n\n";

            // Check for controllers
            var controllers = Object.FindObjectsOfType<MonoBehaviour>();
            int interfaceControllers = 0;
            int detectedControllers = 0;

            foreach (var controller in controllers)
            {
                if (controller is IMovementController)
                {
                    interfaceControllers++;
                }

                var typeName = controller.GetType().Name.ToLower();
                if (typeName.Contains("controller") || typeName.Contains("movement") ||
                    typeName.Contains("fps") || typeName.Contains("player"))
                {
                    detectedControllers++;
                }
            }

            report += $"Controllers implementing IMovementController: {interfaceControllers}\n";
            report += $"Potential movement controllers detected: {detectedControllers}\n\n";

            if (!hasMovementManager)
            {
                report += "⚠️ MovementManager is missing! Use 'Auto Setup Movement Integration' to add it.\n";
            }

            if (interfaceControllers == 0 && detectedControllers > 0)
            {
                report += "💡 Consider implementing IMovementController in your player controller for best results.\n";
            }

            report += "\n=== End Report ===";

            Debug.Log(report);
            EditorUtility.DisplayDialog("Setup Validation",
                "Check the console for detailed validation report.", "OK");
        }

        [MenuItem("HappyLama/Help/Open Movement Integration Guide")]
        public static void OpenMovementGuide()
        {
            string guide = @"
=== Quick Start Guide ===

1. AUTOMATIC SETUP (Recommended):
   - Use menu: HappyLama > Setup > Auto Setup Movement Integration
   - This creates a MovementManager that auto-detects most controllers

2. MANUAL SETUP:
   - Add MovementManager to your scene
   - Enable 'Auto Find Controllers' in inspector
   
3. FOR CUSTOM CONTROLLERS:
   - Implement IMovementController interface in your controller
   - Add 'if (!CanMove) return;' check to your Update() method

4. TROUBLESHOOTING:
   - Enable 'Show Debug Logs' in MovementManager
   - Check console for controller detection messages
   - Use validation tool: HappyLama > Setup > Validate Setup

=== Interface Example ===

public class YourController : MonoBehaviour, IMovementController
{
    public bool CanMove { get; set; } = true;
    public GameObject GameObject => gameObject;
    
    void Update()
    {
        if (!CanMove) return; // Add this line!
        // Your movement code here...
    }
}

For more detailed information, check the documentation!
            ";

            Debug.Log(guide);
            EditorUtility.DisplayDialog("Movement Integration Guide",
                "Quick guide has been logged to console.\n\nFor complete documentation, check the included setup guide.", "OK");
        }

        [MenuItem("HappyLama/Tools/Find All Controllers")]
        public static void FindAllControllers()
        {
            var allControllers = Object.FindObjectsOfType<MonoBehaviour>();
            string report = "=== All Potential Controllers ===\n\n";

            foreach (var controller in allControllers)
            {
                var typeName = controller.GetType().Name;
                bool isMovementController = controller is IMovementController;
                bool hasMovementHints = typeName.ToLower().Contains("controller") ||
                                       typeName.ToLower().Contains("movement") ||
                                       typeName.ToLower().Contains("fps") ||
                                       typeName.ToLower().Contains("player");

                if (isMovementController || hasMovementHints)
                {
                    string status = isMovementController ? "[IMovementController]" : "[Potential]";
                    report += $"{status} {controller.name} - {typeName}\n";
                }
            }

            report += "\n=== Legend ===\n";
            report += "[IMovementController] = Implements the interface (ready to use)\n";
            report += "[Potential] = Might be a movement controller (may need setup)\n";

            Debug.Log(report);
            EditorUtility.DisplayDialog("Controller Detection",
                "All controllers have been logged to console.", "OK");
        }
    }

    /// <summary>
    /// Runtime validator that can be called from code
    /// </summary>
    public static class RuntimeValidator
    {
        public static bool ValidateMovementSetup()
        {
            var movementManager = Object.FindObjectOfType<MovementManager>();
            if (movementManager == null)
            {
                Debug.LogWarning("[Dialogue System] MovementManager not found! Movement will not be controlled during dialogues.");
                return false;
            }

            return true;
        }

        public static void AutoSetupIfMissing()
        {
            if (Object.FindObjectOfType<MovementManager>() == null)
            {
                var go = new GameObject("MovementManager (Auto-Created)");
                go.AddComponent<MovementManager>();
                Debug.Log("[Dialogue System] MovementManager was missing and has been automatically created.");
            }
        }
    }
}