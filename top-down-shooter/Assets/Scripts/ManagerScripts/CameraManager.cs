using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using static GlobalVariables;

public class CameraManager : MonoBehaviour
{
	public GraphicsQuality CurrentQuality;
	public Volume Volume;
	public VolumeProfile Ultra;
	public VolumeProfile High;
	public VolumeProfile Average;
	public VolumeProfile Low;
	void Awake()
	{
		CurrentQuality = GlobalVariables.GraphicQuality;
		try
		{
			Volume.profile = BehaviourFromEnum(CurrentQuality);
		}
		catch (Exception e)
		{
			Debug.Log($"Couldn't get post processing behaviour... {e.Message}");
		}
	}
	public void ChangeCameraQuality(GraphicsQuality quality)
	{
		CurrentQuality = quality;
		Volume.profile = BehaviourFromEnum(CurrentQuality);
	}
	public VolumeProfile BehaviourFromEnum(GraphicsQuality quality)
	{
		switch (quality)
		{
			case GraphicsQuality.Ultra:
				return Ultra;
			case GraphicsQuality.High:
				return High;
			case GraphicsQuality.Average:
				return Average;
			case GraphicsQuality.Low:
				return Low;
		}
		return Ultra;
	}
}
