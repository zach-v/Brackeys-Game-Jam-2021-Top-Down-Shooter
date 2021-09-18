using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using static Sound;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	public Sound[] itemSounds;
	public Sound[] weatherSounds;
	public Sound[] ambientSounds;
	public Sound[] animalSounds;
	public Sound[] music;
	public Sound[] walkingSounds;
	public Sound[] mobSounds;
	[SerializeField] private AudioMixerSnapshot normalVolume;
	[SerializeField] private AudioMixerSnapshot lowPass;
	[SerializeField] private AudioMixerSnapshot reverb;
	[SerializeField] private float musicFadeSpeed = 0.1f;
	[SerializeField] private float ambienceFadeSpeed = 0.2f;
	[SerializeField] private FilterState CurrentState = FilterState.Normal;
	public enum FilterState { Normal, LowPass, Reverb }
	void Awake()
	{
		// Add all arrays to a single list
		List<Sound> all = new List<Sound>(itemSounds);
		all.AddRange(weatherSounds);
		all.AddRange(ambientSounds);
		all.AddRange(animalSounds);
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
	}
	/// <summary>
	/// Plays a sound by name. Faster processing wise to give it a type too.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public Sound Play(string name, SoundType type = SoundType.Default, float pitchVariation = 0, bool oneShot = false)
	{
		Sound s = Find(name, type);
		return Play(s, pitchVariation, oneShot);
	}
	public Sound Play(Sound s, float pitchVariation = 0, bool oneShot = false)
	{
		try
		{
			// Handle pitch variation
			s.source.pitch = UnityEngine.Random.Range(-pitchVariation + s.pitch, pitchVariation + s.pitch);
			// Toggle playing for non-overlapping sound, or overlapping
			if (oneShot)
				s.source.PlayOneShot(s.clip);
			else
				s.source.Play();
			return s;
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to play sound: {name}, because {e.Message}");
		}
		return null;
	}
	private IEnumerator FadeAudio(Sound sound)
	{
		for (float i = sound.volume; i >= 0; i -= 0.1f)
		{
			sound.source.volume = i;
			switch (sound.type)
			{
				case SoundType.Music:
					yield return new WaitForSeconds(musicFadeSpeed);
					break;
				case SoundType.Ambient:
					yield return new WaitForSeconds(ambienceFadeSpeed);
					break;
			}

		}
		sound.source.Stop();
		sound.source.volume = sound.volume;
	}
	/// <summary>
	/// Play a sound by name and type at a certain point.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="position"></param>
	/// <param name="type"></param>
	public void PlayAtPosition(string name, Vector3 position, SoundType type = SoundType.Default)
	{
		try
		{
			Sound s = Find(name, type);
			if (s.type != SoundType.Music)
			{
				AudioSource.PlayClipAtPoint(s.clip, position);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to play sound: {name}, at point {position}, because {e.Message}");
		}
	}
	/// <summary>
	/// Stops a sound directly at the source. Only use if you have the exact sound...
	/// </summary>
	/// <param name="s"></param>
	public void Stop(Sound s)
	{
		try
		{
			switch (s.type)
			{
				case SoundType.Music:
				case SoundType.Ambient:
					StartCoroutine(FadeAudio(s));
					break;
				default:
					s.source.Stop();
					break;
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Unable to stop sound: {s.name}, because {e.Message}");
		}
	}
	/// <summary>
	/// Stops a sound by looking it up through the entire sound directory, or by type.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	public void Stop(string name, SoundType type = SoundType.Default)
	{
		Sound s = Find(name, type);
		Stop(s);
	}
	/// <summary>
	/// Internal finder method based on type. If default type given, it will make a list of all sounds.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public Sound Find(string name, SoundType type = SoundType.Default)
	{
		try
		{
			Sound s = null;
			switch (type)
			{
				case SoundType.Mob:
					s = Array.Find(mobSounds, sound => sound.name == name);
					goto default;
				case SoundType.Item:
					s = Array.Find(itemSounds, sound => sound.name == name);
					goto default;
				case SoundType.Walking:
					s = Array.Find(walkingSounds, sound => sound.name == name);
					goto default;
				case SoundType.Music:
					s = Array.Find(music, sound => sound.name == name);
					goto default;
				case SoundType.Weather | SoundType.Wind:
					s = Array.Find(weatherSounds, sound => sound.name == name);
					goto default;
				case SoundType.Animal:
					s = Array.Find(animalSounds, sound => sound.name == name);
					goto default;
				default:
					if (s != null)
						break;
					List<Sound> all = new List<Sound>(itemSounds);
					all.AddRange(weatherSounds);
					all.AddRange(ambientSounds);
					all.AddRange(animalSounds);
					all.AddRange(walkingSounds);
					all.AddRange(mobSounds);
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