using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class TimelineCameraBinder : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private void Awake()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        BindCinemachineTrack();
    }

    private void BindCinemachineTrack()
    {
        if (director.playableAsset == null)
        {
            Debug.LogWarning("PlayableAsset atanmadı.");
            return;
        }

        foreach (var output in director.playableAsset.outputs)
        {
            if (output.sourceObject is CinemachineTrack)
            {
                var brain = FindObjectOfType<CinemachineBrain>();
                if (brain != null)
                {
                    director.SetGenericBinding(output.sourceObject, brain);
                    Debug.Log("CinemachineBrain Timeline’a başarıyla bind edildi.");
                }
                else
                {
                    Debug.LogError("CinemachineBrain sahnede bulunamadı!");
                }
            }
            
            if (output.sourceObject is AnimationTrack)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                if(currentScene == "BoosRoom")
                {
                    GameObject animator = GameObject.FindGameObjectWithTag("Enemy");
                    var animatorComponent = animator?.GetComponent<Animator>();
                    if (animator != null)
                    {
                        director.SetGenericBinding(output.sourceObject, animatorComponent);
                        Debug.Log("Animator Timeline’a bağlandı.");
                    }
                    else
                    {
                        Debug.LogWarning("Animator sahnede bulunamadı.");
                    }
                }
                else
                {
                    GameObject animator = GameObject.FindGameObjectWithTag("Player");
                    var animatorComponent = animator?.GetComponent<Animator>();
                    if (animator != null)
                    {
                        director.SetGenericBinding(output.sourceObject, animatorComponent);
                        Debug.Log("Animator Timeline’a bağlandı.");
                    }
                    else
                    {
                        Debug.LogWarning("Animator sahnede bulunamadı.");
                    }
                }

              
            }
        }
    }
}