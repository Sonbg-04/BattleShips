using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundSource, sfxSource, victorySource, defeatSource;
    private static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        soundSource.Play();
    }
    
    public void PlaySFX()
    {
        sfxSource.PlayOneShot(sfxSource.clip);
    }
    public void PlayVictory()
    {
        victorySource.PlayOneShot(victorySource.clip);
    }
    public void PlayDefeat()
    {
        defeatSource.PlayOneShot(defeatSource.clip);
    }
}
