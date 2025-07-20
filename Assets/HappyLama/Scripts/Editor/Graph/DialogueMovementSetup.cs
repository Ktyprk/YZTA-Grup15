using UnityEngine;
using UnityEditor;
using System.Linq;

namespace HappyLama
{
    public class DialogueMovementSetup : EditorWindow
    {
        [MenuItem("HappyLama/Movement Integration/Auto Setup")]
        public static void ShowWindow()
        {
            GetWindow<DialogueMovementSetup>("Movement Integration Setup");
        }

        [MenuItem("HappyLama/Movement Integration/Quick Setup")]
        public static void QuickSetup()
        {
            AutoSetupMovementIntegration();
        }

        private Vector2 scrollPosition;
        private GameObject selectedPlayer;
        private MonoBehaviour[] detectedControllers;

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Movement Integration Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Quick setup button
            if (GUILayout.Button("🚀 AUTO SETUP - One Click Solution", GUILayout.Height(40)))
            {
                AutoSetupMovementIntegration();
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("This will automatically find your player controller and add movement integration without any code changes!", MessageType.Info);

            GUILayout.Space(20);
            GUILayout.Label("Manual Setup Options:", EditorStyles.boldLabel);

            // Player selection
            selectedPlayer = (GameObject)EditorGUILayout.ObjectField("Player GameObject:", selectedPlayer, typeof(GameObject), true);

            if (selectedPlayer != null)
            {
                // Detect controllers on selected player
                if (GUILayout.Button("Scan for Controllers"))
                {
                    ScanForControllers();
                }

                if (detectedControllers != null && detectedControllers.Length > 0)
                {
                    GUILayout.Label("Detected Controllers:", EditorStyles.boldLabel);

                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                    foreach (var controller in detectedControllers)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"📋 {controller.GetType().Name}", GUILayout.Width(200));
                        EditorGUILayout.LabelField($"on {controller.gameObject.name}");

                        if (GUILayout.Button("Add Adapter", GUILayout.Width(100)))
                        {
                            AddAdapterToController(controller.gameObject);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }

            GUILayout.Space(20);

            // Validation
            if (GUILayout.Button("🔍 Validate Current Setup"))
            {
                ValidateSetup();
            }

            // Reset
            if (GUILayout.Button("🗑️ Remove All Adapters"))
            {
                RemoveAllAdapters();
            }
        }

        private static void AutoSetupMovementIntegration()
        {
            Debug.Log("🚀 [Auto Setup] Starting automatic movement integration setup...");

            // 1. Find player GameObjects
            var playerObjects = FindPlayerObjects();

            if (playerObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("No Player Found",
                    "No GameObject with 'Player' tag found. Please tag your player GameObject with 'Player' tag.", "OK");
                return;
            }

            int adaptedCount = 0;

            foreach (var player in playerObjects)
            {
                // 2. Check if adapter already exists
                var existingAdapter = player.GetComponent<AutoControllerAdapter>();
                if (existingAdapter != null)
                {
                    Debug.Log($"✅ [Auto Setup] {player.name} already has AutoControllerAdapter");
                    continue;
                }

                // 3. Find controllers on this player
                var controllers = FindControllersOnObject(player);

                if (controllers.Length > 0)
                {
                    // 4. Add adapter
                    var adapter = player.AddComponent<AutoControllerAdapter>();
                    EditorUtility.SetDirty(player);
                    adaptedCount++;

                    Debug.Log($"✅ [Auto Setup] Added AutoControllerAdapter to {player.name}");
                    Debug.Log($"📋 [Auto Setup] Detected controllers: {string.Join(", ", controllers.Select(c => c.GetType().Name))}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ [Auto Setup] No controllers found on {player.name}");
                }
            }

            // 5. Ensure MovementManager exists
            EnsureMovementManagerExists();

            // 6. Show results
            if (adaptedCount > 0)
            {
                EditorUtility.DisplayDialog("Setup Complete!",
                    $"✅ Successfully set up movement integration for {adaptedCount} player controller(s)!\n\n" +
                    "The system will automatically detect and control movement during dialogues.\n\n" +
                    "No code changes needed in your controllers!", "Great!");
            }
            else
            {
                EditorUtility.DisplayDialog("Setup Warning",
                    "No new adapters were added. Either they already exist or no suitable controllers were found.\n\n" +
                    "Make sure your player GameObjects are tagged with 'Player'.", "OK");
            }
        }

        private static GameObject[] FindPlayerObjects()
        {
            // Find by tag first
            var taggedPlayers = GameObject.FindGameObjectsWithTag("Player");
            if (taggedPlayers.Length > 0)
                return taggedPlayers;

            // Find by name patterns
            var allObjects = FindObjectsOfType<GameObject>();
            return allObjects.Where(obj =>
                obj.name.ToLower().Contains("player") ||
                obj.name.ToLower().Contains("character") ||
                obj.name.ToLower().Contains("fps") ||
                obj.name.ToLower().Contains("tps")).ToArray();
        }

        private static MonoBehaviour[] FindControllersOnObject(GameObject obj)
        {
            var allComponents = obj.GetComponents<MonoBehaviour>();

            return allComponents.Where(component =>
            {
                if (component == null) return false;

                var typeName = component.GetType().Name.ToLower();
                return typeName.Contains("controller") ||
                       typeName.Contains("movement") ||
                       typeName.Contains("player") ||
                       typeName.Contains("fps") ||
                       typeName.Contains("character");
            }).ToArray();
        }

        private static void EnsureMovementManagerExists()
        {
            var existing = FindObjectOfType<MovementManager>();
            if (existing == null)
            {
                var go = new GameObject("MovementManager");
                var manager = go.AddComponent<MovementManager>();
                EditorUtility.SetDirty(go);
                Debug.Log("✅ [Auto Setup] Created MovementManager");
            }
            else
            {
                Debug.Log("✅ [Auto Setup] MovementManager already exists");
            }
        }

        private void ScanForControllers()
        {
            if (selectedPlayer == null) return;

            detectedControllers = FindControllersOnObject(selectedPlayer);
            Debug.Log($"Found {detectedControllers.Length} controllers on {selectedPlayer.name}");
        }

        private void AddAdapterToController(GameObject target)
        {
            var existing = target.GetComponent<AutoControllerAdapter>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Already Exists",
                    $"AutoControllerAdapter already exists on {target.name}", "OK");
                return;
            }

            var adapter = target.AddComponent<AutoControllerAdapter>();
            EditorUtility.SetDirty(target);

            EditorUtility.DisplayDialog("Adapter Added",
                $"AutoControllerAdapter added to {target.name}!\n\nThe adapter will automatically detect and control movement during dialogues.", "OK");
        }

        private void ValidateSetup()
        {
            var adapters = FindObjectsOfType<AutoControllerAdapter>();
            var movementManager = FindObjectOfType<MovementManager>();
            var dialogManager = FindObjectOfType<DialogManager>();
            var dialogUI = FindObjectOfType<DialogUIManager>();

            string report = "=== Movement Integration Validation ===\n\n";

            report += $"✅ AutoControllerAdapters found: {adapters.Length}\n";
            report += $"✅ MovementManager: {(movementManager != null ? "FOUND" : "MISSING")}\n";
            report += $"✅ DialogManager: {(dialogManager != null ? "FOUND" : "MISSING")}\n";
            report += $"✅ DialogUIManager: {(dialogUI != null ? "FOUND" : "MISSING")}\n\n";

            if (adapters.Length > 0)
            {
                report += "Adapter Details:\n";
                foreach (var adapter in adapters)
                {
                    report += $"- {adapter.gameObject.name}\n";
                }
            }

            Debug.Log(report);
            EditorUtility.DisplayDialog("Validation Complete",
                "Check console for detailed validation report.", "OK");
        }

        private void RemoveAllAdapters()
        {
            if (!EditorUtility.DisplayDialog("Remove All Adapters",
                "Are you sure you want to remove all AutoControllerAdapters from the scene?",
                "Remove", "Cancel"))
                return;

            var adapters = FindObjectsOfType<AutoControllerAdapter>();
            foreach (var adapter in adapters)
            {
                DestroyImmediate(adapter);
                EditorUtility.SetDirty(adapter.gameObject);
            }

            Debug.Log($"Removed {adapters.Length} AutoControllerAdapters");
        }
    }
}