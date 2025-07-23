using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HappyLama
{
    public class StoryGraph : EditorWindow
    {
        private string _fileName = "New Dialog";
        private StoryGraphView _graphView;
        private DropdownField _containerDropdown;

        [MenuItem("HappyLama/New Dialog Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<StoryGraph>();
            window.titleContent = new GUIContent("Dialog Graph");
        }

        [MenuItem("HappyLama/Reset Prefs")]
        public static void ResetReputationPrefs()
        {
            if (EditorUtility.DisplayDialog("Reset Reputation",
                "Are you sure you want to reset ALL reputation data?\nThis cannot be undone!",
                "Reset", "Cancel"))
            {
                PlayerPrefs.DeleteAll();

                PlayerPrefs.Save();
                Debug.Log("All reputation data has been reset!");
            }
        }
        private void ConstructGraphView()
        {
            _graphView = new StoryGraphView(this)
            {
                name = "Dialog Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            // New container button
            toolbar.Add(new Button(() => CreateNewDialogueContainer())
            {
                text = "Create new dialogue",
                tooltip = "Create new dialogue container",
                style = { width = 140 }
            });

            // Container dropdown
            _containerDropdown = new DropdownField("");
            UpdateContainerDropdownChoices();
            _containerDropdown.RegisterValueChangedCallback(evt =>
            {
                _fileName = evt.newValue;
                RequestDataOperation(false);
            });
            toolbar.Add(_containerDropdown);

            // Save button
            toolbar.Add(new Button(() => RequestDataOperation(true))
            {
                text = "Save",
                tooltip = "Save current dialogue",
                style = { width = 60 }
            });

            // Delete button
            toolbar.Add(new Button(() => DeleteSelectedContainer())
            {
                text = "Delete",
                tooltip = "Delete selected dialogue container",
                style = { width = 45 }
            });

            rootVisualElement.Add(toolbar);
        }

        private void DeleteSelectedContainer()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Error", "No dialogue container selected to delete", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm Delete",
                $"Are you sure you want to delete '{_fileName}'?",
                "Delete", "Cancel"))
            {
                return;
            }

            string path = $"Assets/Resources/{_fileName}.asset";
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();

                if (_containerDropdown.value == _fileName)
                {
                    _fileName = string.Empty;
                    _graphView.ClearGraph();
                }

                UpdateContainerDropdownChoices();
            }
        }

        private void CreateNewDialogueContainer()
        {
            string newName = EditorInputDialog.Show("New Dialogue Container", "Enter name:", "NewDialogue");

            // Check if user canceled or entered empty name
            if (string.IsNullOrEmpty(newName))
                return;

            // Ensure Resources folder exists
            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.Refresh();
            }

            string path = $"Assets/Resources/{newName}.asset";
            if (File.Exists(path))
            {
                EditorUtility.DisplayDialog("Error", "A file with this name already exists!", "OK");
                return;
            }

            // Create and save new container
            var newContainer = ScriptableObject.CreateInstance<DialogueContainer>();
            AssetDatabase.CreateAsset(newContainer, path);
            AssetDatabase.SaveAssets();

            // Update UI
            _fileName = newName;
            UpdateContainerDropdownChoices();
            _containerDropdown.value = newName;
            _graphView.ClearGraph();
        }

        private void UpdateContainerDropdownChoices()
        {
            var currentContainers = GetAllDialogueContainers();
            _containerDropdown.choices = currentContainers;

            if (!currentContainers.Contains(_fileName))
            {
                _fileName = currentContainers.Count > 0 ? currentContainers[0] : string.Empty;
                _containerDropdown.value = _fileName;

                if (!string.IsNullOrEmpty(_fileName))
                    RequestDataOperation(false);
                else
                    _graphView.ClearGraph();
            }
        }

        private List<string> GetAllDialogueContainers()
        {
            var containers = new List<string>();

            if (Directory.Exists("Assets/Resources"))
            {
                var guids = AssetDatabase.FindAssets("t:DialogueContainer", new[] { "Assets/Resources" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    containers.Add(Path.GetFileNameWithoutExtension(path));
                }
            }

            containers.Sort();
            return containers;
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save)
                    saveUtility.SaveGraph(_fileName);
                else
                    saveUtility.LoadNarrative(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select or create a dialogue container first", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            EditorApplication.projectChanged += UpdateContainerDropdownChoices;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= UpdateContainerDropdownChoices;
            rootVisualElement.Remove(_graphView);
        }
    }

  
}