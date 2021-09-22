using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
	public static int seed = Random.state.GetHashCode();
	public enum GraphicsQuality { Low, Average, High, Ultra }
	public static GraphicsQuality GraphicQuality = GraphicsQuality.Ultra;
	public static bool debugOn = false;
}
