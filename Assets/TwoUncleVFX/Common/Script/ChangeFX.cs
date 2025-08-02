using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFXTools
{
	public class ChangeFX : MonoBehaviour
	{
		public List<GameObject> FX;
		float time;
		public float waitTime = 5f;
		void Start()
		{
			FX.ForEach(obj => obj.SetActive(false));
			FX[0].SetActive(true);
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
			var index = FX.FindIndex(obj => obj.activeSelf);
			if (index < FX.Count - 1)
			{
				FX[index].SetActive(false);
				FX[index + 1].SetActive(true);
			}
			else
			{
				FX[index].SetActive(false);
				FX[0].SetActive(true);
			}
		}
	}
}