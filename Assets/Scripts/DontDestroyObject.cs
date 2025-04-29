using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");
        if (objs == null)
        {
            Debug.Log("DontDestroyObject not found!");
        }
        else
        {
            if (objs.Length > 1)
            {
                Destroy(gameObject);
                Debug.Log("Duplicate DontDestroyObject destroyed!");
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Debug.Log("DontDestroyObject set to not destroy on load!");
            }
        }    
    }
}
