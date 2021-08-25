using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using static GlobalVariables;

public class CameraManager : MonoBehaviour
{
	public GraphicsQuality CurrentQuality;
	public PostProcessingBehaviour behaviour;
	public PostProcessingProfile Ultra;
	public PostProcessingProfile High;
	public PostProcessingProfile Average;
	public PostProcessingProfile Low;
	void Awake()
	{
		CurrentQuality = GlobalVariables.GraphicQuality;
		behaviour.profile = BehaviourFromEnum(CurrentQuality);
	}
	public void ChangeCameraQuality(GraphicsQuality quality)
	{
		CurrentQuality = quality;
		behaviour.profile = BehaviourFromEnum(CurrentQuality);
	}
	public PostProcessingProfile BehaviourFromEnum(GraphicsQuality quality)
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
