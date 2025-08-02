using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VFXTools
{
	public class CheckMSAA : MonoBehaviour
	{
		public Volume volume;
		void Start()
		{
			if (volume == null)
				volume = gameObject.GetComponent<Volume>();
			if (volume != null)
				if (QualitySettings.antiAliasing > 0 && Camera.main != null && Camera.main.allowMSAA)
				{
					volume.enabled = false;
				}
				else
				{
					volume.enabled = true;
				}
		}
	}
}