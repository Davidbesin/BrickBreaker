using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input")]
    [SerializeField] private float tapTime = 0.2f;

    public event Action OnTap;
    public event Action<Vector2> OnPointerDrag;
    public event Action<float> OnKeyboardMove;
    public event Action OnDragEnd;

    private bool isPressed;
    private bool isDragging;
    private float pressTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsPlaying)
        {
            isPressed = false;
            isDragging = false;
            return;
        }

        // ---------------- Keyboard ----------------

        if (Keyboard.current != null)
        {
            float horizontal = 0f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontal = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontal = 1f;

            OnKeyboardMove?.Invoke(horizontal);

            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OnTap?.Invoke();
            }
        }

        // ---------------- Pointer ----------------

        if (Pointer.current == null)
            return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            isPressed = true;
            isDragging = false;
            pressTime = Time.time;
        }

        if (isPressed && Pointer.current.press.isPressed)
        {
            if (Time.time - pressTime >= tapTime)
            {
                isDragging = true;
                OnPointerDrag?.Invoke(Pointer.current.position.ReadValue());
            }
        }

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            if (!isDragging)
            {
                OnTap?.Invoke();
            }
            else
            {
                OnDragEnd?.Invoke();
            }

            isPressed = false;
            isDragging = false;
        }
    }
}