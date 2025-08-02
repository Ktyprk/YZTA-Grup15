using UnityEngine;
using System.Collections.Generic;
using HappyLama;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueContainer firstTimeDialogue;
    public bool hasSpokenBefore = false;
    public List<DialogueContainer> positiveDialogues;
    public List<DialogueContainer> negativeDialogues;
    public List<DialogueContainer> defaultDialogues;

    [Header("Reputation Settings")]
    [Tooltip("If checked, reputation changes and dialogue selection will be based on this character's reputation only")]
    public bool useLocalReputationOnly = true;
    [SerializeField, Range(-100, 100)] private int characterReputation = 0;

    // Kullanýlmýþ dialog'larý takip etmek için HashSet'ler
    private HashSet<int> usedPositiveDialogues = new HashSet<int>();
    private HashSet<int> usedNegativeDialogues = new HashSet<int>();
    private HashSet<int> usedDefaultDialogues = new HashSet<int>();

    private string characterID;

    private void Awake()
    {
        // Her karakter için benzersiz ID oluþtur
        characterID = $"NPC_{gameObject.name}_{transform.GetSiblingIndex()}";
    }
    public int CharacterReputation
    {
        get => characterReputation;
        private set
        {
            characterReputation = Mathf.Clamp(value, -100, 100);
            SaveCharacterReputation();
        }
    }

    [Header("Interaction Settings")]
    public bool useTrigger = true;
    public bool useRaycast;
    public KeyCode interactionKey = KeyCode.E;
    private bool isPlayerInRange;
    private DialogUIManager dialogUIManager;

    [Header("Text Animation Settings")]
    public DialogUIManager.TextAnimationType textAnimation = DialogUIManager.TextAnimationType.None;

    [HideInInspector] public float typewriterSpeed = 0.05f;
    [HideInInspector] public float fadeInDuration = 1f;
    [HideInInspector] public float rainbowSpeed = 5f;
    [HideInInspector] public float waveFrequency = 5f;
    [HideInInspector] public float waveAmplitude = 2f;
    [HideInInspector] public float jitterIntensity = 3f;
    [HideInInspector] public float glitchInterval = 0.1f;
    [HideInInspector] public float shakeIntensity = 10f;
    [HideInInspector] public Gradient colorGradient;
    [HideInInspector] public float sparkleFrequency = 0.5f;
    [HideInInspector] public float bounceIntensity = 5f;


    private void Start()
    {
        LoadCharacterReputation();
        LoadUsedDialogues();
        LoadHasSpokenBefore(); // hasSpokenBefore durumunu da yükle
        dialogUIManager = FindObjectOfType<DialogUIManager>();
        if (useTrigger) GetComponent<Collider>().isTrigger = true;

        Debug.Log($"{name}: Dialog sistemi yüklendi. Character ID: {characterID}");
    }

    private void SaveCharacterReputation()
    {
        PlayerPrefs.SetInt(GetReputationKey(), characterReputation);
        PlayerPrefs.Save();
    }

    private void LoadCharacterReputation()
    {
        string key = GetReputationKey();
        if (PlayerPrefs.HasKey(key))
        {
            characterReputation = PlayerPrefs.GetInt(key);
        }
        else
        {
            characterReputation = 0;
        }
    }

    private void SaveUsedDialogues()
    {
        // Pozitif dialoglarý kaydet
        string positiveKey = GetUsedDialoguesKey("positive");
        string positiveData = string.Join(",", usedPositiveDialogues);
        PlayerPrefs.SetString(positiveKey, positiveData);

        // Negatif dialoglarý kaydet
        string negativeKey = GetUsedDialoguesKey("negative");
        string negativeData = string.Join(",", usedNegativeDialogues);
        PlayerPrefs.SetString(negativeKey, negativeData);

        // Default dialoglarý kaydet
        string defaultKey = GetUsedDialoguesKey("default");
        string defaultData = string.Join(",", usedDefaultDialogues);
        PlayerPrefs.SetString(defaultKey, defaultData);

        // hasSpokenBefore durumunu kaydet
        SaveHasSpokenBefore();

        PlayerPrefs.Save();
        Debug.Log($"{name}: Dialog durumlarý PlayerPrefs'e kaydedildi.");
    }

    private void SaveHasSpokenBefore()
    {
        string hasSpokenKey = GetHasSpokenKey();
        PlayerPrefs.SetInt(hasSpokenKey, hasSpokenBefore ? 1 : 0);
    }

    private void LoadHasSpokenBefore()
    {
        string hasSpokenKey = GetHasSpokenKey();
        if (PlayerPrefs.HasKey(hasSpokenKey))
        {
            hasSpokenBefore = PlayerPrefs.GetInt(hasSpokenKey) == 1;
            Debug.Log($"{name}: hasSpokenBefore durumu yüklendi: {hasSpokenBefore}");
        }
    }

    private void LoadUsedDialogues()
    {
        // Pozitif dialoglarý yükle
        string positiveKey = GetUsedDialoguesKey("positive");
        if (PlayerPrefs.HasKey(positiveKey))
        {
            string positiveData = PlayerPrefs.GetString(positiveKey);
            if (!string.IsNullOrEmpty(positiveData))
            {
                foreach (string index in positiveData.Split(','))
                {
                    if (int.TryParse(index, out int idx))
                        usedPositiveDialogues.Add(idx);
                }
            }
        }

        // Negatif dialoglarý yükle
        string negativeKey = GetUsedDialoguesKey("negative");
        if (PlayerPrefs.HasKey(negativeKey))
        {
            string negativeData = PlayerPrefs.GetString(negativeKey);
            if (!string.IsNullOrEmpty(negativeData))
            {
                foreach (string index in negativeData.Split(','))
                {
                    if (int.TryParse(index, out int idx))
                        usedNegativeDialogues.Add(idx);
                }
            }
        }

        // Default dialoglarý yükle
        string defaultKey = GetUsedDialoguesKey("default");
        if (PlayerPrefs.HasKey(defaultKey))
        {
            string defaultData = PlayerPrefs.GetString(defaultKey);
            if (!string.IsNullOrEmpty(defaultData))
            {
                foreach (string index in defaultData.Split(','))
                {
                    if (int.TryParse(index, out int idx))
                        usedDefaultDialogues.Add(idx);
                }
            }
        }

        Debug.Log($"{name}: Dialog durumlarý PlayerPrefs'ten yüklendi. " +
                  $"Positive: {usedPositiveDialogues.Count}, " +
                  $"Negative: {usedNegativeDialogues.Count}, " +
                  $"Default: {usedDefaultDialogues.Count}");
    }

    private string GetReputationKey()
    {
        return $"CharRep_{characterID}";
    }

    private string GetUsedDialoguesKey(string dialogueType)
    {
        return $"UsedDialogues_{characterID}_{dialogueType}";
    }

    private string GetHasSpokenKey()
    {
        return $"HasSpoken_{characterID}";
    }

    private void Update()
    {
        if (!isPlayerInRange || !Input.GetKeyDown(interactionKey)) return;
        StartDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInRange = true;
        if (useTrigger) StartDialogue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInRange = false;
    }

    public void StartDialogue()
    {
        if (dialogUIManager == null)
        {
            Debug.LogWarning("No DialogUIManager found in scene!");
            return;
        }

        var container = GetDialogueBasedOnContext();
        if (container != null)
        {
            if (!hasSpokenBefore)
            {
                hasSpokenBefore = true;
                SaveHasSpokenBefore(); 
                PlayerPrefs.Save();
                Debug.Log($"{name}: Ýlk kez konuþuluyor, durum kaydedildi.");
            }

            dialogUIManager.SetAnimationParameters(
                textAnimation,
                typewriterSpeed,
                fadeInDuration,
                rainbowSpeed,
                waveFrequency,
                waveAmplitude,
                jitterIntensity,
                glitchInterval,
                shakeIntensity,
                colorGradient,
                sparkleFrequency,
                bounceIntensity
            );

            dialogUIManager.StartDialogueUI(container, this, textAnimation);
        }
        else
        {
            Debug.Log($"{name}: No more available dialogues to show.");
        }
    }

    private DialogueContainer GetDialogueBasedOnContext()
    {
      
        if (!hasSpokenBefore && firstTimeDialogue != null)
            return firstTimeDialogue;

        int reputationValue = useLocalReputationOnly
            ? CharacterReputation
            : FindObjectOfType<DialogManager>().GetCurrentReputation();

        if (reputationValue > 0)
        {
            return GetUnusedDialogue(positiveDialogues, usedPositiveDialogues);
        }
        else if (reputationValue < 0)
        {
            return GetUnusedDialogue(negativeDialogues, usedNegativeDialogues);
        }
        else
        {
            return GetUnusedDialogue(defaultDialogues, usedDefaultDialogues);
        }
    }

    private DialogueContainer GetUnusedDialogue(List<DialogueContainer> dialogues, HashSet<int> usedDialogues)
    {
        if (dialogues == null || dialogues.Count == 0)
            return null;


        for (int i = 0; i < dialogues.Count; i++)
        {
            if (!usedDialogues.Contains(i))
            {

                usedDialogues.Add(i);
                SaveUsedDialogues();
                return dialogues[i];
            }
        }

        return null;
    }

    public void ModifyReputation(int amount)
    {
        if (useLocalReputationOnly)
        {
            CharacterReputation += amount;
            Debug.Log($"{name}'s local reputation changed by {amount}. New value: {CharacterReputation}");
        }
        else
        {
            var dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager != null)
            {
                dialogManager.ChangeReputation(amount);
                Debug.Log($"Global reputation changed by {amount}. New value: {dialogManager.GetCurrentReputation()}");
            }
        }
    }

    public void ResetCharacterReputation()
    {
        CharacterReputation = 0;
    }

    public void ResetDialogueSequence()
    {
        // Tüm kullanýlmýþ dialog listelerini temizle
        usedPositiveDialogues.Clear();
        usedNegativeDialogues.Clear();
        usedDefaultDialogues.Clear();
        hasSpokenBefore = false;

        // PlayerPrefs'i de temizle
        PlayerPrefs.DeleteKey(GetUsedDialoguesKey("positive"));
        PlayerPrefs.DeleteKey(GetUsedDialoguesKey("negative"));
        PlayerPrefs.DeleteKey(GetUsedDialoguesKey("default"));
        PlayerPrefs.DeleteKey(GetHasSpokenKey());
        PlayerPrefs.Save();

        Debug.Log($"{name}: Tüm dialog durumlarý sýfýrlandý ve PlayerPrefs'ten silindi.");
    }

    // Inspector'da debug için
    [Header("Debug Info")]
    [SerializeField, Tooltip("Shows used positive dialogue indices")]
    private int[] debugUsedPositive;
    [SerializeField, Tooltip("Shows used negative dialogue indices")]
    private int[] debugUsedNegative;
    [SerializeField, Tooltip("Shows used default dialogue indices")]
    private int[] debugUsedDefault;

    private void OnValidate()
    {
        // Debug bilgilerini güncelle
        if (usedPositiveDialogues != null)
        {
            debugUsedPositive = new int[usedPositiveDialogues.Count];
            usedPositiveDialogues.CopyTo(debugUsedPositive);
        }
        if (usedNegativeDialogues != null)
        {
            debugUsedNegative = new int[usedNegativeDialogues.Count];
            usedNegativeDialogues.CopyTo(debugUsedNegative);
        }
        if (usedDefaultDialogues != null)
        {
            debugUsedDefault = new int[usedDefaultDialogues.Count];
            usedDefaultDialogues.CopyTo(debugUsedDefault);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    private SerializedProperty textAnimationProp;
    private SerializedProperty useLocalReputationOnlyProp;
    private Dictionary<string, SerializedProperty> animationProperties = new Dictionary<string, SerializedProperty>();

    private void OnEnable()
    {
        textAnimationProp = serializedObject.FindProperty("textAnimation");
        useLocalReputationOnlyProp = serializedObject.FindProperty("useLocalReputationOnly");

        animationProperties.Add("typewriterSpeed", serializedObject.FindProperty("typewriterSpeed"));
        animationProperties.Add("fadeInDuration", serializedObject.FindProperty("fadeInDuration"));
        animationProperties.Add("rainbowSpeed", serializedObject.FindProperty("rainbowSpeed"));
        animationProperties.Add("waveFrequency", serializedObject.FindProperty("waveFrequency"));
        animationProperties.Add("waveAmplitude", serializedObject.FindProperty("waveAmplitude"));
        animationProperties.Add("jitterIntensity", serializedObject.FindProperty("jitterIntensity"));
        animationProperties.Add("glitchInterval", serializedObject.FindProperty("glitchInterval"));
        animationProperties.Add("shakeIntensity", serializedObject.FindProperty("shakeIntensity"));
        animationProperties.Add("colorGradient", serializedObject.FindProperty("colorGradient"));
        animationProperties.Add("sparkleFrequency", serializedObject.FindProperty("sparkleFrequency"));
        animationProperties.Add("bounceIntensity", serializedObject.FindProperty("bounceIntensity"));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "textAnimation",
            "typewriterSpeed", "fadeInDuration", "rainbowSpeed",
            "waveFrequency", "waveAmplitude", "jitterIntensity",
            "glitchInterval", "shakeIntensity", "colorGradient",
            "sparkleFrequency", "bounceIntensity", "debugUsedPositive",
            "debugUsedNegative", "debugUsedDefault");

        EditorGUILayout.PropertyField(textAnimationProp);

        switch ((DialogUIManager.TextAnimationType)textAnimationProp.enumValueIndex)
        {
            case DialogUIManager.TextAnimationType.Typewriter:
                DrawProperty("typewriterSpeed");
                break;

            case DialogUIManager.TextAnimationType.FadeIn:
                DrawProperty("fadeInDuration");
                break;

            case DialogUIManager.TextAnimationType.RainbowWave:
                DrawProperty("rainbowSpeed");
                DrawProperty("waveFrequency");
                DrawProperty("waveAmplitude");
                break;

            case DialogUIManager.TextAnimationType.Wave:
                DrawProperty("waveFrequency");
                DrawProperty("waveAmplitude");
                break;

            case DialogUIManager.TextAnimationType.Jitter:
                DrawProperty("jitterIntensity");
                break;

            case DialogUIManager.TextAnimationType.Glitch:
                DrawProperty("glitchInterval");
                break;

            case DialogUIManager.TextAnimationType.Shake:
                DrawProperty("shakeIntensity");
                break;

            case DialogUIManager.TextAnimationType.Gradient:
                DrawProperty("colorGradient");
                break;

            case DialogUIManager.TextAnimationType.Sparkle:
                DrawProperty("sparkleFrequency");
                break;

            case DialogUIManager.TextAnimationType.Bounce:
                DrawProperty("bounceIntensity");
                break;
        }

        // Debug bilgileri göster
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);

        DialogueTrigger trigger = (DialogueTrigger)target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugUsedPositive"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugUsedNegative"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugUsedDefault"));

        // Reset butonu ekle
        EditorGUILayout.Space();
        if (GUILayout.Button("Reset All Dialogues"))
        {
            if (EditorUtility.DisplayDialog("Reset Dialogues",
                "Are you sure you want to reset all dialogue sequences for this character?",
                "Reset", "Cancel"))
            {
                trigger.ResetDialogueSequence();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawProperty(string propertyName)
    {
        if (animationProperties.TryGetValue(propertyName, out SerializedProperty property))
        {
            EditorGUILayout.PropertyField(property);
        }
    }
}
#endif