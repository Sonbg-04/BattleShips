using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene3Manager : MonoBehaviour
{
    public GameObject winPanel, losePanel;
    public Image yourTurnImg, enemyTurnImg;
    private AudioManager audioManager;

    private void Start()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");

        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.Log("AudioManager not found!");
        }

        winPanel.SetActive(false);
        losePanel.SetActive(false);

    }
    
    public void Btn_OK()
    {
        audioManager.PlaySFX();
        SceneManager.LoadSceneAsync(0);
    }
}
