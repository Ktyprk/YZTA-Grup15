using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;

public class TimelineCameraBinder : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    void Start()
    {
        if (director == null)
        {
            Debug.LogError("PlayableDirector atanmadı.");
            return;
        }

        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("Timeline asset bulunamadı.");
            return;
        }

        foreach (var output in timeline.outputs)
        {
            // Cinemachine Track’leri hedef al
            if (output.outputTargetType == typeof(CinemachineVirtualCameraBase))
            {
                var virtualCam = FindObjectOfType<CinemachineVirtualCameraBase>();
                if (virtualCam != null)
                {
                    director.SetGenericBinding(output.sourceObject, virtualCam);
                    Debug.Log("Cinemachine Virtual Camera başarıyla bağlandı.");
                }
            }
            else if (output.outputTargetType == typeof(GameObject)) // Bazı durumlarda GameObject olabilir
            {
                GameObject go = Camera.main.gameObject;
                if (go.GetComponent<CinemachineBrain>() != null)
                {
                    director.SetGenericBinding(output.sourceObject, go);
                    Debug.Log("CinemachineBrain içeren kamera başarıyla bağlandı.");
                }
            }
        }
    }
}