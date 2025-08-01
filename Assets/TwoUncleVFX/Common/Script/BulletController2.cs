using System.Threading.Tasks;
using UnityEngine;

namespace VFXTools
{
	public class BulletController2 : MonoBehaviour
	{
		public Transform rotationCenter;   
		public float rotationSpeed = 100f; 
		public float movementSpeed = 10f;
		public float delayTime = 0f;
		private bool isPlay = false;

		private void Start()
		{
			SetPlay(true);
		}

		private async void SetPlay(bool play)
		{
			await Task.Delay((int)(delayTime * 1000));
			isPlay = play;
		}
		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Space))
			{
				isPlay = !isPlay;
			}
			if(!isPlay) return;
			Vector3 directionToCenter = rotationCenter.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
		}
	}
}