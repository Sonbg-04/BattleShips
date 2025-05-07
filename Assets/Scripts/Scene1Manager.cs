using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene1Manager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel, guidePanel;
    [SerializeField] private Button settingsButton, guideButton;
    private bool isSettingsPanelActive = false, isGuidePanelActive = false;
    private AudioManager audioManager;

    private void Start()
    {
        settingsPanel.SetActive(isSettingsPanelActive);
        guidePanel.SetActive(isGuidePanelActive);

        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");

        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.Log("AudioManager not found!");
        }

    }

    public void BtnStart()
    {
        audioManager.PlaySFX();
        SceneManager.LoadSceneAsync(1);
        Debug.Log("Game Started");
    }

    public void BtnSettings()
    {
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.name == "Settings")
        {
            guideButton.gameObject.SetActive(isSettingsPanelActive);
            audioManager.PlaySFX();
            isSettingsPanelActive = !isSettingsPanelActive;
            settingsPanel.SetActive(isSettingsPanelActive);
            Debug.Log(isSettingsPanelActive ? "Settings Panel Opened" : "Settings Panel Closed");
        }
    }

    public void BtnGuide()
    {
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.name == "Guide")
        {
            settingsButton.gameObject.SetActive(isGuidePanelActive);
            audioManager.PlaySFX();
            isGuidePanelActive = !isGuidePanelActive;
            guidePanel.SetActive(isGuidePanelActive);
            Debug.Log(isGuidePanelActive ? "Guide Panel Opened" : "Guide Panel Closed");
        }
    }
 
    public void BtnQuit()
    {
        audioManager.PlaySFX();
        Application.Quit();
        Debug.Log("Game Quit");
    }    
}
