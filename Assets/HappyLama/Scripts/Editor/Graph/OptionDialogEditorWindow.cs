using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HappyLama
{
    public class OptionDialogEditorWindow : EditorWindow
    {
        private DialogueNode _node;
        private int _optionIndex;
        private TextField _textField;

        public void Initialize(DialogueNode node, int optionIndex, StoryGraphView storyGraphView)
        {
            _node = node;
            _optionIndex = optionIndex;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingTop = 10;

            titleContent = new GUIContent("Edit Dialog Text");

    
            if (_node.Choices == null) _node.Choices = new List<string>();

     
            while (_node.Choices.Count <= _optionIndex) _node.Choices.Add("");

            _textField = new TextField()
            {
                value = _node.Choices[_optionIndex],
                multiline = true,
                style = {
                flexGrow = 1,
                height = 150,
                whiteSpace = WhiteSpace.Normal
            }
            };
            root.Add(_textField);

      
            var buttonContainer = new VisualElement()
            {
                style = {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.FlexEnd,
                marginTop = 10
            }
            };

            var cancelButton = new Button(() => Close()) { text = "Cancel" };
            buttonContainer.Add(cancelButton);

            var saveButton = new Button(SaveAndClose) { text = "Save" };
            buttonContainer.Add(saveButton);

            root.Add(buttonContainer);
        }

        private void SaveAndClose()
        {
            _node.Choices[_optionIndex] = _textField.value;
            Close();
        }
    }
}