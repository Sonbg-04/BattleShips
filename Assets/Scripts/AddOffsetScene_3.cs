using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddOffsetScene_3 : MonoBehaviour
{
    private bool isBattle = false;
    private void Update()
    {
        if (!isBattle)
        {
            AddOffsetToObject();
        }
    }
    private void AddOffsetToObject()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;
        if (sceneIndex == 2)
        {
            transform.position = new Vector3(transform.position.x + 2.3f, transform.position.y - 0.4f, transform.position.z);
            isBattle = true;
        }    
    }    
}
