using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum SoundEffect
{
	TaskComplete
}

public class AudioManager : MonoBehaviour 
{
	public static AudioManager Instance = null;
	private AudioSource[] effectSources;

	[SerializeField]
	private float sfxVolume = 0.8f;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		
		Instance = this;

		effectSources = new AudioSource[6];
		for (int i = 0; i < effectSources.Length; i++)
		{
			effectSources[i] = gameObject.AddComponent<AudioSource>();
		}
	}
	
	public void PlaySound(SoundEffect sound)
	{
		AudioClip clip = null;

		switch (sound)
		{
			case SoundEffect.TaskComplete:
				clip = Resources.Load("task_complete") as AudioClip;
				break;
		}

		if (clip == null)
		{
			return;
		}

		var source = effectSources[0];
		
		for (int i = 0; i < effectSources.Length && source.isPlaying; i++)
		{
			source = effectSources[i];
		}
		if (source.isPlaying && source.time < 0.15f)
		{
			return;
		}
		
		source.Stop();
		source.pitch = Random.Range(0.95f, 1.05f);
		source.clip = clip;
		source.volume = sfxVolume;
		source.Play();

	}

}