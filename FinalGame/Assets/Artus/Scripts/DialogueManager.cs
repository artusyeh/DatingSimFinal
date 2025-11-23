using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public Transform choicesContainer;      // Vertical Layout Group
    public Button choiceButtonPrefab;       // Prefab for a choice button

    [Header("Dialogue File")]
    public string dialogueFileName = "opening_scene"; 

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.02f;   // seconds per character

    [Header("Typewriter SFX")]
    public AudioSource typeSFXSource;
    public AudioClip typeSFXClip;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public int charsPerSound = 2;          // play sound every X characters

    private DialogueRoot dialogueRoot;
    private Dictionary<string, DialogueNode> nodeLookup;
    private DialogueNode currentNode;

    // click-to-continue state
    private bool waitingForClick = false;
    private string nextNodeOnClick = null;

    // typewriter state
    private bool isTyping = false;
    private string fullText = "";
    private Coroutine typingCoroutine;

    //timer inspector
    public TimerScript timerScript;

    void Start()
    {
        LoadDialogue();
        StartDialogue();
    }

    void Update()
    {
        // Left-click behavior
        if (Input.GetMouseButtonDown(0))
        {

            if (isTyping)
            {
                FinishTypingInstantly();
            }

            else if (waitingForClick && !string.IsNullOrEmpty(nextNodeOnClick))
            {
                waitingForClick = false;
                string target = nextNodeOnClick;
                nextNodeOnClick = null;
                GoToNode(target);
            }
        }
    }

    void LoadDialogue()
    {
        // JSON should be at: Assets/Dialogue/Resources/opening_scene.json
        TextAsset jsonAsset = Resources.Load<TextAsset>(dialogueFileName);
        if (jsonAsset == null)
        {
            Debug.LogError("Dialogue JSON not found at Resources/" + dialogueFileName);
            return;
        }

        dialogueRoot = JsonUtility.FromJson<DialogueRoot>(jsonAsset.text);
        if (dialogueRoot == null)
        {
            Debug.LogError("Failed to parse dialogue JSON.");
            return;
        }

        nodeLookup = new Dictionary<string, DialogueNode>();

        foreach (var node in dialogueRoot.nodes)
        {
            if (node == null)
            {
                Debug.LogWarning("Null node found in dialogueRoot.nodes");
                continue;
            }

            if (!nodeLookup.ContainsKey(node.id))
            {
                nodeLookup.Add(node.id, node);
            }
            else
            {
                Debug.LogWarning("Duplicate node id found: " + node.id);
            }
        }

        Debug.Log("Loaded dialogue. Nodes count: " + nodeLookup.Count);
    }

    void StartDialogue()
    {
        if (dialogueRoot == null)
        {
            Debug.LogError("DialogueRoot is null. Did JSON fail to load?");
            return;
        }

        GoToNode(dialogueRoot.startNode);
    }

    void GoToNode(string nodeId)
    {
        // Reset states
        waitingForClick = false;
        nextNodeOnClick = null;
        StopTypingIfNeeded();

        if (!nodeLookup.TryGetValue(nodeId, out currentNode))
        {
            Debug.LogError("No node with id: " + nodeId);
            return;
        }

        Debug.Log("GoToNode: " + nodeId);

        // Update speaker
        if (speakerText != null)
            speakerText.text = currentNode.speaker;

        // Clear choices for new node
        ClearChoices();

        // Start typewriter effect for this node's text
        StartTyping(currentNode.text);

         if(nodeId == "ending_good")
         {
          if (timerScript != null)
          timerScript.StopTimer();
         }
    }

    // TYPEWRITER LOGIC

    void StartTyping(string text)
    {
        StopTypingIfNeeded();

        fullText = text ?? "";
        if (bodyText != null)
            bodyText.text = "";

        typingCoroutine = StartCoroutine(TypeTextCoroutine(fullText));
    }

    IEnumerator TypeTextCoroutine(string textToType)
    {
        isTyping = true;

        for (int i = 0; i < textToType.Length; i++)
        {
            if (bodyText != null)
                bodyText.text += textToType[i];

            // Play sound every few characters
            if (charsPerSound > 0 && i % charsPerSound == 0)
            {
                PlayTypeSound();
            }

            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        typingCoroutine = null;

        OnTypingComplete();
    }

    void FinishTypingInstantly()
    {
        StopTypingIfNeeded();

        if (bodyText != null)
            bodyText.text = fullText;

        isTyping = false;
        OnTypingComplete();
    }

    void StopTypingIfNeeded()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    void PlayTypeSound()
    {
        if (typeSFXSource != null && typeSFXClip != null)
        {
            typeSFXSource.pitch = Random.Range(minPitch, maxPitch);
            typeSFXSource.PlayOneShot(typeSFXClip);
        }
    }

    void OnTypingComplete()
    {
        int choiceCount = (currentNode.choices == null) ? 0 : currentNode.choices.Length;
        Debug.Log("Typing complete. Node '" + currentNode.id + "' has " + choiceCount + " choices.");

        if (choiceCount > 0)
        {
            // Create choice buttons
            foreach (var choice in currentNode.choices)
            {
                CreateChoiceButton(choice);
            }
        }
        else
        {
            // No choices: enable click-to-continue if there is a next node
            if (!string.IsNullOrEmpty(currentNode.next))
            {
                waitingForClick = true;
                nextNodeOnClick = currentNode.next;
            }
            else
            {
                Debug.Log("Dialogue ended at node: " + currentNode.id);
            }
        }
    }


    // CHOICES UI


    void ClearChoices()
    {
        if (choicesContainer == null) return;

        for (int i = choicesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesContainer.GetChild(i).gameObject);
        }
    }

    void CreateChoiceButton(Choice choice)
    {
        if (choiceButtonPrefab == null)
        {
            Debug.LogError("ChoiceButtonPrefab is not assigned on DialogueManager.");
            return;
        }

        if (choicesContainer == null)
        {
            Debug.LogError("ChoicesContainer is not assigned on DialogueManager.");
            return;
        }

        Button btn = Instantiate(choiceButtonPrefab, choicesContainer);
        TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
        if (btnText != null)
        {
            btnText.text = choice.text;
        }

        btn.onClick.AddListener(() =>
        {
            //GoToNode(choice.next);
            HandleChoiceSelection(choice);
        });

        Debug.Log("Created choice button: " + choice.text + " > " + choice.next);
    }
    void HandleChoiceSelection(Choice choice)
    {
        if (!choice.isCorrect)
        {
            if (timerScript != null)
                timerScript.StartTimer();
        }

        GoToNode(choice.next);
        ClearChoices();

        if (choice.next == "ending_good")
        {
            if (timerScript != null)
                timerScript.StopTimer();
        }
    }
}


