using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public enum SoundType { Default, Mob, Walking, Weather, Music, Item, Wind, Ambient, Animal }
	public string name;
	public AudioClip clip;

	[Range(0, 1)]
	public float volume = 0.8f;
	[Range(0.1f, 3)]
	public float pitch = 1;
	public bool loop = false;
	public AudioMixerGroup audioMixer;
	public SoundType type;
	[HideInInspector]
	public AudioSource source;
}