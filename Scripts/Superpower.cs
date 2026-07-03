using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Superpower : MonoBehaviour
{
    public static Superpower Instance { get; private set; }

    [SerializeField] private GameObject failure;
    [SerializeField] private GameObject normal;
    [SerializeField] private Image timerBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Uncomment if you want this to persist across scene loads.
        // DontDestroyOnLoad(gameObject);
    }

    public void MultiBalls()
    {
        BallPaddleManager.Instance.LaunchMultipleBalls();
    }

    [ContextMenu("Wall Active")]
    public void WallSActive()
    {
        StartCoroutine(WallChange());
    }

    private IEnumerator WallChange()
    {
        failure.SetActive(false);
        normal.SetActive(true);

        float duration = 8f;
        float timer = duration;

        timerBar.gameObject.SetActive(true);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            timerBar.fillAmount = timer / duration;
            yield return null;
        }

        timerBar.fillAmount = 0f;
        timerBar.gameObject.SetActive(false);

        normal.SetActive(false);
        failure.SetActive(true);
    }
}