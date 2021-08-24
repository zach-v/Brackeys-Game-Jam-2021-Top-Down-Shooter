using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
	public AudioMixerSnapshot normalVolume;
	public AudioMixerSnapshot lowPass;

	bool lowPassFilter = true;

    void Awake() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
			s.source.outputAudioMixerGroup = s.audioMixer;
		}
    }

	public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

	public void toggleLowPass() {
		if (lowPassFilter) {
			normalVolume.TransitionTo(5);
		} else {
			lowPass.TransitionTo(2);
		}
		lowPassFilter = !lowPassFilter;
	}
}