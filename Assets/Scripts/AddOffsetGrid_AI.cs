using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddOffsetGrid_AI : MonoBehaviour
{
    private void Start()
    {
        AddOffsetToObject();
    }
    private void AddOffsetToObject()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;
        if (sceneIndex == 2)
        {
            transform.position = new Vector3(transform.position.x - 2.72f, transform.position.y - 3f, transform.position.z);
        }
    }
}
