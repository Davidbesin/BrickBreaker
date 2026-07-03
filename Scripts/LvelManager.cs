using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private BrickBundle brickPrefab;
    [SerializeField] private Transform levelOrigin;
    [SerializeField] private int currentLevel;

    [Header("Layout")]
    [SerializeField] private float spacingX = 1.1f;
    [SerializeField] private float spacingY = 0.6f;

    private readonly List<Brick> bricks = new();             // tracks live bricks for win-check
    private readonly List<BrickBundle> brickBundles = new();  // tracks bundles for cleanup on restart/change

    private readonly Level level1 = new Level1();
    private readonly Level level2 = new Level2();
    private readonly Level level3 = new Level3();
    private readonly Level level4 = new Level4();
    private readonly Level level5 = new Level5();
    private readonly Level level6 = new Level6();
    private readonly Level level7 = new Level7();
    private readonly Level level8 = new Level8();
    private readonly Level level9 = new Level9();
    private readonly Level level10 = new Level10();

    private const int MaxLevel = 10;

    public Level CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ChangeLevel(GetLevel(currentLevel));
    }

    // Clears whatever is currently loaded and builds the given level
    public void ChangeLevel(Level newLevel)
    {
        ClearLevel();

        CurrentLevel = newLevel;

        InitializeLevel();
    }

    // Rebuilds the CURRENT level from scratch (e.g. after losing all lives)
    public void Restart()
    {
        currentLevel = 1;

        if (BallPaddleManager.Instance != null)
        {
            BallPaddleManager.Instance.currentState = BallPaddleManager.BallState.onPaddle;
        }

        ChangeLevel(GetLevel(currentLevel));
    }

    // Advances to the next level index and builds it
    public void NextLevel()
    {
        currentLevel++;

        if (currentLevel > MaxLevel)
        {
            // No more levels — treat as a full game clear instead of wrapping/crashing
            GameStateManager.Instance?.SetState(GameStateManager.GameState.Menu);
            return;
        }

        if (BallPaddleManager.Instance != null)
        {
            BallPaddleManager.Instance.currentState = BallPaddleManager.BallState.onPaddle;
        }

        ChangeLevel(GetLevel(currentLevel));
    }

    // Destroys all currently-tracked brick bundles and unsubscribes their events
    private void ClearLevel()
    {
        foreach (Brick brick in bricks)
        {
            if (brick != null)
                brick.OnBrickDestroyed -= HandleBrickDestroyed;
        }

        foreach (BrickBundle bundle in brickBundles)
        {
            if (bundle != null)
                Destroy(bundle.gameObject);
        }

        bricks.Clear();
        brickBundles.Clear();
    }

    // Instantiates bricks according to CurrentLevel's layout grid
    private void InitializeLevel()
    {
        int[,] layout = CurrentLevel.Layout;

        for (int y = 0; y < layout.GetLength(0); y++)
        {
            for (int x = 0; x < layout.GetLength(1); x++)
            {
                int hits = layout[y, x];

                if (hits == 0)
                    continue; // 0 means "no brick at this grid cell"

                Vector3 position = levelOrigin.position +
                    new Vector3(x * spacingX, -y * spacingY, 0);

                BrickBundle brickbundle = Instantiate(brickPrefab, position, Quaternion.identity);

                brickbundle.brick.Hits = hits;

                bricks.Add(brickbundle.brick);
                brickBundles.Add(brickbundle);

                brickbundle.brick.OnBrickDestroyed += HandleBrickDestroyed;
            }
        }
    }

    // Called whenever any brick reaches 0 hits and is destroyed
    private void HandleBrickDestroyed(Brick brick)
    {
        bricks.Remove(brick);
        brick.OnBrickDestroyed -= HandleBrickDestroyed;

        // Win condition — all bricks cleared
        if (bricks.Count == 0)
        {
            // TODO: confirm — currently goes to Menu. Change to NextLevel() instead
            // if clearing a level should auto-advance rather than return to menu.

            GameStateManager.Instance?.SetState(GameStateManager.GameState.Menu);
        }
    }

    private Level GetLevel(int level)
    {
        switch (level)
        {
            case 1: return level1;
            case 2: return level2;
            case 3: return level3;
            case 4: return level4;
            case 5: return level5;
            case 6: return level6;
            case 7: return level7;
            case 8: return level8;
            case 9: return level9;
            case 10: return level10;
            default: return level1; // fallback for out-of-range values
        }
    }
}