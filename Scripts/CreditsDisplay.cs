using UnityEngine;
using TMPro; // Remove this line if you're not using TextMeshPro

public class CreditsDisplay : MonoBehaviour
{
    [Header("Assign ONE of these depending on what you're using")]
    [SerializeField] private TextMeshProUGUI tmpText;   // TextMeshPro - UGUI
    [SerializeField] private TextMesh textMesh;         // 3D TextMesh (world-space, non-UI)

    [Header("Credits Content")]
    [TextArea(6, 12)]
    [SerializeField] private string creditsText =
        "ART CREDITS\n\n" +
        "Rocket sprite\n" +
        "by JM.Atencia (OpenGameArt.org)\n" +
        "License: CC-BY 3.0\n" +
        "opengameart.org/content/rocket\n\n" +
        "Shooting Star PNG\n" +
        "by Animated Video (Vecteezy)\n" +
        "License: Free License (Attribution Required)\n" +
        "vecteezy.com";

    private void Start()
    {
        ApplyCredits();
    }

    // Call this manually too, e.g. from a "Credits" button
    public void ApplyCredits()
    {
        if (tmpText != null)
        {
            tmpText.text = creditsText;
        }
        else if (textMesh != null)
        {
            textMesh.text = creditsText;
        }
        else
        {
            Debug.LogWarning("CreditsDisplay: No TextMeshProUGUI or TextMesh assigned in the Inspector.");
        }
    }
}
