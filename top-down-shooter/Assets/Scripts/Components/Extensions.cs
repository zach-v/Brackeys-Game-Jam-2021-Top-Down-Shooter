using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
	public static class Extensions
	{
		public static int MaskToLayer(this LayerMask mask)
		{
			int layerNumber = 0;
			int layer = mask.value;
			while (layer > 0)
			{
				layer = layer >> 1;
				layerNumber++;
			}
			return layerNumber - 1;
		}
		public static float ConvertToAngle180(float input)
		{
			while (input > 360)
			{
				input = input - 360;
			}
			while (input < -360)
			{
				input = input + 360;
			}
			if (input > 180)
			{
				input = input - 360;
			}
			if (input < -180)
				input = 360 + input;
			return input;
		}
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			return rotation * (point - pivot) + pivot;
		}
		public static float Map(this float s, float a1, float a2, float b1, float b2)
		{
			return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
		}
	}
}
