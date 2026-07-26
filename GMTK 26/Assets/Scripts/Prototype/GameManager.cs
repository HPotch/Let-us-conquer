using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("References")]
    public Transform LevelCheckers;
    public Elevator LeftElevator;
    public Elevator RightElevator;
    [SerializeField] private RawImage skipImage;

    [Header("Settings")]
    [SerializeField] private float fastSpeed = 2f;
    
    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Time.timeScale = Mathf.Lerp(
            Time.timeScale,
            Keyboard.current.spaceKey.isPressed ? fastSpeed : 1f,
            Mathf.Clamp01(5f * Time.deltaTime));
        skipImage.color = new Color(1, 1, 1, (Time.timeScale - 1f) / (fastSpeed - 1f));
    }
}
