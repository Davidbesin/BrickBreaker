using UnityEngine;
using UnityEngine.Events;

public class SupesTouchPaddle : MonoBehaviour
{
    [SerializeField] private UnityEvent onPaddleTouched;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paddle"))
        {
               Debug.Log("Listener Count: " + onPaddleTouched.GetPersistentEventCount());
            onPaddleTouched?.Invoke();
            gameObject.SetActive(false);
            Debug.Log("shashs");
        }
         if (other.CompareTag("Failure"))
        {
            gameObject.SetActive(false);
        }
    }
}