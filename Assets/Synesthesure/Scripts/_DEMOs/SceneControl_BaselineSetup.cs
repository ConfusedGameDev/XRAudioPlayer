using UnityEngine;
using System.Collections;
namespace Synesthesure
{
    public class SceneControl_BaselineSetup : MonoBehaviour
    {
        AudioVisual AV;
        bool effectsActive = false;
        AudioSource audioSource;

        [Header("Beat Triggers")]
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float volumeBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float bassBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float midBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float trebleBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float sibilanceBeatTriggerDurationMinimum = 0.15f;
        bool volumeBeatTriggered;
        bool bassBeatTriggered;
        bool midBeatTriggered;
        bool trebleBeatTriggered;
        bool sibilanceBeatTriggered;

        // REACTIONS

        [Space]
        public Transform cam;
        public Transform[] cameraLocations;
        public float cameraChangeSpeed = 10f;
        public CameraShake cameraShake;
        public float shakeTime = .1f;
        public bool cameraShakingEnabled;


        void Awake()
        {

        }

        void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();

            // SUBSCRIBE EVENT listeners to the beat event triggers,
            AV.BeatTrigger_VOLUME += VolumeBeatTriggered;
            AV.BeatTrigger_BASS += BassBeatTriggered;
            AV.BeatTrigger_MID += MidBeatTriggered;
            AV.BeatTrigger_TREBLE += TrebleBeatTriggered;
            AV.BeatTrigger_SIBILANCE += SibilanceBeatTriggered;
            AV.RefreshBeatListenerCount();
        }

        void OnDestroy()
        {
            // *** MUST UNSUBSCRIBE *** the beat trigger EVENT listeners,
            // or attempts to trigger them will continued after the effect's scene is no longer loaded.
            // AND FAILING TO UNSUBSCRIBE FROM AN EVENT CAN CAUSE MEMORY LEAKS.
            AV.BeatTrigger_VOLUME -= VolumeBeatTriggered;
            AV.BeatTrigger_BASS -= BassBeatTriggered;
            AV.BeatTrigger_MID -= MidBeatTriggered;
            AV.BeatTrigger_TREBLE -= TrebleBeatTriggered;
            AV.BeatTrigger_SIBILANCE -= SibilanceBeatTriggered;
            // AV.RefreshBeatListenerCount(); // ONLY NEED THIS IF LOADING ANOTHER SCREEN AT RUNTIME
        }


        void Effects(bool active)
        {
            if (active)
            {
                effectsActive = true;
                // do whatever to turn on all effects
            }
            else
            {
                effectsActive = false;
                // do whatever to turn off all effects
            }
        }

        void Update()
        {
            // Automatically Turn Effects ON/OFF
            if (audioSource.isPlaying && !effectsActive) Effects(true);
            else if (!audioSource.isPlaying && effectsActive) Effects(false);

            // Reaction to music...
            if (audioSource.isPlaying)
            {
                // just some examples...
                float volumeReaction = AV.Volume.average;
                float bassReaction = AV.Bass.smoothed;
                float midReaction = AV.Mid.attack;
                float sibilanceReaction = AV.Sibilance.raw;

            }

        }


        #region // ------- COMMANDS -------

        public void ExecuteCommands(string buffer)
        {
            int charIndex = buffer.IndexOf((char)127);
            if (charIndex < 0) buffer += (char)127;
            string[] commands = buffer.Split((char)127);
            for (int i = 0; i < commands.Length; i++)
            {
                charIndex = commands[i].IndexOf("|");
                if (charIndex > 0)
                {
                    string command = commands[i].Substring(0, charIndex);
                    string commandParameter = commands[i].Substring(charIndex + 1);
                    Commands(command, commandParameter);
                }
            }
        }
        public void ExecuteCommand(string theCommand)
        {
            int charIndex = theCommand.IndexOf("|");
            if (charIndex > 0)
            {
                string command = theCommand.Substring(0, charIndex);
                string commandParameter = theCommand.Substring(charIndex + 1);
                Commands(command, commandParameter);
            }
        }

        void Commands(string theCommand, string theParameter)
        {
            switch (theCommand.ToUpper().Trim())
            {
                case "MUSICSEEK":
                    MusicPosition(float.Parse(theParameter));
                    break;

                case "CAMERASHAKE":
                    cameraShakingEnabled = bool.Parse(theParameter);
                    break;

                case "CAMERASHAKE_CONSTANTLY":
                    CameraShakeControl(theParameter);
                    break;

                case "CAMERALOCATION":
                    string[] parameters = theParameter.Split(',');
                    CameraLocation(int.Parse(parameters[0]), float.Parse(parameters[1]));
                    break;
            }
        }

        // --- THE COMMANDS  ---

        void MusicPosition(float seek)
        {
            audioSource.time = seek;
        }


        void CameraLocation(int location, float time)
        {
            if ((location < 0) || (location >= cameraLocations.Length)) return;
            StartCoroutine(ChangeCameraLocation(location, time));
        }
        IEnumerator ChangeCameraLocation(int location, float time)
        {
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < time)
            {
                animationTimeElapsed += Time.deltaTime;
                float n = animationTimeElapsed / time;
                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraLocations[location].position, n);

                // NEED TO ADD LERP FOR ROTATION ----------------<<*******

                yield return null;
            }
        }

        void ShakeCamera()
        {
            if (cameraShakingEnabled) cameraShake.Shake(shakeTime);
        }

        void CameraShakeControl(string parameter)
        {
            if (parameter.ToUpper().Trim() == "ON")
            {
                cameraShake.ShakeConstantly(); ;
            }
            if (parameter.ToUpper().Trim() == "OFF")
            {
                cameraShake.StopShaking();
            }
        }

        #endregion



        #region ------ BEAT CONTROL ------

        public void VolumeBeatTriggered()
        {
            StartCoroutine(VolumeBeat());
        }
        IEnumerator VolumeBeat()
        {
            if (!volumeBeatTriggered)
            {// Triggered here
                volumeBeatTriggered = true;
                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < volumeBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                volumeBeatTriggered = false;
            }
        }

        public void BassBeatTriggered()
        {
            StartCoroutine(BassBeat());
        }
        IEnumerator BassBeat()
        {
            if (!bassBeatTriggered)
            {// Triggered here
                ShakeCamera();
                bassBeatTriggered = true;
                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < bassBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                bassBeatTriggered = false;
            }
        }

        public void MidBeatTriggered()
        {
            StartCoroutine(MidBeat());
        }
        IEnumerator MidBeat()
        {
            if (!midBeatTriggered)
            {// Triggered here
                midBeatTriggered = true;
                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < midBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                midBeatTriggered = false;
            }
        }

        public void TrebleBeatTriggered()
        {
            StartCoroutine(TrebleBeat());
        }
        IEnumerator TrebleBeat()
        {
            if (!trebleBeatTriggered)
            {// Triggered here
                trebleBeatTriggered = true;
                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < trebleBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                trebleBeatTriggered = false;
            }
        }

        public void SibilanceBeatTriggered()
        {
            StartCoroutine(SibilanceBeat());
        }
        IEnumerator SibilanceBeat()
        {
            if (!sibilanceBeatTriggered)
            {// Triggered here
                sibilanceBeatTriggered = true;
                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < sibilanceBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                sibilanceBeatTriggered = false;
            }
        }

        #endregion
    }
}