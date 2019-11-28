using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    GameController gameController;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnReturnGameClick()
    {
        gameController.Pause(false);
    }

    public void OnReturnTitleClick()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OnQuitApp()
    {
        Quit();
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        UnityEngine.Application.Quit();
#else
        Debug.Log("Any other platform");
#endif
    }
}
