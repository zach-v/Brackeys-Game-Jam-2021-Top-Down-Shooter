using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;
	public Sound[] ambientSounds;
	public AudioMixerSnapshot normalVolume;
	public AudioMixerSnapshot lowPass;
	public AudioMixerSnapshot reverb;
	[ReadOnly] public Sound CurrentMusic;
	public FilterState CurrentState = FilterState.Normal;
	public enum FilterState { Normal, LowPass, Reverb }

	void Awake()
	{
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();

			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.outputAudioMixerGroup = s.audioMixer;
		}
		CurrentMusic = Array.Find(sounds, sound => sound.isMusic);
	}

	public void Play(string name)
	{
		try
		{
			Sound s = Array.Find(sounds, sound => sound.name == name);
			if (s.isMusic)
			{
				Stop(CurrentMusic.name);
				CurrentMusic = s;
			}
			s.source.Play();
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to play sound: {name}, because {e.Message}");
		}
	}
	private IEnumerator FadeAudio(Sound sound)
	{
		for (float i = sound.volume; i >= 0; i -= 0.1f)
		{
			sound.source.volume = i;
			yield return new WaitForSeconds(0.1f);
		}
		sound.source.Stop();
	}
	public void Stop(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (!s.isMusic)
			s.source.Stop();
		else
			StartCoroutine("FadeAudio",s);
	}

	public void ChangeFilterState(FilterState state)
	{
		CurrentState = state;
		switch (state)
		{
			case FilterState.Normal:
				normalVolume.TransitionTo(5);
				break;
			case FilterState.LowPass:
				lowPass.TransitionTo(2);
				break;
			case FilterState.Reverb:
				reverb.TransitionTo(2);
				break;
		}
	}
}