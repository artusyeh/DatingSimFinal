using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;

    [Header("Dialogue Box Sprites")]
    [SerializeField] Image dialogueBoxImage;
    [SerializeField] Sprite mcBoxSprite;
    [SerializeField] Sprite aikoBoxSprite;

    [Header("Dialogue File")]
    public string dialogueFileName = "opening_scene";

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.02f;

    [Header("Typewriter SFX")]
    public AudioSource typeSFXSource;
    public AudioClip typeSFXClip;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public int charsPerSound = 2;

    [Header("Timer")]
    public TimerScript timerScript;
    [SerializeField] ZoomIn zoomInScript;

    [Header("Aiko Character")]
    public AikoCharacter aikoCharacter;

    [Header("Screen Shake")]
    public TestScreenShake shaker;

    // ---------------------------------------------------
    //                    ENDING SUPPORT
    // ---------------------------------------------------
    [Header("Ending Scenes")]
    public string goodEndingSceneName;
    public string badEndingSceneName;

    // ---------------------------------------------------
    //                    JUMPSCARE SYSTEM
    // ---------------------------------------------------
    [Header("Video Jumpscare")]
    public GameObject jumpscareVideoObject;
    [Tooltip("Objects to hide during jumpscares (video 1 and video 2).")]
    public GameObject[] objectsToHide;
    public float jumpscareDuration = 2f;

    [Header("Jumpscare 2 (After Creepy Aiko)")]
    public GameObject jumpscare2VideoObject;
    public float jumpscare2Duration = 2f;

    // ---------------------------------------------------
    //               CREEPY AIKO JUMPSCARE
    // ---------------------------------------------------
    [Header("Creepy Aiko Jumpscare")]
    public GameObject creepyAiko;
    public float creepySlideDistance = 6f;
    public float creepySlideSpeed = 6f;
    public float creepyHoldTime = 1.2f;

    public AudioSource creepyAikoSFXSource;
    public AudioClip creepyAikoSFXClip;

    // ---------------------------------------------------
    //                        BLACKOUT
    // ---------------------------------------------------
    [Header("Blackout")]
    public GameObject blackoutObject;
    public AudioSource blackoutSFXSource;
    public AudioClip blackoutSFXClip;

    [Header("BGM + Transition")]
    public AudioSource bgmSource;

    private DialogueRoot dialogueRoot;
    private Dictionary<string, DialogueNode> nodeLookup;
    private DialogueNode currentNode;

    private bool waitingForClick = false;
    private string nextNodeOnClick = null;

    private bool isTyping = false;
    private string fullText = "";
    private Coroutine typingCoroutine;

    void Start()
    {
        if (jumpscareVideoObject != null) jumpscareVideoObject.SetActive(false);
        if (jumpscare2VideoObject != null) jumpscare2VideoObject.SetActive(false);
        if (blackoutObject != null) blackoutObject.SetActive(false);
        if (creepyAiko != null) creepyAiko.SetActive(false);

        LoadDialogue();
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                FinishTypingInstantly();
            }
            else if (waitingForClick && !string.IsNullOrEmpty(nextNodeOnClick))
            {
                string target = nextNodeOnClick;
                waitingForClick = false;
                nextNodeOnClick = null;

                GoToNode(target);
            }
        }
    }

    void LoadDialogue()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(dialogueFileName);
        dialogueRoot = JsonUtility.FromJson<DialogueRoot>(jsonAsset.text);

        nodeLookup = new Dictionary<string, DialogueNode>();
        foreach (DialogueNode n in dialogueRoot.nodes)
            nodeLookup[n.id] = n;
    }

    void StartDialogue()
    {
        GoToNode(dialogueRoot.startNode);
    }

    // ---------------------------------------------------------
    //                     MAIN NODE HANDLER
    // ---------------------------------------------------------
    void GoToNode(string nodeId)
    {
        StopTypingIfNeeded();
        waitingForClick = false;
        nextNodeOnClick = null;

        if (!nodeLookup.TryGetValue(nodeId, out currentNode))
            return;

        // ENDINGS
        if (!string.IsNullOrEmpty(currentNode.endType))
        {
            HandleEnding(currentNode.endType);
            return;
        }

        // BLACKOUT
        HandleBlackout(currentNode);

        // TIMER CONTROL
        if (!string.IsNullOrEmpty(currentNode.speaker) &&
            currentNode.speaker.StartsWith("TimerControl"))
        {
            HandleTimerControlNode(currentNode);
            if (!string.IsNullOrEmpty(currentNode.next))
                GoToNode(currentNode.next);
            return;
        }

        // ZOOM
        if (!string.IsNullOrEmpty(currentNode.zoomType))
            HandleZoomNode(currentNode);

        // CREEPY AIKO JUMPSCARE
        if (currentNode.creepyAikoJumpscare)
        {
            HandleCreepyAikoJumpscare(currentNode);
            return;
        }

        // NORMAL VIDEO JUMPSCARE
        if (currentNode.jumpscare)
        {
            HandleJumpscareNode(currentNode);
            return;
        }

        // SHAKE
        HandleShake(currentNode);

        // Expression
        if (aikoCharacter != null)
            aikoCharacter.SetExpression(currentNode.expression);

        // Speaker box
        speakerText.text = currentNode.speaker;
        dialogueBoxImage.sprite =
            (currentNode.speaker == "Aiko") ? aikoBoxSprite : mcBoxSprite;

        ClearChoices();
        StartTyping(currentNode.text);
    }

    // ---------------------------------------------------------
    //                      ENDING SYSTEM
    // ---------------------------------------------------------
    void HandleEnding(string type)
    {
        if (type == "good" && !string.IsNullOrEmpty(goodEndingSceneName))
        {
            SceneManager.LoadScene(goodEndingSceneName);
            return;
        }

        if (type == "bad" && !string.IsNullOrEmpty(badEndingSceneName))
        {
            SceneManager.LoadScene(badEndingSceneName);
            return;
        }

        Debug.LogWarning("ENDING TYPE SET BUT NO SCENE ASSIGNED!");
    }

    // ---------------------------------------------------------
    //               CREEPY AIKO SLIDE-UP JUMPSCARE
    // ---------------------------------------------------------
    void HandleCreepyAikoJumpscare(DialogueNode node)
    {
        waitingForClick = false;

        if (creepyAikoSFXSource && creepyAikoSFXClip)
            creepyAikoSFXSource.PlayOneShot(creepyAikoSFXClip);

        if (creepyAiko != null)
        {
            creepyAiko.SetActive(true);
            StartCoroutine(CreepyAikoRoutine(node));
        }
        else
        {
            // fallback if object missing
            GoToNode(node.next);
        }
    }

    IEnumerator CreepyAikoRoutine(DialogueNode node)
    {
        Vector3 start = creepyAiko.transform.localPosition;
        Vector3 end = start + new Vector3(0, creepySlideDistance, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * creepySlideSpeed;
            creepyAiko.transform.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        yield return new WaitForSeconds(creepyHoldTime);

        creepyAiko.transform.localPosition = start;
        creepyAiko.SetActive(false);

        // Trigger Jumpscare 2 if it exists
        if (jumpscare2VideoObject != null)
        {
            StartCoroutine(PlayJumpscare2ThenContinue(node));
            yield break;
        }

        GoToNode(node.next);
    }

    IEnumerator PlayJumpscare2ThenContinue(DialogueNode node)
    {
        // Hide UI / objects while jumpscare2 plays
        if (objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
                if (obj != null) obj.SetActive(false);
        }

        jumpscare2VideoObject.SetActive(true);

        yield return new WaitForSeconds(jumpscare2Duration);

        jumpscare2VideoObject.SetActive(false);

        // Restore hidden objects
        if (objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
                if (obj != null) obj.SetActive(true);
        }

        GoToNode(node.next);
    }

    // ---------------------------------------------------------
    //                       BLACKOUT
    // ---------------------------------------------------------
    void HandleBlackout(DialogueNode node)
    {
        if (blackoutObject == null) return;

        if (node.blackout)
        {
            blackoutObject.SetActive(true);
            bgmSource?.Stop();

            if (blackoutSFXSource && blackoutSFXClip)
                blackoutSFXSource.PlayOneShot(blackoutSFXClip);
        }
        else
        {
            blackoutObject.SetActive(false);
        }
    }

    // ---------------------------------------------------------
    //                NORMAL VIDEO JUMPSCARE
    // ---------------------------------------------------------
    void HandleJumpscareNode(DialogueNode node)
    {
        if (objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
                if (obj != null) obj.SetActive(false);
        }

        jumpscareVideoObject.SetActive(true);

        StartCoroutine(JumpscareRoutine(node));
    }

    IEnumerator JumpscareRoutine(DialogueNode node)
    {
        yield return new WaitForSeconds(jumpscareDuration);

        jumpscareVideoObject.SetActive(false);

        if (objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
                if (obj != null) obj.SetActive(true);
        }

        GoToNode(node.next);
    }

    // ---------------------------------------------------------
    //                    SHAKE / TIMER / ZOOM
    // ---------------------------------------------------------
    void HandleShake(DialogueNode node)
    {
        if (shaker == null || string.IsNullOrEmpty(node.shake)) return;

        if (node.shake == "good") shaker.ShakeGood();
        if (node.shake == "bad") shaker.ShakeBad();
    }

    void HandleTimerControlNode(DialogueNode node)
    {
        if (timerScript == null) return;

        if (node.speaker == "TimerControl:Stop")
            timerScript.ResetTimer();
        else if (node.speaker == "TimerControl:Continue")
            timerScript.StartTimer(false);
    }

    void HandleZoomNode(DialogueNode node)
    {
        if (zoomInScript == null) return;
        // Put zoom logic here if needed later
    }

    // ---------------------------------------------------------
    //                 TYPEWRITER + CHOICES
    // ---------------------------------------------------------
    void StartTyping(string text)
    {
        StopTypingIfNeeded();
        fullText = text;
        bodyText.text = "";
        typingCoroutine = StartCoroutine(TypeTextCoroutine(text));
    }

    IEnumerator TypeTextCoroutine(string text)
    {
        isTyping = true;

        for (int i = 0; i < text.Length; i++)
        {
            bodyText.text += text[i];

            if (i % charsPerSound == 0)
                PlayTypeSound();

            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        OnTypingComplete();
    }

    void FinishTypingInstantly()
    {
        StopTypingIfNeeded();
        bodyText.text = fullText;
        isTyping = false;
        OnTypingComplete();
    }

    void StopTypingIfNeeded()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
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
        if (currentNode.choices != null && currentNode.choices.Length > 0)
        {
            foreach (var c in currentNode.choices)
                CreateChoiceButton(c);
        }
        else
        {
            waitingForClick = true;
            nextNodeOnClick = currentNode.next;
        }
    }

    void ClearChoices()
    {
        foreach (Transform c in choicesContainer)
            Destroy(c.gameObject);
    }

    void CreateChoiceButton(Choice choice)
    {
        Button btn = Instantiate(choiceButtonPrefab, choicesContainer);
        btn.GetComponentInChildren<TMP_Text>().text = choice.text;

        btn.onClick.AddListener(() => HandleChoiceSelection(choice));
    }

    void HandleChoiceSelection(Choice choice)
    {
        if (!choice.isCorrect)
            timerScript?.PlayHeartbreakFX();

        ClearChoices();
        GoToNode(choice.next);
    }
}
