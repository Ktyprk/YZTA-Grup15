using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace HappyLama
{
    public class StoryGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(250, 200);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;
        private int portIndex;
        public const int MAX_REPUTATION = 100;
        public const int MIN_REPUTATION = 0;
        public StoryGraphView(StoryGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
            this.AddManipulator(new ClickSelector());


            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());

            AddSearchWindow(editorWindow);
        }

        private void UpdateSelectedNodes()
        {
            foreach (var selectedNode in selection.OfType<DialogueNode>())
            {
                selectedNode.RefreshExpandedState();
                selectedNode.RefreshPorts();
                selectedNode.UpdateReputationDisplay();
                Debug.Log($"Updated node: {selectedNode.GUID}");
            }
        }


        public void AddUpdateButtonToToolbar(Toolbar toolbar)
        {
            var updateButton = new Button(() => UpdateSelectedNodes())
            {
                text = "Update",
                tooltip = "Update selected nodes"
            };
            toolbar.Add(updateButton);
        }

        private void AddSearchWindow(StoryGraph editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData?.Title ?? "Comment Block"
            };

            group.headerContainer.style.minHeight = 30;

            group.headerContainer.style.justifyContent = Justify.Center; 
            group.headerContainer.style.alignItems = Align.Center; 

            group.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            group.style.borderTopColor = Color.gray;
            group.style.borderBottomColor = Color.gray;
            group.style.borderLeftColor = Color.gray;
            group.style.borderRightColor = Color.gray;

            AddElement(group);
            group.SetPosition(rect);

            var titleLabel = group.Q<Label>("title-label");
            if (titleLabel != null)
            {

                titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                titleLabel.style.fontSize = 14;
                titleLabel.style.flexGrow = 1;
            }

            return group;
        }


        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = property.Type.ToString() };
            container.Add(field);

            VisualElement propertyValueField;

            switch (property.Type)
            {
                case ExposedProperty.PropertyType.Integer:
                    var intField = new IntegerField("Value:") { value = property.IntValue };
                    intField.RegisterValueChangedCallback(evt =>
                    {
                        var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                        ExposedProperties[index].IntValue = Mathf.Clamp(evt.newValue, MIN_REPUTATION, MAX_REPUTATION);
                    });
                    propertyValueField = intField;
                    break;

                case ExposedProperty.PropertyType.Float:
                    var floatField = new FloatField("Value:") { value = property.FloatValue };
                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                        ExposedProperties[index].FloatValue = evt.newValue;
                    });
                    propertyValueField = floatField;
                    break;

                case ExposedProperty.PropertyType.Boolean:
                    var boolField = new UnityEngine.UIElements.Toggle("Value:") { value = property.BoolValue };
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                        ExposedProperties[index].BoolValue = evt.newValue;
                    });
                    propertyValueField = boolField;
                    break;

                default:
                    var stringField = new TextField("Value:") { value = property.StringValue };
                    stringField.RegisterValueChangedCallback(evt =>
                    {
                        var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                        ExposedProperties[index].StringValue = evt.newValue;
                    });
                    propertyValueField = stringField;
                    break;
            }

            var sa = new BlackboardRow(field, propertyValueField);
            container.Add(sa);
            Blackboard.Add(container);
        }
        public void AddReputationProperty()
        {
            var reputationProperty = ExposedProperty.CreateInstance();
            reputationProperty.PropertyName = "Reputation";
            reputationProperty.Type = ExposedProperty.PropertyType.Integer;
            reputationProperty.IntValue = 50; 

            AddPropertyToBlackBoard(reputationProperty);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                {
                    if (startPortView.direction != portView.direction)
                    {

                        compatiblePorts.Add(port);
                    }
                }
            });

            return compatiblePorts;
        }
        public void CreateNewNode(string nodeType, Vector2 position)
        {
            AddElement(CreateNode(nodeType, position));
        }
        public DialogueNode CreateNode(string nodeType, Vector2 position)
        {
            var node = new DialogueNode();

            switch (nodeType.ToLower())
            {
                case "entry":
                    node.SetAsEntry();
                    break;
                case "end":
                    node.SetAsEnd();
                    break;
                case "player":
                    node.SetAsPlayer();
                    break;
                default:
                    node.SetAsBot();
                    break;
            }


            node.GUID = Guid.NewGuid().ToString();
            node.SetPosition(new Rect(position, DefaultNodeSize));


            if (!node.EntryPoint)
            {
                var inputPort = GetPortInstance(node, Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);
            }


            if (node.IsPlayerNode)
            {

                var addOptionButton = new Button(() => AddOptionWithAutoConnection(node))
                {
                    text = "Add Option",
                    style = {
                height = 20,
                marginBottom = 5,
                marginTop = 5
            }
                };
                node.titleContainer.Add(addOptionButton);

           
                if (node.Choices == null) node.Choices = new List<string>();
                if (node.ReputationChanges == null) node.ReputationChanges = new List<int>();
            }

            else if (!node.EntryPoint && !node.IsEndNode)
            {

                var dialogLabel = new Label("DIALOG")
                {
                    style = {
                unityTextAlign = TextAnchor.UpperCenter,
                fontSize = 14,
                marginTop = 5,
                marginBottom = 5,
                color = new Color(0.8f, 0.8f, 0.8f)
            }
                };
                node.mainContainer.Add(dialogLabel);

                var dialogueField = new TextField()
                {
                    value = node.DialogueText,
                    multiline = true,
                    style = {
                flexGrow = 1,
                minWidth = DefaultNodeSize.x - 30,
                height = DefaultNodeSize.y - 70,
                marginLeft = 5,
                marginRight = 5,
                marginBottom = 5,
                unityTextAlign = TextAnchor.UpperLeft,
                whiteSpace = WhiteSpace.Normal
            }
                };
                dialogueField.RegisterValueChangedCallback(evt => node.DialogueText = evt.newValue);
                node.mainContainer.Add(dialogueField);
            }


            if (!node.IsEndNode && !node.IsPlayerNode && !node.EntryPoint)
            {
                var outputPort = GetPortInstance(node, Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);
            }

          


            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

      

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var node = new DialogueNode();
            node.SetAsEntry();

            var generatedPort = GetPortInstance(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            var portLabel = generatedPort.Q<Label>("type");
            if (portLabel != null)
            {
                portLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            }

            node.SetPosition(new Rect(100, 200, 100, 150));
            node.RefreshExpandedState();
            return node;
        }

        private void ShowOptionDialogWithReputation(DialogueNode node, Port port)
        {
            var portParent = port.parent;
            if (portParent == null) return;

            var index = node.outputContainer.IndexOf(portParent);
            if (index < 0) return;

            if (node.Choices == null)
            {
                node.Choices = new List<string>();
                node.ReputationChanges = new List<int>();
            }

            var window = ScriptableObject.CreateInstance<OptionDialogEditorWindow>();
            window.titleContent = new GUIContent("Edit Option Dialog");
            window.Initialize(node, index,this);
            window.ShowAuxWindow();
        }

        private void RemoveOptionPort(DialogueNode node, VisualElement portContainer)
        {
            if (node == null || portContainer == null) return;

       
            var port = portContainer.Q<Port>();
            if (port != null)
            {
      
                var targetEdges = edges.ToList()
                    .Where(x => x.output == port)
                    .ToList();

                foreach (var edge in targetEdges)
                {
                    edge.input?.Disconnect(edge);
                    RemoveElement(edge);
                }
            }

 
            node.outputContainer.Remove(portContainer);


            var index = node.outputContainer.IndexOf(portContainer);
            if (index >= 0 && index < node.Choices.Count)
            {
                node.Choices.RemoveAt(index);
            }

            node.RefreshPorts();
            node.RefreshExpandedState();
        }


        public void AddOptionPort(DialogueNode nodeCache, string overriddenPortName = "", int reputationChange = 0)
        {
            if (nodeCache.Choices == null)
            {
                nodeCache.Choices = new List<string>();
                nodeCache.ReputationChanges = new List<int>();
            }

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count;
            var currentPortIndex = outputPortCount;

            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;

            var optionPort = GetPortInstance(nodeCache, Direction.Output);
            optionPort.portName = outputPortName;

            var portContainer = new VisualElement();
            portContainer.style.flexDirection = FlexDirection.Row;
            portContainer.style.justifyContent = Justify.SpaceBetween;
            portContainer.style.alignItems = Align.Center;
            portContainer.style.width = Length.Percent(100);
            portContainer.style.marginTop = 2;
            portContainer.style.marginBottom = 2;
            portContainer.style.paddingLeft = 5;
            portContainer.style.paddingRight = 5;
            portContainer.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

            // Option label
            var optionLabel = new Label($"Option {outputPortCount + 1}:")
            {
                style = {
            width = 70,
            unityTextAlign = TextAnchor.MiddleLeft,
            marginRight = 5
        }
            };
            portContainer.Add(optionLabel);

            // Reputation field
            var reputationField = new IntegerField()
            {
                value = reputationChange,
                style = {
            width = 50,
            marginLeft = 5,
            marginRight = 5
        }
            };
            reputationField.RegisterValueChangedCallback(evt => {
                var newValue = Mathf.Clamp(evt.newValue, -100, 100);
                reputationField.value = newValue;

                if (nodeCache.ReputationChanges == null)
                    nodeCache.ReputationChanges = new List<int>();

                while (nodeCache.ReputationChanges.Count <= currentPortIndex)
                    nodeCache.ReputationChanges.Add(0);

                nodeCache.ReputationChanges[currentPortIndex] = newValue;
                nodeCache.UpdateReputationDisplay(); 
            });
            portContainer.Add(reputationField);

            // Reputation label
            var reputationLabel = new Label()
            {
                style = {
            width = 60,
            unityTextAlign = TextAnchor.MiddleRight,
            marginLeft = 5,
            marginRight = 5
        }
            };
            portContainer.Add(reputationLabel);

            // Button container
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.alignItems = Align.Center;

            // Edit button
            var editButton = new Button(() => ShowOptionDialogWithReputation(nodeCache, optionPort))
            {
                text = "Edit",
                style = {
            width = 50,
            height = 20,
            fontSize = 10,
            marginRight = 2
        }
            };
            buttonContainer.Add(editButton);

            // Delete button
            var deleteButton = new Button(() => RemoveOptionPort(nodeCache, portContainer))
            {
                text = "X",
                style = {
            width = 20,
            height = 20,
            fontSize = 10,
            backgroundColor = new Color(0.8f, 0.2f, 0.2f)
        }
            };
            buttonContainer.Add(deleteButton);

            portContainer.Add(buttonContainer);
            portContainer.Add(optionPort);

            nodeCache.outputContainer.Add(portContainer);
            nodeCache.outputContainer.style.justifyContent = Justify.FlexStart;

            if (outputPortCount >= nodeCache.Choices.Count)
            {
                nodeCache.Choices.Add(outputPortName);
                nodeCache.ReputationChanges.Add(reputationChange);
            }
            else
            {
                nodeCache.Choices[outputPortCount] = outputPortName;
                nodeCache.ReputationChanges[outputPortCount] = reputationChange;
            }

            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
            nodeCache.UpdateReputationDisplay(); 
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
        public void AddOptionWithAutoConnection(DialogueNode sourceNode, string optionText = "")
        {
          
            AddOptionPort(sourceNode, optionText);

        
        }

        public void ClearGraph()
        {

            foreach (var node in nodes.ToList())
            {
                RemoveElement(node);
            }

            foreach (var edge in edges.ToList())
            {
                RemoveElement(edge);
            }


            foreach (var group in graphElements.OfType<Group>().ToList())
            {
                RemoveElement(group);
            }

            ClearBlackBoardAndExposedProperties();

            EntryPointNode = GetEntryPointNodeInstance();
            AddElement(EntryPointNode);
        }

    }



}