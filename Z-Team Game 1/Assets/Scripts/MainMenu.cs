using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //method to start game after play button pressed
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    //quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
