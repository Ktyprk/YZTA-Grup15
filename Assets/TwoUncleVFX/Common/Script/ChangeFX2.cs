using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFXTools
{
	public class ChangeFX2 : MonoBehaviour
	{
		public List<GameObject> FX;
		float time;
		public float waitTime = 1f;
		void Start()
		{
			FX.ForEach(obj => obj.SetActive(true));
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				DoChangeFX();
			}
			else if (time < waitTime)
			{
				time += Time.deltaTime;
			}
			else if (time >= waitTime)
			{
				DoChangeFX();
			}
		}

		void DoChangeFX()
		{
			time = 0;
			FX.ForEach(obj => obj.SetActive(false));
			FX.ForEach(obj => obj.SetActive(true));

		}
	}
}