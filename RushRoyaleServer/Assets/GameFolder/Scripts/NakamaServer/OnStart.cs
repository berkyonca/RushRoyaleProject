using UnityEngine;
using UnityEngine.Events;

public class OnStart : MonoBehaviour
{
    public UnityEvent OnGameStart;

    private void Start()
    {
        OnGameStart?.Invoke();
    }
}
