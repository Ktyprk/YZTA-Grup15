using HappyLama;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HappyLama
{
    public class DialogUIManager : MonoBehaviour
    {
        public enum TextAnimationType
        {
            None,
            Typewriter,
            FadeIn,
            RainbowWave,
            Bounce,
            Wave,
            Jitter,
            Glitch,
            Shake,
            Gradient,
            Sparkle
        }

        [Header("UI References")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private TextMeshProUGUI npcText;
        [SerializeField] private Transform choicesPanel;
        [SerializeField] private TextMeshProUGUI reputationText;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Color selectedColor = new Color(0.58f, 0.51f, 0.51f, 1f);
        [SerializeField] private Color normalColor = new Color(0.65f, 0.65f, 0.65f, 0.5f);
        [SerializeField] private float arrowAnimationSpeed = 2f;
        [SerializeField] private float arrowAnimationDistance = 5f;

        [Header("Text Animation Settings")]
        [SerializeField] private TextAnimationType defaultAnimation = TextAnimationType.Typewriter;
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float rainbowSpeed = 5f;
        [SerializeField] private float bounceIntensity = 5f;
        [SerializeField] private float waveFrequency = 5f;
        [SerializeField] private float waveAmplitude = 2f;
        [SerializeField] private float jitterIntensity = 3f;
        [SerializeField] private float glitchInterval = 0.1f;
        [SerializeField] private float shakeIntensity = 10f;
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private float sparkleFrequency = 0.5f;

        private DialogManager dialogManager;
        private List<GameObject> currentChoiceButtons = new List<GameObject>();
        private int currentSelectionIndex = 0;
        private bool isSelectingChoices = false;
        private float inputCooldown = 0.2f;
        private float lastInputTime = 0f;
        private bool isNPCMessageShown = false;
        private bool isDialogueActive = false; // Diyalog aktif mi kontrolü için
        private Dictionary<GameObject, Coroutine> arrowAnimations = new Dictionary<GameObject, Coroutine>();
        private Coroutine currentTextAnimation;
        private TextAnimationType currentAnimationType;

        private void Awake()
        {
            dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager == null)
            {
                Debug.LogError("DialogManager not found in scene!");
                return;
            }

            DialogManager.OnBotMessage += ShowNPCMessage;
            DialogManager.OnPlayerChoices += ShowPlayerChoices;
            DialogManager.OnReputationChanged += UpdateReputationUI;
            DialogManager.OnDialogueEnd += HideDialogUI;

            dialogPanel.SetActive(false);
        }

        private void Start()
        {
            UpdateReputationUI(dialogManager.GetCurrentReputation());
        }

        private void OnDestroy()
        {
            DialogManager.OnBotMessage -= ShowNPCMessage;
            DialogManager.OnPlayerChoices -= ShowPlayerChoices;
            DialogManager.OnReputationChanged -= UpdateReputationUI;
            DialogManager.OnDialogueEnd -= HideDialogUI;

            foreach (var anim in arrowAnimations.Values)
            {
                if (anim != null) StopCoroutine(anim);
            }
        }

        private void Update()
        {
            if (Time.time < lastInputTime + inputCooldown)
                return;

            // E tuşu kontrolü - sadece diyalog aktifken
            if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
            {
                if (isSelectingChoices && currentChoiceButtons.Count > 0)
                {
                    ConfirmSelection();
                }
                else if (isNPCMessageShown)
                {
                    ContinueDialogue();
                }
                lastInputTime = Time.time;
                return;
            }

            if (isSelectingChoices && currentChoiceButtons.Count > 0)
            {
                HandleSelectionInput();
            }
        }


      
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        public void StartDialogueUI(DialogueContainer dialogueContainer, DialogueTrigger trigger, TextAnimationType animationType = TextAnimationType.None)
        {
            Debug.Log("🔥 StartDialogueUI çağrıldı - hareket durduruluyor!");

            if (animationType == TextAnimationType.None)
                animationType = defaultAnimation;

            currentAnimationType = animationType;
            dialogManager.SetCurrentTrigger(trigger);
            dialogManager.SetDialogueContainer(dialogueContainer);
            dialogPanel.SetActive(true);
            isDialogueActive = true;

            // *** HAREKET DURDUR - BASIT YOL ***
            var fpsController = FindObjectOfType<FPSController>();
            if (fpsController != null)
            {
                if (fpsController is IMovementController movementController)
                {
                    movementController.CanMove = false;
                    Debug.Log("✅ FPSController hareketi durduruldu!");
                }
                else
                {
                    Debug.LogError("❌ FPSController IMovementController implement etmiyor!");
                }
            }
            else
            {
                Debug.LogError("❌ FPSController bulunamadı!");
            }

            // Cursor'u serbest bırak
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("✅ Cursor serbest bırakıldı");

            dialogManager.StartDialogue();
        }
        private void ShowNPCMessage(string message, bool closeAfter)
        {
            isSelectingChoices = false;
            isNPCMessageShown = true;
            choicesPanel.gameObject.SetActive(false);

            if (currentTextAnimation != null)
                StopCoroutine(currentTextAnimation);

            switch (currentAnimationType)
            {
                case TextAnimationType.Typewriter:
                    currentTextAnimation = StartCoroutine(TypewriterEffect(message));
                    break;
                case TextAnimationType.FadeIn:
                    currentTextAnimation = StartCoroutine(FadeInEffect(message));
                    break;
                case TextAnimationType.RainbowWave:
                    currentTextAnimation = StartCoroutine(RainbowWaveEffect(message));
                    break;
                case TextAnimationType.Bounce:
                    currentTextAnimation = StartCoroutine(BounceEffect(message));
                    break;
                case TextAnimationType.Wave:
                    currentTextAnimation = StartCoroutine(WaveEffect(message));
                    break;
                case TextAnimationType.Jitter:
                    currentTextAnimation = StartCoroutine(JitterEffect(message));
                    break;
                case TextAnimationType.Glitch:
                    currentTextAnimation = StartCoroutine(GlitchEffect(message));
                    break;
                case TextAnimationType.Shake:
                    currentTextAnimation = StartCoroutine(ShakeEffect(message));
                    break;
                case TextAnimationType.Gradient:
                    currentTextAnimation = StartCoroutine(GradientEffect(message));
                    break;
                case TextAnimationType.Sparkle:
                    currentTextAnimation = StartCoroutine(SparkleEffect(message));
                    break;
                default:
                    npcText.text = message;
                    break;
            }

            npcText.gameObject.SetActive(true);
            ClearChoices();
        }

        #region Text Animation Effects
        private IEnumerator TypewriterEffect(string message)
        {
            npcText.text = "";
            foreach (char c in message)
            {
                npcText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }
        }

        private IEnumerator FadeInEffect(string message)
        {
            npcText.text = message;
            npcText.color = new Color(npcText.color.r, npcText.color.g, npcText.color.b, 0);

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                float alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
                npcText.color = new Color(npcText.color.r, npcText.color.g, npcText.color.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            npcText.color = new Color(npcText.color.r, npcText.color.g, npcText.color.b, 1);
        }

        private IEnumerator RainbowWaveEffect(string message)
        {
            npcText.text = message;
            npcText.ForceMeshUpdate();

            yield return null;

            TMP_TextInfo textInfo = npcText.textInfo;

            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
            Color32[][] originalColors = new Color32[textInfo.meshInfo.Length][];

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
                originalColors[i] = textInfo.meshInfo[i].colors32.Clone() as Color32[];
            }

            while (true)
            {
                textInfo = npcText.textInfo;

                if (textInfo.characterCount != message.Length)
                {
                    npcText.ForceMeshUpdate();
                    textInfo = npcText.textInfo;
                    yield return null;
                    continue;
                }

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    float hue = (Time.time * rainbowSpeed + i * 0.1f) % 1f;
                    Color32 rainbowColor = Color.HSVToRGB(hue, 0.8f, 1f);
                    float waveOffset = Mathf.Sin(Time.time * waveFrequency + i * 0.2f) * waveAmplitude;

                    for (int j = 0; j < 4; j++)
                    {
                        if (vertexIndex + j < textInfo.meshInfo[materialIndex].vertices.Length)
                        {
                            textInfo.meshInfo[materialIndex].vertices[vertexIndex + j] =
                                originalVertices[materialIndex][vertexIndex + j] +
                                new Vector3(0, waveOffset, 0);

                            textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = rainbowColor;
                        }
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    if (textInfo.meshInfo[i].mesh != null)
                    {
                        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                        textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                        npcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator BounceEffect(string message)
        {
            npcText.text = message;
            Vector3 originalScale = npcText.transform.localScale;

            while (true)
            {
                float bounce = Mathf.Abs(Mathf.Sin(Time.time * bounceIntensity)) * 0.2f + 0.8f;
                npcText.transform.localScale = originalScale * bounce;
                yield return null;
            }
        }

        private IEnumerator WaveEffect(string message)
        {
            npcText.text = message;
            npcText.ForceMeshUpdate();
            TMP_TextInfo textInfo = npcText.textInfo;

            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
            }

            while (true)
            {
                if (textInfo.characterCount > 0)
                {
                    for (int i = 0; i < textInfo.characterCount; i++)
                    {
                        var charInfo = textInfo.characterInfo[i];
                        if (!charInfo.isVisible) continue;

                        int materialIndex = charInfo.materialReferenceIndex;
                        int vertexIndex = charInfo.vertexIndex;
                        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                        for (int j = 0; j < 4; j++)
                        {
                            if (originalVertices[materialIndex] != null)
                            {
                                float waveOffset = Mathf.Sin(Time.time * waveFrequency + i * 0.2f) * waveAmplitude;
                                vertices[vertexIndex + j] = originalVertices[materialIndex][vertexIndex + j] +
                                                          new Vector3(0, waveOffset, 0);
                            }
                        }
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                        npcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }
                yield return null;
            }
        }

        private IEnumerator JitterEffect(string message)
        {
            npcText.text = message;
            TMP_TextInfo textInfo = npcText.textInfo;
            Vector3[] originalVertices = new Vector3[textInfo.characterCount * 4];

            while (true)
            {
                if (textInfo.characterCount > 0)
                {
                    for (int i = 0; i < textInfo.characterCount; i++)
                    {
                        var charInfo = textInfo.characterInfo[i];
                        if (!charInfo.isVisible) continue;

                        int vertexIndex = charInfo.vertexIndex;
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                        for (int j = 0; j < 4; j++)
                        {
                            Vector3 jitter = new Vector3(
                                Random.Range(-jitterIntensity, jitterIntensity),
                                Random.Range(-jitterIntensity, jitterIntensity),
                                0
                            );
                            vertices[vertexIndex + j] = originalVertices[vertexIndex + j] + jitter;
                        }
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                        npcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }
                yield return null;
            }
        }

        private IEnumerator GlitchEffect(string message)
        {
            string originalMessage = message;
            while (true)
            {
                char[] glitched = originalMessage.ToCharArray();
                for (int i = 0; i < glitched.Length; i++)
                {
                    if (Random.value > 0.9f)
                    {
                        glitched[i] = (char)Random.Range(33, 126);
                    }
                }
                npcText.text = new string(glitched);
                yield return new WaitForSeconds(glitchInterval);
            }
        }

        private IEnumerator ShakeEffect(string message)
        {
            npcText.text = message;
            Vector3 originalPosition = npcText.transform.localPosition;

            while (true)
            {
                npcText.transform.localPosition = originalPosition +
                    new Vector3(
                        Random.Range(-shakeIntensity, shakeIntensity),
                        Random.Range(-shakeIntensity, shakeIntensity),
                        0
                    );
                yield return null;
            }
        }

        private IEnumerator GradientEffect(string message)
        {
            npcText.text = message;
            TMP_TextInfo textInfo = npcText.textInfo;

            while (true)
            {
                if (textInfo.characterCount > 0)
                {
                    for (int i = 0; i < textInfo.characterCount; i++)
                    {
                        var charInfo = textInfo.characterInfo[i];
                        if (!charInfo.isVisible) continue;

                        int vertexIndex = charInfo.vertexIndex;
                        Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                        float t = (Time.time * 0.5f + i * 0.05f) % 1f;
                        Color32 gradientColor = colorGradient.Evaluate(t);

                        for (int j = 0; j < 4; j++)
                        {
                            colors[vertexIndex + j] = gradientColor;
                        }
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                        npcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }
                yield return null;
            }
        }

        private IEnumerator SparkleEffect(string message)
        {
            npcText.text = message;
            npcText.ForceMeshUpdate();
            TMP_TextInfo textInfo = npcText.textInfo;

            Color32[][] originalColors = new Color32[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalColors[i] = textInfo.meshInfo[i].colors32.Clone() as Color32[];
            }

            while (true)
            {
                if (textInfo.characterCount > 0)
                {
                    for (int i = 0; i < textInfo.characterCount; i++)
                    {
                        var charInfo = textInfo.characterInfo[i];
                        if (!charInfo.isVisible) continue;

                        int materialIndex = charInfo.materialReferenceIndex;
                        int vertexIndex = charInfo.vertexIndex;
                        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

                        bool shouldSparkle = Random.value < sparkleFrequency;

                        for (int j = 0; j < 4; j++)
                        {
                            colors[vertexIndex + j] = shouldSparkle ?
                                Color.white :
                                originalColors[materialIndex][vertexIndex + j];
                        }
                    }

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                        npcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        #endregion

        private void ShowPlayerChoices(List<string> choices, List<int> reputationChanges)
        {
            isSelectingChoices = true;
            isNPCMessageShown = false;
            npcText.gameObject.SetActive(false);
            choicesPanel.gameObject.SetActive(true);

            if (currentTextAnimation != null)
            {
                StopCoroutine(currentTextAnimation);
                currentTextAnimation = null;
            }

            ClearChoices();

            for (int i = 0; i < choices.Count; i++)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesPanel);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                Image buttonImage = buttonObj.GetComponent<Image>();

                string choiceText = choices[i];
                int repChange = i < reputationChanges.Count ? reputationChanges[i] : 0;
                string repText = repChange != 0 ? $" ({GetReputationChangeText(repChange)})" : "";

                buttonText.text = $"{choiceText}{repText}";
                buttonText.color = i == 0 ? Color.white : new Color(1f, 1f, 1f, 0.8f);

                if (buttonImage != null)
                {
                    buttonImage.color = (i == 0) ? selectedColor : normalColor;
                }

                Transform arrow = buttonObj.transform.Find("Arrow");
                if (arrow != null)
                {
                    arrow.gameObject.SetActive(i == 0);
                    if (i == 0)
                    {
                        arrowAnimations[buttonObj] = StartCoroutine(AnimateArrow(arrow));
                    }
                }

                int choiceIndex = i;
                button.onClick.AddListener(() => SelectChoice(choiceIndex));

                currentChoiceButtons.Add(buttonObj);
            }

            currentSelectionIndex = 0;
        }

        private void SelectChoice(int choiceIndex)
        {
          
            dialogManager.SelectChoice(choiceIndex);
            dialogManager.ProcessNextNode();
        }

        private void ContinueDialogue()
        {
            var currentNode = dialogManager.GetCurrentNode();
            if (currentNode == null) return;

            var dialogueContainer = dialogManager.GetDialogueContainer();
            var nextLinks = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == currentNode.NodeGUID).ToList();

            if (nextLinks.Count > 0)
            {
                var nextLink = nextLinks.First();
                var nextNode = dialogueContainer.DialogueNodeData.FirstOrDefault(x => x.NodeGUID == nextLink.TargetNodeGUID);

                if (nextNode != null)
                {
                    dialogManager.EnqueueNode(nextNode);
                }
                else
                {
                    HideDialogUI();
                }
            }
            else
            {
                HideDialogUI();
            }
        }

        private void UpdateReputationUI(int reputation)
        {
            var currentTrigger = dialogManager.GetCurrentTrigger();
            if (currentTrigger == null || !currentTrigger.useLocalReputationOnly)
            {
                if (reputationText != null)
                {
                    reputationText.text = $"Reputation: {reputation}/100";
                    reputationText.color = GetReputationColor(reputation);
                }
            }
        }

        private Color GetReputationColor(int value)
        {
            if (value < 30) return Color.red;
            if (value > 70) return Color.green;
            return Color.yellow;
        }

        private string GetReputationChangeText(int value)
        {
            return value > 0 ? $"+{value}" : value.ToString();
        }

        private void ClearChoices()
        {
            foreach (var anim in arrowAnimations.Values)
            {
                if (anim != null) StopCoroutine(anim);
            }
            arrowAnimations.Clear();

            foreach (var button in currentChoiceButtons)
            {
                Destroy(button);
            }
            currentChoiceButtons.Clear();
            currentSelectionIndex = 0;
        }


        public void HideDialogUI()
        {
            Debug.Log("🔥 HideDialogUI çağrıldı - hareket açılıyor!");

            dialogPanel.SetActive(false);
            npcText.gameObject.SetActive(false);
            choicesPanel.gameObject.SetActive(false);
            isSelectingChoices = false;
            isNPCMessageShown = false;
            isDialogueActive = false;

            if (currentTextAnimation != null)
            {
                StopCoroutine(currentTextAnimation);
                currentTextAnimation = null;
            }

            // *** HAREKET AÇ - BASIT YOL ***
            var fpsController = FindObjectOfType<FPSController>();
            if (fpsController != null)
            {
                if (fpsController is IMovementController movementController)
                {
                    movementController.CanMove = true;
                    Debug.Log("✅ FPSController hareketi açıldı!");
                }
            }

            // Cursor'u kilitle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("✅ Cursor kilitlendi");
        }
        private IEnumerator AnimateArrow(Transform arrow)
        {
            Vector3 originalPosition = arrow.localPosition;
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime * arrowAnimationSpeed;
                float offset = Mathf.Sin(time) * arrowAnimationDistance;
                arrow.localPosition = originalPosition + new Vector3(offset, 0, 0);
                yield return null;
            }
        }

        private void HandleSelectionInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                NavigateSelection(-1);
                lastInputTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                NavigateSelection(1);
                lastInputTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmSelection();
                lastInputTime = Time.time;
            }
        }

        private void NavigateSelection(int direction)
        {
            UpdateButtonAppearance(currentSelectionIndex, false);

            currentSelectionIndex += direction;
            if (currentSelectionIndex < 0)
                currentSelectionIndex = currentChoiceButtons.Count - 1;
            else if (currentSelectionIndex >= currentChoiceButtons.Count)
                currentSelectionIndex = 0;

            UpdateButtonAppearance(currentSelectionIndex, true);
        }

        private void UpdateButtonAppearance(int index, bool isSelected)
        {
            if (index < 0 || index >= currentChoiceButtons.Count)
                return;

            var buttonObj = currentChoiceButtons[index];
            var buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = isSelected ? selectedColor : normalColor;
            }

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.8f);
            }

            Transform arrow = buttonObj.transform.Find("Arrow");
            if (arrow != null)
            {
                arrow.gameObject.SetActive(isSelected);

                if (isSelected)
                {
                    if (arrowAnimations.ContainsKey(buttonObj) && arrowAnimations[buttonObj] != null)
                    {
                        StopCoroutine(arrowAnimations[buttonObj]);
                    }
                    arrowAnimations[buttonObj] = StartCoroutine(AnimateArrow(arrow));
                }
                else
                {
                    if (arrowAnimations.ContainsKey(buttonObj) && arrowAnimations[buttonObj] != null)
                    {
                        StopCoroutine(arrowAnimations[buttonObj]);
                        arrowAnimations[buttonObj] = null;
                    }
                }
            }
        }

        public void SetAnimationParameters(
            TextAnimationType animationType,
            float typewriterSpeed,
            float fadeInDuration,
            float rainbowSpeed,
            float waveFrequency,
            float waveAmplitude,
            float jitterIntensity,
            float glitchInterval,
            float shakeIntensity,
            Gradient colorGradient,
            float sparkleFrequency,
            float bounceIntensity)
        {
            this.currentAnimationType = animationType;
            this.typewriterSpeed = typewriterSpeed;
            this.fadeInDuration = fadeInDuration;
            this.rainbowSpeed = rainbowSpeed;
            this.waveFrequency = waveFrequency;
            this.waveAmplitude = waveAmplitude;
            this.jitterIntensity = jitterIntensity;
            this.glitchInterval = glitchInterval;
            this.shakeIntensity = shakeIntensity;
            this.colorGradient = colorGradient;
            this.sparkleFrequency = sparkleFrequency;
            this.bounceIntensity = bounceIntensity;
        }

        private void ConfirmSelection()
        {
            SelectChoice(currentSelectionIndex);
        }
    }
}