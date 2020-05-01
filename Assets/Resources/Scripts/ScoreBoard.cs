using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(AudioSource))]
public class ScoreBoard : MonoBehaviour {
    public AudioClip notification;
    private AudioSource audioSource;
    private Text text;

	// Use this for initialization
	public void Start () {
        audioSource = GetComponent<AudioSource>();
        text = GetComponent<Text>();
	}


	
	// Update is called once per frame
	public void UpdateBoard (bool silent = false) {
        if (!silent)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(notification);
            }
        }
        text.text = GameManager.Instance.PlayerScore + " : " + GameManager.Instance.EnemyScore;
	}
}
