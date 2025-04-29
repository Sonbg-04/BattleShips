using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundSource, sfxSource;

    private void Start()
    {
        soundSource.Play();
    }
    
    public void PlaySFX()
    {
        sfxSource.PlayOneShot(sfxSource.clip);
    }    
}
