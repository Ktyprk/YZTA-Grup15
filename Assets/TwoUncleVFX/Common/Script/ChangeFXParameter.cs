using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace VFXTools
{
	public class ChangeFXParameter : MonoBehaviour
	{
		private VisualEffect FX;
		float time = 0;
		public string parameterName = "Radius";
		public float minValue = 0;
		public float maxValue = 1;
		void Start()
		{
			FX = GetComponent<VisualEffect>();
		}

		// Update is called once per frame
		void Update()
		{
			time +=2f * Time.deltaTime;
			DoChangeFX();
		}
		private void DoChangeFX()
		{
			FX.SetFloat(parameterName, minValue + (maxValue - minValue) * (float)Math.Abs(Math.Sin(time)));
		}
	}
}