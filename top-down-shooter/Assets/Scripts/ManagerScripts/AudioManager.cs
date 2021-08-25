using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;
	public AudioMixerSnapshot normalVolume;
	public AudioMixerSnapshot lowPass;
	public AudioMixerSnapshot reverb;
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
	}

	public void Play(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.Play();
	}

	public void Stop(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.Stop();
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