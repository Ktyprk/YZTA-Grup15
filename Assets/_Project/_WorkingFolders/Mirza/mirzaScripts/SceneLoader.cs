using UnityEngine;

public class SceneLoader : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            //SceneFader.Instance.loadSceneWithTrigger();
        }
    }
}
