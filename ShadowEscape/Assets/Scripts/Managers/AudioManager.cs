using UnityEngine;

namespace ShadowEscape.Managers
{
    /// <summary>
    /// Singleton audio manager for background music and all one-shot SFX
    /// (jump, footstep, button click, door open, victory). Persists across scenes.
    /// Attach to: an empty "AudioManager" GameObject placed in the first-loaded scene only.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        public AudioClip backgroundMusic;
        [Range(0f, 1f)] public float musicVolume = 0.5f;

        [Header("SFX Clips")]
        public AudioClip jumpClip;
        public AudioClip footstepClip;
        public AudioClip buttonClickClip;
        public AudioClip doorOpenClip;
        public AudioClip victoryClip;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;

        private AudioSource musicSource;
        private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        private void Start()
        {
            PlayMusic();
        }

        public void PlayMusic()
        {
            if (backgroundMusic == null) return;
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            musicSource.volume = volume;
        }

        public void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
        }
    }
}
