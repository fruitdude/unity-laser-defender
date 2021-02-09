using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour{
    [SerializeField] int delayInSeconds = 4;

    public void LoadStartMenu() {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel() {
        SceneManager.LoadScene(1);
        FindObjectOfType<GameSession>().ResetGame();
    }

    public void LoadGameOver() {
        StartCoroutine(DelayLoading());
    }

    IEnumerator DelayLoading() {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(2);
    }


    public void QuitApplication() {
        Application.Quit();
    }
}
