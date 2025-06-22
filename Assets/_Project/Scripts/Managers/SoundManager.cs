using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class SoundManager : MonoBehaviour
{
        public static SoundManager Instance;

        [FormerlySerializedAs("audioClips")] [SerializeField]
        private AudioClip[] _audioClips;
        [FormerlySerializedAs("sources")] [SerializeField]
        private List<AudioSource> _sources = new List<AudioSource>();

        private float _audioVolume;

        private Transform _sourcesParent;

        private void Awake()
        {
            if (Instance == null) Init();
        }

        public void Init()
        {
            Instance = this;

            _audioVolume = PlayerPrefs.GetFloat("Option_Audio_SoundEffects", 1);

            _sourcesParent = new GameObject("Audio Sources").transform;
            _sourcesParent.SetParent(transform, true);
        }

        public void PlayAudio3D(Vector3 position, string name, float volume = 1, float pitch = 1, float distance = 30)
        {
            AudioClip selectedClip = null;

            foreach (AudioClip clip in _audioClips)
            {
                if (clip.name == name)
                {
                    selectedClip = clip;
                }
            }

            if (selectedClip != null)
            {
                AudioSource source;

                if (_sources.Count == 0)
                {
                    source = new GameObject("AudioSource").AddComponent<AudioSource>();
                    source.transform.SetParent(_sourcesParent, true);
                }
                else
                {
                    source = _sources[0];
                    _sources.Remove(source);
                }

                source.clip = selectedClip;
                source.volume = _audioVolume * volume;
                source.pitch = pitch;

                source.spatialBlend = 1;
                source.minDistance = 0;
                source.maxDistance = distance;
                source.rolloffMode = AudioRolloffMode.Linear;

                source.transform.position = position;

                source.enabled = true;
                source.Play();

                StartCoroutine(Poolize(source));
            }
        }

        public void PlayAudio(string name, float volume = 1, float pitch = 1)
        {
            AudioClip selectedClip = null;

            foreach (AudioClip clip in _audioClips)
            {
                if (clip.name == name)
                {
                    selectedClip = clip;
                }
            }

            if (selectedClip != null)
            {
                AudioSource source;

                if (_sources.Count == 0)
                {
                    source = new GameObject("AudioSource").AddComponent<AudioSource>();
                    source.transform.SetParent(_sourcesParent, true);
                }
                else
                {
                    source = _sources[0];
                    _sources.Remove(source);
                }

                source.clip = selectedClip;
                source.volume = _audioVolume * volume;
                source.pitch = pitch;

                source.enabled = true;
                source.Play();

                StartCoroutine(Poolize(source));
            }
        }

        private IEnumerator Poolize(AudioSource source)
        {
            float wait = source.clip.length;
            wait = Mathf.Clamp(wait, Time.deltaTime, wait);

            yield return new WaitForSeconds(wait);

            source.enabled = false;
            _sources.Add(source);

            source.spatialBlend = 0;
            source.transform.position = Vector3.zero;
        }
}