using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddOffset : MonoBehaviour
{
    private void Start()
    {
        AddOffsetToObject();
    }

    private void AddOffsetToObject()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;
        if (sceneIndex == 1)
        {
            transform.position = new Vector3(transform.position.x - 5f, transform.position.y - 2.6f, transform.position.z);
        }    
    }    
}
