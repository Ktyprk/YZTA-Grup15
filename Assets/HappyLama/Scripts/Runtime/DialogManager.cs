using HappyLama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HappyLama
{
    public class DialogManager : MonoBehaviour
    {
        [Header("Dialog Configuration")]
        [SerializeField] private DialogueContainer dialogueContainer;
        [SerializeField] private bool autoStartOnAwake = true;

        [Header("Reputation System")]
        [SerializeField] private int currentReputation = 50;
        [SerializeField] private int minReputation = 0;
        [SerializeField] private int maxReputation = 100;
        [SerializeField] private bool showReputationChanges = true;

        public static event Action<string, bool> OnBotMessage;
        public static event Action<List<string>, List<int>> OnPlayerChoices;
        public static event Action<int> OnReputationChanged;
        public static event Action OnDialogueEnd;

        private DialogueNodeData currentNode;
        private Dictionary<string, ExposedProperty> exposedProperties = new Dictionary<string, ExposedProperty>();
        private Queue<DialogueNodeData> dialogQueue = new Queue<DialogueNodeData>();
        private bool isProcessing = false;
        private bool isDialogueActive = false;

        private DialogueTrigger _currentTrigger;
        public bool _isLastNPCMessage = false;

        private const string REPUTATION_KEY = "GlobalReputation";

        private void Awake()
        {
            if (autoStartOnAwake && dialogueContainer != null)
            {
                StartDialogue();
            }
        }

        private void Start()
        {
            LoadReputation();
            if ((dialogueContainer != null))
            {
                InitializeExposedProperties();
            }
            OnReputationChanged?.Invoke(currentReputation);
        }

        public void StartDialogue()
        {
            if (dialogueContainer == null)
            {
                Debug.LogError("DialogueContainer is not assigned!");
                return;
            }

            var entryNode = dialogueContainer.DialogueNodeData.FirstOrDefault(node => node.EntryPoint);
            if (entryNode == null)
            {
                Debug.LogError("No entry point found in dialogue container!");
                return;
            }

            isDialogueActive = true;
            dialogQueue.Clear();
            EnqueueNode(entryNode);
        }

        private void InitializeExposedProperties()
        {
            exposedProperties.Clear();
            foreach (var property in dialogueContainer.ExposedProperties)
            {
                exposedProperties[property.PropertyName] = property;
                if (property.PropertyName == "Reputation")
                {
                    currentReputation = property.IntValue;
                }
            }
        }

        public void SetCurrentTrigger(DialogueTrigger trigger)
        {
            _currentTrigger = trigger;
        }

        public void ApplyReputationChange(int amount)
        {
            ChangeReputation(amount);
        }

        public void ProcessNode(DialogueNodeData node)
        {
            currentNode = node;
            _isLastNPCMessage = IsLastNPCMessage(node);

            if (node.IsEndNode)
            {
                OnBotMessage?.Invoke(node.DialogueText, true);
                return;
            }

            if (node.IsPlayerNode)
            {
                ProcessPlayerNode(node);
            }
            else
            {
                OnBotMessage?.Invoke(node.DialogueText, _isLastNPCMessage);
            }
        }

        public bool IsLastNPCMessage(DialogueNodeData node)
        {
            if (node.IsPlayerNode || node.IsEndNode) return false;

            var nextLinks = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == node.NodeGUID).ToList();
            if (nextLinks.Count == 0) return true;

            foreach (var link in nextLinks)
            {
                var nextNode = dialogueContainer.DialogueNodeData.FirstOrDefault(x => x.NodeGUID == link.TargetNodeGUID);
                if (nextNode != null && !nextNode.IsPlayerNode)
                {
                    return false;
                }
            }

            return true;
        }

        public void TryAdvanceDialogue()
        {
            if (currentNode == null) return;

            if (_isLastNPCMessage)
            {
                EndDialogue();
                return;
            }

            var nextLink = dialogueContainer.NodeLinks.FirstOrDefault(x => x.BaseNodeGUID == currentNode.NodeGUID);
            if (nextLink != null)
            {
                var nextNode = dialogueContainer.DialogueNodeData.FirstOrDefault(x => x.NodeGUID == nextLink.TargetNodeGUID);
                if (nextNode != null)
                {
                    ProcessNode(nextNode);
                }
            }
            else
            {
                EndDialogue();
            }
        }

        private void ProcessPlayerNode(DialogueNodeData node)
        {
            var choices = new List<string>();
            var reputationChanges = new List<int>();

            for (int i = 0; i < node.Choices.Count; i++)
            {
                choices.Add(node.Choices[i]);
                var repChange = i < node.ReputationChanges.Count ? node.ReputationChanges[i] : 0;
                reputationChanges.Add(repChange);
            }

            OnPlayerChoices?.Invoke(choices, reputationChanges);
        }

        public void EnqueueNode(DialogueNodeData node)
        {
            dialogQueue.Enqueue(node);
            if (!isProcessing)
            {
                ProcessNextNode();
            }
        }

        public void ProcessNextNode()
        {
            if (dialogQueue.Count > 0)
            {
                var nextNode = dialogQueue.Dequeue();
                ProcessNode(nextNode);
            }
            else
            {
                isProcessing = false;
            }
        }

        public void SelectChoice(int choiceIndex)
        {
            if (currentNode == null || !currentNode.IsPlayerNode)
            {
                Debug.LogError("No active player node!");
                return;
            }

            if (choiceIndex < 0 || choiceIndex >= currentNode.Choices.Count)
            {
                Debug.LogError($"Invalid choice index: {choiceIndex}");
                return;
            }

            if (_currentTrigger != null && choiceIndex < currentNode.ReputationChanges.Count)
            {
                var repChange = currentNode.ReputationChanges[choiceIndex];
                if (repChange != 0)
                {
                    _currentTrigger.ModifyReputation(repChange);
                }
            }

            var choicePortName = $"Option {choiceIndex + 1}";
            var nextNode = GetNextNode(currentNode.NodeGUID, choicePortName);

            if (nextNode != null)
            {
                EnqueueNode(nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        public DialogueTrigger GetCurrentTrigger()
        {
            return _currentTrigger;
        }

        public void ChangeReputation(int change)
        {
            currentReputation = Mathf.Clamp(currentReputation + change, minReputation, maxReputation);
            SaveReputation();
            OnReputationChanged?.Invoke(currentReputation);
        }

        private void SaveReputation()
        {
            PlayerPrefs.SetInt(REPUTATION_KEY, currentReputation);
            PlayerPrefs.Save();
        }

        private void LoadReputation()
        {
            if (PlayerPrefs.HasKey(REPUTATION_KEY))
            {
                currentReputation = PlayerPrefs.GetInt(REPUTATION_KEY);
            }
            else
            {
                currentReputation = 50;
            }
        }

        public void ResetReputation()
        {
            currentReputation = 50;
            SaveReputation();
            OnReputationChanged?.Invoke(currentReputation);
        }

        public DialogueNodeData GetNextNode(string currentNodeGUID, string portName)
        {
            var link = dialogueContainer.NodeLinks.FirstOrDefault(l =>
                l.BaseNodeGUID == currentNodeGUID && l.PortName == portName);

            if (link == null) return null;

            return dialogueContainer.DialogueNodeData.FirstOrDefault(n =>
                n.NodeGUID == link.TargetNodeGUID);
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            currentNode = null;

         
            OnDialogueEnd?.Invoke();

            if (MovementManager.Instance != null)
            {
                MovementManager.Instance.SetMovementEnabled(true);
            }
        }

        public int GetCurrentReputation() => currentReputation;
        public DialogueContainer GetDialogueContainer() => dialogueContainer;
        public DialogueNodeData GetCurrentNode() => currentNode;

        public void SetDialogueContainer(DialogueContainer container)
        {
            dialogueContainer = container;
            InitializeExposedProperties();
        }
    }
}