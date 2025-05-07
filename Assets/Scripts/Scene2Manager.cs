using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene2Manager : MonoBehaviour
{
    public Button playGameButton;
    private AudioManager audioManager;
    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.Log("PlayerManager not found!");
        }

        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");

        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.Log("AudioManager not found!");
        }

        playGameButton.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (playerManager.shipCount == 0)
        {
            playGameButton.gameObject.SetActive(true);
        }
    }
    public void Btn_RotateShip()
    {
        audioManager.PlaySFX();
        playerManager.PlayerRotateShip();
        Debug.Log("Btn_RotateShip is click!");
    }
    public void Btn_BackToMenu()
    {
        audioManager.PlaySFX();
        DontDestroyObject[] obj = FindObjectsByType<DontDestroyObject>(FindObjectsSortMode.None);
        if (obj != null)
        {
            foreach (DontDestroyObject des in obj)
            {
                if (des != null && des.gameObject != null)
                {
                    Destroy(des.gameObject);
                    Debug.Log("DontDestroyObject destroyed!");
                }
            }
        }
        SceneManager.LoadSceneAsync(0);
        Debug.Log("Btn_BackToMenu is click!");
    }
    public void Btn_PlayGame()
    {
        audioManager.PlaySFX();
        SceneManager.LoadSceneAsync(2);
        Debug.Log("Btn_PlayGame is click!");
    }
}
