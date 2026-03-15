using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [Header ("Sprite Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer hairRenderer;
    public SpriteRenderer clothingRenderer;

    [Header ("Skin Tone Sprites")]
    public Sprite[] skinToneSprites;

    [Header ("Hair Colour Sprites")]
    public Sprite[] hairColorSprites;

    [Header ("Clothing Sprites")]
    public Sprite[] clothingSprites;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        ApplyAppearance ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void ApplyAppearance ()
    {
        if (GameState.Instance == null)
        {
            Debug.LogWarning ("GameState not found — using default appearance.");
            return;
        }

        // Apply skin tone
        int skinIndex = GameState.Instance.skinToneIndex;
        if (skinToneSprites != null && skinIndex < skinToneSprites.Length)
            bodyRenderer.sprite = skinToneSprites[skinIndex];

        // Apply hair colour
        int hairIndex = GameState.Instance.hairColorIndex;
        if (hairColorSprites != null && hairIndex < hairColorSprites.Length)
            hairRenderer.sprite = hairColorSprites[hairIndex];

        // Apply clothing based on profession
        int professionIndex = (int) GameState.Instance.currentProfession;
        if (clothingSprites != null && professionIndex < clothingSprites.Length)
            clothingRenderer.sprite = clothingSprites[professionIndex];

        Debug.Log ($"Appearance applied — Skin: {skinIndex}, Hair: {hairIndex}, Profession: {GameState.Instance.currentProfession}");
    }
}