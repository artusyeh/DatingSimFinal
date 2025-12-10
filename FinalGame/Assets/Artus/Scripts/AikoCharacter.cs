using System.Collections.Generic;
using UnityEngine;

public class AikoCharacter : MonoBehaviour
{
    [Header("Sprite Renderer")]
    public SpriteRenderer spriteRenderer;

    [Header("Expressions")]
    public Sprite aiko_sad;
    public Sprite aiko_angry;
    public Sprite aiko_blushing;
    public Sprite aiko_happy;

    public Sprite aiko_happy2;
    public Sprite aiko_neutral;
    public Sprite aiko_pout;
    public Sprite aiko_shocked;
    public Sprite aiko_smug;
    public Sprite aiko_neutral2;
    public Sprite aiko_invisible;
    public Sprite aiko_crazy;

    public Sprite aiko_evil;

    public AikoBounce bounce;

    private Dictionary<string, Sprite> expressionMap;

    private void Awake()
    {
        expressionMap = new Dictionary<string, Sprite>
        {
            { "sad", aiko_sad },
            { "angry", aiko_angry },
            { "blushing", aiko_blushing },
            { "happy", aiko_happy },
            { "neutral", aiko_neutral },
            { "pout", aiko_pout },
            { "shocked", aiko_shocked },
            { "smug", aiko_smug },
            { "happy2", aiko_happy2 },
            { "neutral2", aiko_neutral2 },
            { "invisible", aiko_invisible },
            { "crazy", aiko_crazy },
            { "evil", aiko_evil }
        };
    }

    // Called by DialogueManager when an Aiko node is shown
    public void SetExpression(string expression)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("AikoCharacter has no SpriteRenderer assigned.");
            return;
        }

        if (string.IsNullOrEmpty(expression))
        {
            expression = "neutral2";
        }

        expression = expression.ToLowerInvariant();

        if (expressionMap.TryGetValue(expression, out var sprite) && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            // fallback to neutral if unknown/faulty
            if (aiko_neutral != null)
                spriteRenderer.sprite = aiko_neutral2;
        }

        // If expression is "happy", trigger bounce
        if (expression == "happy" && bounce != null)
        {
            bounce.PlayBounce();
        }
    }
}