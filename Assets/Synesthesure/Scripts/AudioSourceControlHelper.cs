using UnityEngine;

namespace Synesthesure
{
    public class AudioSourceControlHelper : MonoBehaviour
    {
        [Header("Helper for building a Command Events list while in Play Mode.")]
        public AudioSource audioSource;
        [SerializeField, ReadOnlyAtribute] float playTime;
        [SerializeField] float seekTime;
        bool wasStopped;
        CommandEvents CommandEvents;

        void Awake()
        {
            CommandEvents = GetComponent<CommandEvents>();
        }

        void Start()
        {
            audioSource.time = seekTime;
        }

        void Update()
        {
            playTime = audioSource.time;
            seekTime = Mathf.Clamp(seekTime, 0f, audioSource.clip.length);
        }

        public void PlayPause()
        {
            if (audioSource.isPlaying) audioSource.Pause();
            else
            {
                if (wasStopped)
                {
                    audioSource.time = seekTime;
                    wasStopped = false;
                }
                audioSource.Play();
            }
        }

        public void Stop()
        {
            audioSource.Stop();
            if (CommandEvents != null) CommandEvents.RefreshEventList();
            wasStopped = true;
        }

    }
}