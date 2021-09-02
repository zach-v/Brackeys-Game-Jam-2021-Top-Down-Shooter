using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using static Sound;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	public Sound[] itemSounds;
	public Sound[] natureSounds;
	public Sound[] music;
	public Sound[] walkingSounds;
	public Sound[] mobSounds;
	public AudioMixerSnapshot normalVolume;
	public AudioMixerSnapshot lowPass;
	public AudioMixerSnapshot reverb;
	[ReadOnly] public Sound CurrentMusic;
	public FilterState CurrentState = FilterState.Normal;
	public enum FilterState { Normal, LowPass, Reverb }
	void Awake()
	{
		// Add all arrays to a single list
		List<Sound> all = new List<Sound>(itemSounds);
		all.AddRange(natureSounds);
		all.AddRange(music);
		all.AddRange(walkingSounds);
		all.AddRange(mobSounds);
		// Iterate through list to assign components
		foreach (Sound s in all)
		{
			s.source = gameObject.AddComponent<AudioSource>();

			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.outputAudioMixerGroup = s.audioMixer;
		}
		// Set current music
		CurrentMusic = Array.Find(music, sound => sound.type == SoundType.Music);
	}
	/// <summary>
	/// Plays a sound by name. Faster processing wise to give it a type too.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public void Play(string name, SoundType type = SoundType.Default, float pitchVariation = 0)
	{
		try
		{
			Sound s = Find(name, type);
			if (s.type == SoundType.Music)
			{
				Stop(CurrentMusic.name);
				CurrentMusic = s;
			}
			s.source.pitch = UnityEngine.Random.Range(-pitchVariation + s.pitch, pitchVariation + s.pitch);
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
	/// <summary>
	/// Play a sound by name and type at a certain point.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="point"></param>
	/// <param name="type"></param>
	public void PlayAtPoint(string name, Vector3 point, SoundType type = SoundType.Default)
	{
		try
		{
			Sound s = Find(name, type);
			if (s.type != SoundType.Music)
			{
				AudioSource.PlayClipAtPoint(s.clip, point);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to play sound: {name}, at point {point}, because {e.Message}");
		}
	}
	public void Stop(string name, SoundType type = SoundType.Default)
	{
		try
		{
			Sound s = Find(name, type);
			if (type == SoundType.Music)
				StartCoroutine(FadeAudio(s));
			else
				s.source.Stop();
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to stop sound: {name}, because {e.Message}");
		}
	}
	/// <summary>
	/// Internal finder method based on type. If default type given, it will make a list of all sounds.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	private Sound Find(string name, SoundType type = SoundType.Default)
	{
		try
		{
			Sound s = null;
			switch (type)
			{
				case SoundType.Mob:
					s = Array.Find(mobSounds, sound => sound.name == name);
					break;
				case SoundType.Item:
					s = Array.Find(itemSounds, sound => sound.name == name);
					break;
				case SoundType.Walking:
					s = Array.Find(walkingSounds, sound => sound.name == name);
					break;
				case SoundType.Music:
					s = Array.Find(music, sound => sound.name == name);
					break;
				case SoundType.Ambient:
					s = Array.Find(natureSounds, sound => sound.name == name);
					break;
				default:
					List<Sound> all = new List<Sound>(itemSounds);
					all.AddRange(natureSounds);
					all.AddRange(music);
					s = Array.Find(all.ToArray(), sound => sound.name == name);
					break;
			}
			return s;
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to find sound: {name}, because {e.Message}");
		}
		return null;
	}
	public void ChangeFilterState(FilterState state)
	{
		CurrentState = state;
		switch (state)
		{
			case FilterState.Normal:
				normalVolume.TransitionTo(1);
				break;
			case FilterState.LowPass:
				lowPass.TransitionTo(0.1f);
				break;
			case FilterState.Reverb:
				reverb.TransitionTo(1);
				break;
		}
	}
}