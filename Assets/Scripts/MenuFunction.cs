using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunction : MonoBehaviour
{
    public void Retry(){
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Quit(){
        Application.Quit();
    }

    public void Resume(){
        Time.timeScale = 1;
    }
}
