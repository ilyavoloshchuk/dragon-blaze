using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioSource musicSource;

        [Header("UI References")]
        public Slider musicSlider;
        public Slider soundSlider;
        
        private void Awake()
        {
            InitializeSingleton();
            InitializeAudioSources();
            InitializeSliders();
        }
        
        private void InitializeSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            if (soundSource == null) soundSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

            var savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            var savedSoundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
            musicSource.volume = savedMusicVolume;
            soundSource.volume = savedSoundVolume;
        }

        private void InitializeSliders()
        {
            if (musicSlider != null)
            {
                musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
                musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            }

            if (soundSlider != null)
            {
                soundSlider.value = PlayerPrefs.GetFloat("soundVolume", 0.5f);
                soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
            }
        }
        
        public void PlaySound(AudioClip sound)
        {
            soundSource.PlayOneShot(sound);
        }
        
        public void ChangeSoundVolume(float volume)
        {
            soundSource.volume = volume;
            PlayerPrefs.SetFloat("soundVolume", volume);
            PlayerPrefs.Save();

            if (soundSlider != null)
            {
                soundSlider.value = volume;
            }
        }

        public void ChangeMusicVolume(float volume)
        {
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("musicVolume", volume);
            PlayerPrefs.Save();

            if (musicSlider != null)
            {
                musicSlider.value = volume;
            }
        }
    }
}