using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float delay = 0f;
    [SerializeField] private Scenes scene;

    public static SceneChanger Instance;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChangeScene();
        NakamaManager.Instance.OnLoginSuccess += () => ChangeScene(Scenes.Lobby, 0.5f);
        MultiplayerManager.Instance.onMatchJoin += () => ChangeScene(Scenes.Battle, 0.5f);
    }

    private void OnDestroy()
    {
        NakamaManager.Instance.OnLoginSuccess -= () => ChangeScene(Scenes.Lobby, 0.5f);
        MultiplayerManager.Instance.onMatchJoin -= () => ChangeScene(Scenes.Battle, 0.5f);
    }

    public void ChangeScene(Scenes scene, float delay)
    {
        this.scene = scene;
        this.delay = delay;
        StartCoroutine(ChangeSceneCoroutine());
    }

    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneCoroutine());
    }

    private IEnumerator ChangeSceneCoroutine()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene((int)scene);
    }
}
