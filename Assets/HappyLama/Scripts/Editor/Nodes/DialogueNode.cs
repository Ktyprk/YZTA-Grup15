using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HappyLama
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string DialogueText;
        public bool EntryPoint;
        public bool IsEndNode;
        public bool IsPlayerNode;
        public List<string> Choices = new List<string>();
        public List<int> ReputationChanges = new List<int>();

        public override void OnSelected()
        {
            base.OnSelected();
            if (IsPlayerNode)
            {
                UpdateReputationDisplay();
            }
        }



        public void UpdateReputationDisplay()
        {
            var existingLabels = this.Query<Label>(className: "reputation-label").ToList();
            foreach (var label in existingLabels)
            {
                label.RemoveFromHierarchy();
            }

            for (int i = 0; i < outputContainer.childCount; i++)
            {
                var portContainer = outputContainer[i];
                var reputationChange = i < ReputationChanges.Count ? ReputationChanges[i] : 0;

                var port = portContainer.Q<Port>();
                if (port != null)
                {
                    port.portName = $"Option {i + 1}";
                }

                var reputationLabel = new Label
                {
                    text = GetReputationText(reputationChange),
                    tooltip = "Reputation Change",
                    style = {
                    marginLeft = 5,
                    marginRight = 5,
                    unityTextAlign = TextAnchor.MiddleRight
                }
                };
                reputationLabel.AddToClassList("reputation-label");

                portContainer.Insert(portContainer.childCount - 1, reputationLabel);
            }
        }
        private string GetReputationText(int value)
        {
            return value switch
            {
                > 0 => $"<color=green>+{value}</color>",
                < 0 => $"<color=red>{value}</color>",
                _ => "0"
            };
        }

        public void SetAsEntry()
        {
            EntryPoint = true;
            IsEndNode = false;
            IsPlayerNode = false;
            title = "START";
            DialogueText = "";
            var gradient = new Texture2D(1, 2);
            gradient.SetPixel(0, 0, new Color(0.3f, 0.6f, 0.3f)); 
            gradient.SetPixel(0, 1, new Color(0.6f, 0.9f, 0.6f)); 
            gradient.Apply();

            style.backgroundImage = gradient;
            capabilities &= ~Capabilities.Deletable;
        }
        public void SetAsEnd()
        {
            IsEndNode = true;
            EntryPoint = false;
            IsPlayerNode = false;
            title = "END";

            style.backgroundColor = new Color(0.9f, 0.6f, 0.6f);
        }

        public void SetAsPlayer()
        {
            IsPlayerNode = true;
            EntryPoint = false;
            IsEndNode = false;
            title = "PLAYER";
            style.backgroundColor = new Color(0.5f, 0.7f, 0.9f);
            DialogueText = string.Empty;
        }

        public void SetAsBot()
        {
            IsPlayerNode = false;
            EntryPoint = false;
            IsEndNode = false;
            title = "NPC";
            style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}