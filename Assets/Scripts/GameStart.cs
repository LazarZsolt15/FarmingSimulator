using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject restartPanel;
    public GameObject camera;
    public GameObject FPScontroller;
    public GameObject TPScontroller;

    private bool gameStarted = false;
    float elapsedTime = 0;
    float startTime;

void Update()
{
    if (gameStarted)
    {
        float elapsedTime = Time.time - startTime; 
       
        if (elapsedTime >= 300)
        {
            restartPanel.SetActive(true);
            camera.SetActive(true); 
            camera.transform.position = new Vector3(0,0,0);
            FPScontroller.SetActive(false);
            TPScontroller.SetActive(true);
            gameStarted = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

    public void FPSpressed(){
        pausePanel.SetActive(false);
        camera.SetActive(false);
        FPScontroller.SetActive(true);
        gameStarted = true;
        startTime = Time.time;
    }
      public void TPSpressed(){
        pausePanel.SetActive(false);
        TPScontroller.SetActive(true);
        gameStarted = true;
        startTime = Time.time;
    }

    public void RestartGame(){
        SceneManager.LoadScene("CountrySideDemo");
    }

}
