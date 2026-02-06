using UnityEngine;

namespace SFX
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip buttonClip;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayButtonClip()
        {
            _audioSource.PlayOneShot(buttonClip);
        }
    }
}