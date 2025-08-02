using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HappyLama
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<DialogueNode> Nodes => _graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private StoryGraphView _graphView;

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(fileName, dialogueContainerObject)) return;
            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
            {
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any())
            {
                Debug.LogWarning("No edges found - nothing to save!");
                return false;
            }

            dialogueContainerObject.NodeLinks.Clear();
            dialogueContainerObject.DialogueNodeData.Clear();

            // Save all node connections
            foreach (var edge in Edges.Where(x => x.input.node != null))
            {
                var outputNode = edge.output.node as DialogueNode;
                var inputNode = edge.input.node as DialogueNode;

                if (outputNode == null || inputNode == null) continue;

                dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = edge.output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

      
            foreach (var node in Nodes)
            {
                var nodeData = new DialogueNodeData
                {
                    NodeGUID = node.GUID,
                    DialogueText = node.DialogueText, 
                    Position = node.GetPosition().position,
                    Choices = new List<string>(),
                    ReputationChanges = new List<int>(),
                    EntryPoint = node.EntryPoint,
                    IsEndNode = node.IsEndNode,
                    IsPlayerNode = node.IsPlayerNode
                };

                if (node.IsPlayerNode)
                {
                    if (node.Choices != null)
                    {
                        nodeData.Choices.AddRange(node.Choices);
                    }

                    if (node.ReputationChanges != null)
                    {
                        nodeData.ReputationChanges.AddRange(node.ReputationChanges);
                    }
                    else
                    {
                        nodeData.ReputationChanges.AddRange(Enumerable.Repeat(0, node.Choices?.Count ?? 0));
                    }
                }

                dialogueContainerObject.DialogueNodeData.Add(nodeData);
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            dialogueContainer.CommentBlockData.Clear();

            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>().Select(x => x.GUID)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            _dialogueContainer = Resources.Load<DialogueContainer>(fileName);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        private void ClearGraph()
        {

            var entryNode = Nodes.Find(x => x.EntryPoint);


            foreach (var node in Nodes.Where(x => !x.EntryPoint).ToList())
            {

                var edgesToRemove = _graphView.edges.Where(e =>
                    e.input.node == node || e.output.node == node).ToList();

                foreach (var edge in edgesToRemove)
                {
                    _graphView.RemoveElement(edge);
                }

                _graphView.RemoveElement(node);
            }

            if (_dialogueContainer.NodeLinks.Count > 0)
            {
                if (entryNode != null)
                {
                    entryNode.GUID = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
                }
            }
        }
        private void GenerateDialogueNodes()
        {
            var existingEntryNode = _graphView.nodes.ToList().Cast<DialogueNode>().FirstOrDefault(x => x.EntryPoint);

            foreach (var nodeData in _dialogueContainer.DialogueNodeData)
            {
         
                if (nodeData.EntryPoint && existingEntryNode != null)
                {
        
                    existingEntryNode.GUID = nodeData.NodeGUID;
                    continue;
                }

                var nodeType = nodeData.IsEndNode ? "end" :
                              nodeData.IsPlayerNode ? "player" :
                              nodeData.EntryPoint ? "entry" : "bot";

                var newNode = _graphView.CreateNode(nodeType, Vector2.zero);
                newNode.GUID = nodeData.NodeGUID;
               newNode.DialogueText = nodeData.DialogueText;
                if (nodeData.IsPlayerNode)
                {
                    newNode.Choices = new List<string>(nodeData.Choices);
                    newNode.ReputationChanges = new List<int>(nodeData.ReputationChanges ?? new List<int>());

                    for (int i = 0; i < nodeData.Choices.Count; i++)
                    {
                        var reputationChange = i < nodeData.ReputationChanges.Count
                            ? nodeData.ReputationChanges[i]
                            : 0;
                        _graphView.AddOptionPort(newNode, nodeData.Choices[i], reputationChange);
                    }
                }
                else if (!nodeData.EntryPoint && !nodeData.IsEndNode)
                {

                    var textField = newNode.mainContainer.Q<TextField>();
                    if (textField != null)
                    {
                    textField.value = nodeData.DialogueText;
                    }
                }

                _graphView.AddElement(newNode);
                newNode.SetPosition(new Rect(nodeData.Position, _graphView.DefaultNodeSize));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();

                for (var j = 0; j < connections.Count; j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.FirstOrDefault(x => x.GUID == targetNodeGUID);

                    if (targetNode == null) continue;

                    if (j < Nodes[i].outputContainer.childCount)
                    {
                        var outputPort = Nodes[i].outputContainer[j].Q<Port>();
                        if (outputPort != null && targetNode.inputContainer.childCount > 0)
                        {
                            LinkNodesTogether(outputPort, (Port)targetNode.inputContainer[0]);
                        }
                    }

                    var targetNodeData = _dialogueContainer.DialogueNodeData.FirstOrDefault(x => x.NodeGUID == targetNodeGUID);
                    if (targetNodeData != null)
                    {
                        targetNode.SetPosition(new Rect(targetNodeData.Position, _graphView.DefaultNodeSize));
                    }
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _dialogueContainer.CommentBlockData)
            {
                var block = _graphView.CreateCommentBlock(
                    new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                    commentBlockData);

                var nodes = Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.GUID));
                block.AddElements(nodes);
            }
        }
    }
}