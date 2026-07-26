using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class as the name suggests is able to
/// 1. Load a scene by configuring its name in the inspector.
/// 2. Reload the current scene, which automatically gets the scene name.
/// </summary>

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public string currentSceneName;
    public static SceneLoader Instance { get; set; }

    public void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        print(currentSceneName);
        Time.timeScale = 1;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(currentSceneName);
    }
}