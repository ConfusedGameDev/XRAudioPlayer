using UnityEngine;
using System.Collections;
namespace Synesthesure
{
    public class SceneControl_MeterBars_Rave : MonoBehaviour
    {
        [SerializeField] AudioVisual AV;
        AudioSource audioSource;

        [Header("Beat Triggers")]
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float bassBeatTriggerDurationMinimum = 0.15f;
        bool bassBeatTriggered;

        [Space]
        [SerializeField] Camera cam;
        Transform camTransform;
        [SerializeField] Transform[] cameraLocations;
        [SerializeField] bool cameraShakingEnabled;
        CameraShake cameraShake;
        [SerializeField] float shakeTime = .1f;

        [Space]
        [SerializeField] Texture2D[] colors;
        MeterBarsArc meterBars;
        [SerializeField] TextureSwapper texturePicture;

        void Awake()
        {
            camTransform = cam.transform;
            cameraShake = cam.GetComponent<CameraShake>();
            meterBars = FindObjectOfType<MeterBarsArc>();
        }

        void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();
            // SUBSCRIBE event listeners to the beat event triggers,
            AV.BeatTrigger_BASS += BassBeatTriggered;
            AV.RefreshBeatListenerCount();
        }

        void OnDestroy()
        {
            // MUST UNSUBSCRIBE the beat trigger event listeners,
            // or attempts to trigger them will continued after the effect's scene is no longer loaded.
            AV.BeatTrigger_BASS -= BassBeatTriggered;
            AV.RefreshBeatListenerCount();
        }


        #region ------- COMMANDS -------

        public void ExecuteCommands(string buffer)
        {
            int ii = buffer.IndexOf((char)127);
            //if (ii < 0) buffer = buffer + (char)127;
            if (ii < 0) buffer += (char)127;
            string[] commands = buffer.Split((char)127);
            for (int i = 0; i < commands.Length; i++)
            {
                ii = commands[i].IndexOf("|");
                if (ii > 0)
                {
                    string command = commands[i].Substring(0, ii);
                    string commandParameter = commands[i].Substring(ii + 1);
                    Commands(command, commandParameter);
                }
            }
        }
        public void ExecuteCommand(string theCommand)
        {
            int ii = theCommand.IndexOf("|");
            if (ii > 0)
            {
                string command = theCommand.Substring(0, ii);
                string commandParameter = theCommand.Substring(ii + 1);
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

                case "METERBARHEIGHT":
                    SetMeterBarsHeightFactor(float.Parse(theParameter));
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

                case "CHANGECOLOR":
                    ChangeTexture(int.Parse(theParameter));
                    break;
            }
        }

        void MusicPosition(float seek)
        {
            seek = Mathf.Clamp(seek, 0f, audioSource.clip.length);
            audioSource.time = seek;
        }

        void SetMeterBarsHeightFactor(float height)
        {
            if (height < 0) height = 0;
            meterBars.meterBarsHeightFactor = height;
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
                camTransform.transform.position = Vector3.Lerp(camTransform.transform.position, cameraLocations[location].position, n);
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


        void ChangeTexture(int index)
        {
            meterBars.colorTexture = colors[index];
            texturePicture.SwapTexture(index);
        }

        #endregion


        #region ------ BEAT CONTROL ------

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
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < bassBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                bassBeatTriggered = false;
            }
        }

        #endregion

    }
}