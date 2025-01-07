using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace Synesthesure
{
    public class AudioVisual : MonoBehaviour
    {
        public static AudioVisual AV;
        static bool instantiated;

        [Tooltip("Uses the Audio Listener if no Audio Source is set \n or if the Audio Source has no clip.")]
        public AudioSource audioInput;
        [Tooltip("Forces the use of the AudioSource... \n even if it has no audio clip set.")]
        public bool isStreamingAudio;
        [HideInInspector] public float rmsLeftStream;
        [HideInInspector] public float rmsRightStream;

        enum Bins { low = 128, normal = 256, high = 512 }
        [Tooltip("Resolution of the audio input sampling.")]
        [SerializeField] Bins resolution = Bins.normal;
        Bins previousResolution;
        int SAMPLE_SIZE = 512;
        float[] samples;
        [HideInInspector] public float[] spectrum;
        float[] spectrum_left;
        float[] spectrum_right;

        float rmsValue;
        float dbValue;
        float rmsLeftValue;
        float dbLeftValue;
        float rmsRightValue;
        float dbRightValue;
        float sum;
        [HideInInspector] public float leftVolume;
        float velocity_leftVolume;
        [HideInInspector] public float rightVolume;
        float velocity_rightVolume;

        [Tooltip("Pitch detection's sensitivity to changes.")]
        [Range(0f, 1f)] public float pitchSensitivity = 0.7f;
        [HideInInspector] public float pitchValue;
        float weight_total;
        float pitchVelocity = 0.0f;

        // ---- EVENTS ---------

        [Space]
        public UnityEvent BeatTriggered_VOLUME;
        public UnityEvent BeatTriggered_BASS;
        public UnityEvent BeatTriggered_MID;
        public UnityEvent BeatTriggered_TREBLE;
        public UnityEvent BeatTriggered_SIBILANCE;
        public event Action BeatTrigger_VOLUME;
        public event Action BeatTrigger_BASS;
        public event Action BeatTrigger_MID;
        public event Action BeatTrigger_TREBLE;
        public event Action BeatTrigger_SIBILANCE;

        // Audio Dynamics
        #region
        public struct AudioValues
        {
            /// <summary>
            /// The raw value.
            /// </summary>
            public float raw;
            /// <summary>
            /// The raw value smoothed.
            /// </summary>
            public float smoothed;
            /// <summary>
            /// The average raw value.
            /// </summary>
            public float average;
            /// <summary>
            /// The attack dynamic increases when the raw is greater than average.
            /// </summary>
            public float attack;
            /// <summary>
            /// Matches the peak value, but then decreases if not "pushed" up constantly.
            /// </summary>
            public float decay;
            /// <summary>
            /// Chases the raw value: The bigger the difference, the faster it changes. (Similar to smoothed value)
            /// </summary>
            public float chase;
            /// <summary>
            /// Threshold to be considered for beat calculations. (A value of 0-1.)
            /// </summary>
            public float beatHighpassThreshold;
            /// <summary>
            /// If a beat has been triggered.
            /// </summary>
            public bool beatTriggered;
            /// <summary>
            /// INTERNAL USE
            /// </summary>
            public float previousAttack;
        }
        public AudioValues Volume;
        public AudioValues Bass;
        public AudioValues Mid;
        public AudioValues Treble;
        public AudioValues Sibilance;

        float smoothVelocity_volume;
        const float smoothTime_volume = .05f;
        float averageVelocity_volume;
        const float averageTime_volume = 1f;
        float attackVelocity_volume;
        const float attackTime_volume = .1f;
        float decayVelocity_volume;
        const float decayTime_volume = 1f;

        float smoothVelocity_bass;
        const float smoothTime_bass = .05f;
        float averageVelocity_bass;
        const float averageTime_bass = 1f;
        float attackVelocity_bass;
        const float attackTime_bass = .1f;
        float decayVelocity_bass;
        const float decayTime_bass = 1f;

        float smoothVelocity_mid;
        const float smoothTime_mid = .05f;
        float averageVelocity_mid;
        const float averageTime_mid = .75f;
        float attackVelocity_mid;
        const float attackTime_mid = .1f;
        float decayVelocity_mid;
        const float decayTime_mid = 1f;

        float smoothVelocity_treble;
        const float smoothTime_treble = .04f;
        float averageVelocity_treble;
        const float averageTime_treble = .5f;
        float attackVelocity_treble;
        const float attackTime_treble = .1f;
        float decayVelocity_treble;
        const float decayTime_treble = 1f;

        float smoothVelocity_sibilance;
        const float smoothTime_sibilance = .04f;
        float averageVelocity_sibilance;
        const float averageTime_sibilance = .5f;
        float attackVelocity_sibilance;
        const float attackTime_sibilance = .07f;
        float decayVelocity_sibilance;
        const float decayTime_sibilance = 1f;

        bool useBeatUnityEvent_Volume;
        bool useBeatUnityEvent_Bass;
        bool useBeatUnityEvent_Mid;
        bool useBeatUnityEvent_Treble;
        bool useBeatUnityEvent_Sibilance;
        bool useBeatEvent_Volume;
        bool useBeatEvent_Bass;
        bool useBeatEvent_Mid;
        bool useBeatEvent_Treble;
        bool useBeatEvent_Sibilance;

        public enum DynamicTypes
        {
            None = 0,
            Pitch = -3, LeftVolume = -1, RightVolume = -2,
            VolumeRaw = 1, VolumeSmoothed = 2, VolumeAveraged = 3, VolumeChase = 4, VolumeDecay = 5, VolumeAttack = 6,
            BassRaw = 7, BassSmoothed = 8, BassAveraged = 9, BassChase = 10, BassDecay = 11, BassAttack = 12,
            MidRaw = 13, MidSmoothed = 14, MidAveraged = 15, MidChase = 16, MidDecay = 17, MidAttack = 18,
            TrebleRaw = 19, TrebleSmoothed = 20, TrebleAveraged = 21, TrebleChase = 22, TrebleDecay = 23, TrebleAttack = 24,
            SibilanceRaw = 25, SibilanceSmoothed = 26, SibilanceAveraged = 27, SibilanceChase = 28, SibilanceDecay = 29, SibilanceAttack = 30,
        }
        float reaction;
        public float ReactionValue(DynamicTypes dynamicType)
        {
            switch (dynamicType)
            {
                case AudioVisual.DynamicTypes.VolumeRaw:
                    reaction = Volume.raw;
                    break;
                case AudioVisual.DynamicTypes.VolumeSmoothed:
                    reaction = Volume.smoothed;
                    break;
                case AudioVisual.DynamicTypes.VolumeAveraged:
                    reaction = Volume.average;
                    break;
                case AudioVisual.DynamicTypes.VolumeChase:
                    reaction = Volume.chase;
                    break;
                case AudioVisual.DynamicTypes.VolumeDecay:
                    reaction = Volume.decay;
                    break;
                case AudioVisual.DynamicTypes.VolumeAttack:
                    reaction = Volume.attack;
                    break;
                case AudioVisual.DynamicTypes.BassRaw:
                    reaction = Bass.raw;
                    break;
                case AudioVisual.DynamicTypes.BassSmoothed:
                    reaction = Bass.smoothed;
                    break;
                case AudioVisual.DynamicTypes.BassAveraged:
                    reaction = Bass.average;
                    break;
                case AudioVisual.DynamicTypes.BassChase:
                    reaction = Bass.chase;
                    break;
                case AudioVisual.DynamicTypes.BassDecay:
                    reaction = Bass.decay;
                    break;
                case AudioVisual.DynamicTypes.BassAttack:
                    reaction = Bass.attack;
                    break;
                case AudioVisual.DynamicTypes.MidRaw:
                    reaction = Mid.raw;
                    break;
                case AudioVisual.DynamicTypes.MidSmoothed:
                    reaction = Mid.smoothed;
                    break;
                case AudioVisual.DynamicTypes.MidAveraged:
                    reaction = Mid.average;
                    break;
                case AudioVisual.DynamicTypes.MidChase:
                    reaction = Mid.chase;
                    break;
                case AudioVisual.DynamicTypes.MidDecay:
                    reaction = Mid.decay;
                    break;
                case AudioVisual.DynamicTypes.MidAttack:
                    reaction = Mid.attack;
                    break;
                case AudioVisual.DynamicTypes.TrebleRaw:
                    reaction = Treble.raw;
                    break;
                case AudioVisual.DynamicTypes.TrebleSmoothed:
                    reaction = Treble.smoothed;
                    break;
                case AudioVisual.DynamicTypes.TrebleAveraged:
                    reaction = Treble.average;
                    break;
                case AudioVisual.DynamicTypes.TrebleChase:
                    reaction = Treble.chase;
                    break;
                case AudioVisual.DynamicTypes.TrebleDecay:
                    reaction = Treble.decay;
                    break;
                case AudioVisual.DynamicTypes.TrebleAttack:
                    reaction = Treble.attack;
                    break;
                case AudioVisual.DynamicTypes.SibilanceRaw:
                    reaction = Sibilance.raw;
                    break;
                case AudioVisual.DynamicTypes.SibilanceSmoothed:
                    reaction = Sibilance.smoothed;
                    break;
                case AudioVisual.DynamicTypes.SibilanceAveraged:
                    reaction = Sibilance.average;
                    break;
                case AudioVisual.DynamicTypes.SibilanceChase:
                    reaction = Sibilance.chase;
                    break;
                case AudioVisual.DynamicTypes.SibilanceDecay:
                    reaction = Sibilance.decay;
                    break;
                case AudioVisual.DynamicTypes.SibilanceAttack:
                    reaction = Sibilance.attack;
                    break;
            }
            return reaction;
        }

        #endregion


        void SetSampleSize()
        {
            previousResolution = resolution;
            SAMPLE_SIZE = (int)resolution;
            samples = new float[SAMPLE_SIZE];
            spectrum = new float[SAMPLE_SIZE];
            spectrum_left = new float[SAMPLE_SIZE];
            spectrum_right = new float[SAMPLE_SIZE];
        }
        void Awake()
        {
            if (instantiated) return;
            AV = this;
            instantiated = true;

            SetSampleSize();
            #region Initialize Audio Dynamics Values
            Volume.raw = 0f;
            Volume.smoothed = 0f;
            Volume.average = 0f;
            Volume.attack = 0f;
            Volume.decay = 0f;
            Volume.chase = 0f;
            Volume.beatHighpassThreshold = .15f;
            Volume.previousAttack = 0f;

            Bass.raw = 0f;
            Bass.smoothed = 0f;
            Bass.average = 0f;
            Bass.attack = 0f;
            Bass.decay = 0f;
            Bass.chase = 0f;
            Bass.beatHighpassThreshold = .15f;
            Bass.previousAttack = 0f;

            Mid.raw = 0f;
            Mid.smoothed = 0f;
            Mid.average = 0f;
            Mid.attack = 0f;
            Mid.decay = 0f;
            Mid.chase = 0f;
            Mid.beatHighpassThreshold = .15f;
            Mid.previousAttack = 0f;

            Treble.raw = 0f;
            Treble.smoothed = 0f;
            Treble.average = 0f;
            Treble.attack = 0f;
            Treble.decay = 0f;
            Treble.chase = 0f;
            Treble.beatHighpassThreshold = .15f;
            Treble.previousAttack = 0f;

            Sibilance.raw = 0f;
            Sibilance.smoothed = 0f;
            Sibilance.average = 0f;
            Sibilance.attack = 0f;
            Sibilance.decay = 0f;
            Sibilance.chase = 0f;
            Sibilance.beatHighpassThreshold = .15f;
            Sibilance.previousAttack = 0f;

            if (BeatTriggered_VOLUME.GetPersistentEventCount() > 0) useBeatUnityEvent_Volume = true;
            if (BeatTriggered_BASS.GetPersistentEventCount() > 0) useBeatUnityEvent_Bass = true;
            if (BeatTriggered_MID.GetPersistentEventCount() > 0) useBeatUnityEvent_Mid = true;
            if (BeatTriggered_TREBLE.GetPersistentEventCount() > 0) useBeatUnityEvent_Treble = true;
            if (BeatTriggered_SIBILANCE.GetPersistentEventCount() > 0) useBeatUnityEvent_Sibilance = true;
            RefreshBeatListenerCount();
            #endregion
        }
        private void Start()
        {
            if (audioInput == null)
            {// if it hasn't been set by another script's Awake() method by now...
                audioInput = GetComponent<AudioSource>();
            }
        }


        void Update()
        {
            if (resolution != previousResolution) SetSampleSize();
            AnalyzeSound();
        }
        void AnalyzeSound()
        {
            // Get Audio Waveform -------------
            // left
            if (audioInput == null || (audioInput.clip == null && !isStreamingAudio))
                AudioListener.GetOutputData(samples, 0);
            else
                audioInput.GetOutputData(samples, 0);
            sum = 0;
            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                sum = samples[i] * samples[i];
            }
            if (sum < 0f || float.IsNaN(sum)) sum = 0f;
            //rmsLeftValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
            rmsLeftValue = Mathf.Min(1.0f, Mathf.Sqrt(sum / SAMPLE_SIZE));
            dbLeftValue = 20f * Mathf.Log10(rmsLeftValue / 0.1f);
            sum = rmsLeftValue * 40f;
            if (sum > 1f)  sum = 1f;
            else if (sum < 0f || float.IsNaN(sum)) sum = 0f;
            leftVolume = Mathf.SmoothDamp(leftVolume, sum, ref velocity_leftVolume, .07f);
            // right
            if (audioInput == null || (audioInput.clip == null && !isStreamingAudio))
                AudioListener.GetOutputData(samples, 1);
            else
                audioInput.GetOutputData(samples, 1);
            sum = 0;
            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                sum = samples[i] * samples[i];
            }
            if (sum < 0f || float.IsNaN(sum)) sum = 0f;
            //rmsRightValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
            rmsRightValue = Mathf.Min(1.0f, Mathf.Sqrt(sum / SAMPLE_SIZE));
            dbRightValue = 20f * Mathf.Log10(rmsRightValue / 0.1f);
            sum = rmsRightValue * 40f;
            if (sum > 1f) sum = 1f;
            else if (sum < 0f || float.IsNaN(sum)) sum = 0f;
            rightVolume = Mathf.SmoothDamp(rightVolume, sum, ref velocity_rightVolume, .07f);
            // average
            rmsValue = (rmsLeftValue + rmsRightValue) / 2f;
            dbValue = (dbLeftValue + dbRightValue) / 2f;

            // Volume
            Volume.raw = rmsValue * 20f;
            if (Volume.raw > 1f) { Volume.raw = 1f; }
            Volume.smoothed = Mathf.SmoothDamp(Volume.smoothed, Volume.raw, ref smoothVelocity_volume, smoothTime_volume);
            Volume.average = Mathf.SmoothDamp(Volume.average, Volume.raw, ref averageVelocity_volume, averageTime_volume);
            Volume.previousAttack = Volume.attack;
            if (Volume.smoothed > Volume.average && Volume.smoothed > .1f)
                Volume.attack = Mathf.SmoothDamp(Volume.attack, 1f, ref attackVelocity_volume, attackTime_volume);
            else
                Volume.attack = Mathf.SmoothDamp(Volume.attack, 0f, ref attackVelocity_volume, attackTime_volume);
            if (Volume.raw > Volume.decay)
                Volume.decay = Volume.smoothed;
            else
                Volume.decay = Mathf.SmoothDamp(Volume.decay, 0f, ref decayVelocity_volume, decayTime_volume);
            Volume.chase = (Volume.smoothed + Volume.average) / 2f;

            // Get Audio Spectrum --------------

            if (audioInput == null || (audioInput.clip == null && !isStreamingAudio))
            {
                AudioListener.GetSpectrumData(spectrum_left, 0, FFTWindow.BlackmanHarris);
                AudioListener.GetSpectrumData(spectrum_right, 1, FFTWindow.BlackmanHarris);
            }
            else
            {
                audioInput.GetSpectrumData(spectrum_left, 0, FFTWindow.BlackmanHarris);
                audioInput.GetSpectrumData(spectrum_right, 1, FFTWindow.BlackmanHarris);
            }
            // average the left and right channels together
            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                spectrum[i] = (spectrum_left[i] + spectrum_right[i]) / 2f;
            }


            // Pitch
            //weight_total = Bass.average + Mid.average + Treble.average + Sibilance.average;
            weight_total = Bass.chase + Mid.chase + Treble.chase + Sibilance.chase;
            if (weight_total <= 0f) { weight_total = .001f; }
            sum = 1f * Bass.average / weight_total + 2f * Mid.average / weight_total + 3f * Treble.average / weight_total + 4f * Sibilance.average / weight_total;
            float pv = (sum - 1f) / 3f;
            pv = Mathf.Clamp(pv, 0f, 1f);
            float ppv = pitchValue;
            pitchValue = Mathf.SmoothDamp(ppv, pv, ref pitchVelocity, 1f - pitchSensitivity);


            // Bass
            Bass.raw = (spectrum[0] + spectrum[1]) * 1.5f;
            if (Bass.raw > 1f) Bass.raw = 1f;
            else if (Bass.raw < 0f || float.IsNaN(Bass.raw)) Bass.raw = 0f;
            Bass.smoothed = Mathf.SmoothDamp(Bass.smoothed, Bass.raw, ref smoothVelocity_bass, smoothTime_bass);
            Bass.average = Mathf.SmoothDamp(Bass.average, Bass.raw, ref averageVelocity_bass, averageTime_bass);
            Bass.previousAttack = Bass.attack;
            if (Bass.smoothed > Bass.average && Bass.smoothed > .16f)
                Bass.attack = Mathf.SmoothDamp(Bass.attack, 1f, ref attackVelocity_bass, attackTime_bass);
            else
                Bass.attack = Mathf.SmoothDamp(Bass.attack, 0f, ref attackVelocity_bass, attackTime_bass);
            if (Bass.raw > Bass.decay)
                Bass.decay = Bass.smoothed;
            else
                Bass.decay = Mathf.SmoothDamp(Bass.decay, 0f, ref decayVelocity_bass, decayTime_bass);
            Bass.chase = (Bass.smoothed + Bass.average) / 2f;

            // Mid
            sum = 0;
            for (int i = 6; i < 12; i++)
            {
                sum += spectrum[i];
            }
            Mid.raw = sum * 3f;
            if (Mid.raw > 1f) Mid.raw = 1f;
            else if (Mid.raw < 0f || float.IsNaN(Mid.raw)) Mid.raw = 0f;
            Mid.smoothed = Mathf.SmoothDamp(Mid.smoothed, Mid.raw, ref smoothVelocity_mid, smoothTime_mid);
            Mid.average = Mathf.SmoothDamp(Mid.average, Mid.raw, ref averageVelocity_mid, averageTime_mid);
            Mid.previousAttack = Mid.attack;
            if (Mid.smoothed > Mid.average && Mid.smoothed > .12f)
                Mid.attack = Mathf.SmoothDamp(Mid.attack, 1f, ref attackVelocity_mid, attackTime_mid);
            else
                Mid.attack = Mathf.SmoothDamp(Mid.attack, 0f, ref attackVelocity_mid, attackTime_mid);
            if (Mid.raw > Mid.decay)
                Mid.decay = Mid.smoothed;
            else
                Mid.decay = Mathf.SmoothDamp(Mid.decay, 0f, ref decayVelocity_mid, decayTime_mid);
            Mid.chase = (Mid.smoothed + Mid.average) / 2f;

            // Treble
            sum = 0;
            for (int i = 18; i < 28; i++)
            {
                sum += spectrum[i];
            }
            Treble.raw = sum * 5f;
            if (Treble.raw > 1f) Treble.raw = 1f;
            else if (Treble.raw < 0f || float.IsNaN(Treble.raw)) Treble.raw = 0f;
            Treble.smoothed = Mathf.SmoothDamp(Treble.smoothed, Treble.raw, ref smoothVelocity_treble, smoothTime_treble);
            Treble.average = Mathf.SmoothDamp(Treble.average, Treble.raw, ref averageVelocity_treble, averageTime_treble);
            Treble.previousAttack = Treble.attack;
            if (Treble.smoothed > Treble.average && Treble.smoothed > .1f)
                Treble.attack = Mathf.SmoothDamp(Treble.attack, 1f, ref attackVelocity_treble, attackTime_treble);
            else
                Treble.attack = Mathf.SmoothDamp(Treble.attack, 0f, ref attackVelocity_treble, attackTime_treble);
            if (Treble.raw > Treble.decay)
                Treble.decay = Treble.smoothed;
            else
                Treble.decay = Mathf.SmoothDamp(Treble.decay, 0f, ref decayVelocity_treble, decayTime_treble);
            Treble.chase = (Treble.smoothed + Treble.average) / 2f;

            // Sibilance
            sum = 0;
            for (int i = 32; i < 64; i++)
            {
                sum += spectrum[i];
            }
            Sibilance.raw = sum * 2.5f;
            if (Sibilance.raw > 1f) Sibilance.raw = 1f;
            else if (Sibilance.raw < 0f || float.IsNaN(Sibilance.raw)) Sibilance.raw = 0f;
            Sibilance.smoothed = Mathf.SmoothDamp(Sibilance.smoothed, Sibilance.raw, ref smoothVelocity_sibilance, smoothTime_sibilance);
            Sibilance.average = Mathf.SmoothDamp(Sibilance.average, Sibilance.raw, ref averageVelocity_sibilance, averageTime_sibilance);
            Sibilance.previousAttack = Sibilance.attack;
            if (Sibilance.smoothed > Sibilance.average && Sibilance.smoothed > .07f)
                Sibilance.attack = Mathf.SmoothDamp(Sibilance.attack, 1f, ref attackVelocity_sibilance, attackTime_sibilance);
            else
                Sibilance.attack = Mathf.SmoothDamp(Sibilance.attack, 0f, ref attackVelocity_sibilance, attackTime_sibilance);
            if (Sibilance.raw > Sibilance.decay)
                Sibilance.decay = Sibilance.smoothed;
            else
                Sibilance.decay = Mathf.SmoothDamp(Sibilance.decay, 0f, ref decayVelocity_sibilance, decayTime_sibilance);
            Sibilance.chase = (Sibilance.smoothed + Sibilance.average) / 2f;

            // Beat Detection =========================================================================

            // Volume
            if ((Volume.smoothed >= Volume.beatHighpassThreshold) && (Volume.attack > Volume.previousAttack) &&
            (!Volume.beatTriggered))
            {
                Volume.beatTriggered = true;
                if (useBeatUnityEvent_Volume) BeatTriggered_VOLUME.Invoke();
                if (useBeatEvent_Volume) BeatTrigger_VOLUME.Invoke();
            }
            else if ((Volume.attack < Volume.previousAttack) && (Volume.beatTriggered))
                Volume.beatTriggered = false;
            // Bass
            if ((Bass.smoothed >= Bass.beatHighpassThreshold) && (Bass.attack > Bass.previousAttack) &&
            (!Bass.beatTriggered))
            {
                Bass.beatTriggered = true;
                if (useBeatUnityEvent_Bass) BeatTriggered_BASS.Invoke();
                if (useBeatEvent_Bass) BeatTrigger_BASS.Invoke();
            }
            else if ((Bass.attack < Bass.previousAttack) && (Bass.beatTriggered))
                Bass.beatTriggered = false;
            // Mid
            if ((Mid.smoothed >= Mid.beatHighpassThreshold) && (Mid.attack > Mid.previousAttack) &&
            (!Mid.beatTriggered))
            {
                Mid.beatTriggered = true;
                if (useBeatUnityEvent_Mid) BeatTriggered_MID.Invoke();
                if (useBeatEvent_Mid) BeatTrigger_MID.Invoke();
            }
            else if ((Mid.attack < Mid.previousAttack) && (Mid.beatTriggered))
                Mid.beatTriggered = false;
            // Treble
            if ((Treble.smoothed >= Treble.beatHighpassThreshold) && (Treble.attack > Treble.previousAttack) &&
            (!Treble.beatTriggered))
            {
                Treble.beatTriggered = true;
                if (useBeatUnityEvent_Treble) BeatTriggered_TREBLE.Invoke();
                if (useBeatEvent_Treble) BeatTrigger_TREBLE.Invoke();
            }
            else if ((Treble.attack < Treble.previousAttack) && (Treble.beatTriggered))
                Treble.beatTriggered = false;
            // Sibilance
            if ((Sibilance.smoothed >= Sibilance.beatHighpassThreshold) && (Sibilance.attack > Sibilance.previousAttack) &&
            (!Sibilance.beatTriggered))
            {
                Sibilance.beatTriggered = true;
                if (useBeatUnityEvent_Sibilance) BeatTriggered_SIBILANCE.Invoke();
                if (useBeatEvent_Sibilance) BeatTrigger_SIBILANCE.Invoke();
            }
            else if ((Sibilance.attack < Sibilance.previousAttack) && (Sibilance.beatTriggered))
                Sibilance.beatTriggered = false;
        }


        public void RefreshBeatListenerCount()
        {
            if (AV) StartCoroutine(CheckForBeatListeners());
        }

        IEnumerator CheckForBeatListeners()
        {
            if (useBeatEvent_Volume || useBeatEvent_Bass || useBeatEvent_Mid || useBeatEvent_Treble || useBeatEvent_Sibilance)
            {
                CheckForBeatListeners_NOW();
            }
            else
            {
                // Can't gaurantee order of execution of Start methods on scene load...
                // So giving it a frame ensures any listeners have subscribed before checking for them.
                yield return null;
                CheckForBeatListeners_NOW();
            }
        }
        void CheckForBeatListeners_NOW()
        {
            if (BeatTrigger_VOLUME != null)
            {
                if (BeatTrigger_VOLUME.GetInvocationList().Length > 0) useBeatEvent_Volume = true;
            }
            else useBeatEvent_Volume = false;
            if (BeatTrigger_BASS != null)
            {
                if (BeatTrigger_BASS.GetInvocationList().Length > 0) useBeatEvent_Bass = true;
            }
            else useBeatEvent_Bass = false;
            if (BeatTrigger_MID != null)
            {
                if (BeatTrigger_MID.GetInvocationList().Length > 0) useBeatEvent_Mid = true;
            }
            else useBeatEvent_Mid = false;
            if (BeatTrigger_TREBLE != null)
            {
                if (BeatTrigger_TREBLE.GetInvocationList().Length > 0) useBeatEvent_Treble = true;
            }
            else useBeatEvent_Treble = false;
            if (BeatTrigger_SIBILANCE != null)
            {
                if (BeatTrigger_SIBILANCE.GetInvocationList().Length > 0) useBeatEvent_Sibilance = true;
            }
            else useBeatEvent_Sibilance = false;
        }

    }
}