// THIS SCRIPT DEMONSTRATES HOW TO USE EVERY ASPECT OF THE AUDIO-VISUAL INTERFACE

using System.Collections;
using UnityEngine;
namespace Synesthesure
{
    public class SceneControl_AudioToVisual : MonoBehaviour
    {
        AudioVisual AV;

        #region --- Interface ---
        [Header("Beat Triggers")]
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float volumeBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float bassBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float midBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float trebleBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float sibilanceBeatTriggerDurationMinimum = 0.15f;

        [Header("Reference Game Object")]
        public GameObject go;

        [Header("Dominant Tone")]
        public Transform toneIndicator_transform;
        GameObject goTone;

        [Header("Audio Dynamics")]
        public GameObject goLeftVolume;
        public GameObject goRightVolume;

        public GameObject goVolume_Raw;
        public GameObject goVolume_Smooth;
        public GameObject goVolume_Average;
        public GameObject goVolume_Chase;
        public GameObject goVolume_Decay;
        public GameObject goVolume_Attack;
        public GameObject goVolume_Beat;

        public GameObject goBass_Raw;
        public GameObject goBass_Smooth;
        public GameObject goBass_Average;
        public GameObject goBass_Chase;
        public GameObject goBass_Decay;
        public GameObject goBass_Attack;
        public GameObject goBass_Beat;

        public GameObject goMid_Raw;
        public GameObject goMid_Smooth;
        public GameObject goMid_Average;
        public GameObject goMid_Chase;
        public GameObject goMid_Decay;
        public GameObject goMid_Attack;
        public GameObject goMid_Beat;

        public GameObject goTreble_Raw;
        public GameObject goTreble_Smooth;
        public GameObject goTreble_Average;
        public GameObject goTreble_Chase;
        public GameObject goTreble_Decay;
        public GameObject goTreble_Attack;
        public GameObject goTreble_Beat;

        public GameObject goSibilance_Raw;
        public GameObject goSibilance_Smooth;
        public GameObject goSibilance_Average;
        public GameObject goSibilance_Chase;
        public GameObject goSibilance_Decay;
        public GameObject goSibilance_Attack;
        public GameObject goSibilance_Beat;
        #endregion

        private void Start()
        {
            AV = AudioVisual.AV;

            goTone = Instantiate(go, toneIndicator_transform.position, Quaternion.identity, toneIndicator_transform);

            // SUBSCRIBE event listeners to the beat event triggers,
            AV.BeatTrigger_VOLUME += VolumeBeatTriggered;
            AV.BeatTrigger_BASS += BassBeatTriggered;
            AV.BeatTrigger_MID += MidBeatTriggered;
            AV.BeatTrigger_TREBLE += TrebleBeatTriggered;
            AV.BeatTrigger_SIBILANCE += SibilanceBeatTriggered;
            AV.RefreshBeatListenerCount();
        }
        private void OnDestroy()
        {
            // MUST UNSUBSCRIBE the beat trigger event listeners,
            // or attempts to trigger them will continued after the effect's scene is no longer loaded.
            AV.BeatTrigger_VOLUME -= VolumeBeatTriggered;
            AV.BeatTrigger_BASS -= BassBeatTriggered;
            AV.BeatTrigger_MID -= MidBeatTriggered;
            AV.BeatTrigger_TREBLE -= TrebleBeatTriggered;
            AV.BeatTrigger_SIBILANCE -= SibilanceBeatTriggered;
            AV.RefreshBeatListenerCount();
        }

        void Update()
        {
            goTone.transform.localPosition = new Vector3(1f + AV.pitchValue * 6f, 0f, 0f);

            goLeftVolume.transform.localScale = new Vector3(1f, AV.leftVolume * 10f, 1f);
            goRightVolume.transform.localScale = new Vector3(1f, AV.rightVolume * 10f, 1f);
            goLeftVolume.transform.position = new Vector3(goLeftVolume.transform.position.x, AV.leftVolume * 5f, goLeftVolume.transform.position.z);
            goRightVolume.transform.position = new Vector3(goRightVolume.transform.position.x, AV.rightVolume * 5f, goRightVolume.transform.position.z);

            goVolume_Raw.transform.localScale = new Vector3(AV.Volume.raw, AV.Volume.raw, AV.Volume.raw);
            goVolume_Smooth.transform.localScale = new Vector3(AV.Volume.smoothed, AV.Volume.smoothed, AV.Volume.smoothed);
            goVolume_Average.transform.localScale = new Vector3(AV.Volume.average, AV.Volume.average, AV.Volume.average);
            goVolume_Attack.transform.localScale = new Vector3(AV.Volume.attack, AV.Volume.attack, AV.Volume.attack);
            goVolume_Decay.transform.localScale = new Vector3(AV.Volume.decay, AV.Volume.decay, AV.Volume.decay);
            goVolume_Chase.transform.localScale = new Vector3(AV.Volume.chase, AV.Volume.chase, AV.Volume.chase);

            goBass_Raw.transform.localScale = new Vector3(AV.Bass.raw, AV.Bass.raw, AV.Bass.raw);
            goBass_Smooth.transform.localScale = new Vector3(AV.Bass.smoothed, AV.Bass.smoothed, AV.Bass.smoothed);
            goBass_Average.transform.localScale = new Vector3(AV.Bass.average, AV.Bass.average, AV.Bass.average);
            goBass_Attack.transform.localScale = new Vector3(AV.Bass.attack, AV.Bass.attack, AV.Bass.attack);
            goBass_Decay.transform.localScale = new Vector3(AV.Bass.decay, AV.Bass.decay, AV.Bass.decay);
            goBass_Chase.transform.localScale = new Vector3(AV.Bass.chase, AV.Bass.chase, AV.Bass.chase);

            goMid_Raw.transform.localScale = new Vector3(AV.Mid.raw, AV.Mid.raw, AV.Mid.raw);
            goMid_Smooth.transform.localScale = new Vector3(AV.Mid.smoothed, AV.Mid.smoothed, AV.Mid.smoothed);
            goMid_Average.transform.localScale = new Vector3(AV.Mid.average, AV.Mid.average, AV.Mid.average);
            goMid_Attack.transform.localScale = new Vector3(AV.Mid.attack, AV.Mid.attack, AV.Mid.attack);
            goMid_Decay.transform.localScale = new Vector3(AV.Mid.decay, AV.Mid.decay, AV.Mid.decay);
            goMid_Chase.transform.localScale = new Vector3(AV.Mid.chase, AV.Mid.chase, AV.Mid.chase);

            goTreble_Raw.transform.localScale = new Vector3(AV.Treble.raw, AV.Treble.raw, AV.Treble.raw);
            goTreble_Smooth.transform.localScale = new Vector3(AV.Treble.smoothed, AV.Treble.smoothed, AV.Treble.smoothed);
            goTreble_Average.transform.localScale = new Vector3(AV.Treble.average, AV.Treble.average, AV.Treble.average);
            goTreble_Attack.transform.localScale = new Vector3(AV.Treble.attack, AV.Treble.attack, AV.Treble.attack);
            goTreble_Decay.transform.localScale = new Vector3(AV.Treble.decay, AV.Treble.decay, AV.Treble.decay);
            goTreble_Chase.transform.localScale = new Vector3(AV.Treble.chase, AV.Treble.chase, AV.Treble.chase);

            goSibilance_Raw.transform.localScale = new Vector3(AV.Sibilance.raw, AV.Sibilance.raw, AV.Sibilance.raw);
            goSibilance_Smooth.transform.localScale = new Vector3(AV.Sibilance.smoothed, AV.Sibilance.smoothed, AV.Sibilance.smoothed);
            goSibilance_Average.transform.localScale = new Vector3(AV.Sibilance.average, AV.Sibilance.average, AV.Sibilance.average);
            goSibilance_Attack.transform.localScale = new Vector3(AV.Sibilance.attack, AV.Sibilance.attack, AV.Sibilance.attack);
            goSibilance_Decay.transform.localScale = new Vector3(AV.Sibilance.decay, AV.Sibilance.decay, AV.Sibilance.decay);
            goSibilance_Chase.transform.localScale = new Vector3(AV.Sibilance.chase, AV.Sibilance.chase, AV.Sibilance.chase);
        }

        #region --- Beat Control ---
        public void VolumeBeatTriggered()
        {
            StartCoroutine(VolumeBeat());
        }
        IEnumerator VolumeBeat()
        {
            goVolume_Beat.gameObject.SetActive(true);
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < volumeBeatTriggerDurationMinimum)
            {
                animationTimeElapsed += Time.deltaTime;
                yield return null;
            }
            goVolume_Beat.gameObject.SetActive(false);
        }

        public void BassBeatTriggered()
        {
            StartCoroutine(BassBeat());
        }
        IEnumerator BassBeat()
        {
            goBass_Beat.gameObject.SetActive(true);
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < bassBeatTriggerDurationMinimum)
            {
                animationTimeElapsed += Time.deltaTime;
                yield return null;
            }
            goBass_Beat.gameObject.SetActive(false);
        }

        public void MidBeatTriggered()
        {
            StartCoroutine(MidBeat());
        }
        IEnumerator MidBeat()
        {
            goMid_Beat.gameObject.SetActive(true);
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < midBeatTriggerDurationMinimum)
            {
                animationTimeElapsed += Time.deltaTime;
                yield return null;
            }
            goMid_Beat.gameObject.SetActive(false);
        }

        public void TrebleBeatTriggered()
        {
            StartCoroutine(TrebleBeat());
        }
        IEnumerator TrebleBeat()
        {
            goTreble_Beat.gameObject.SetActive(true);
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < trebleBeatTriggerDurationMinimum)
            {
                animationTimeElapsed += Time.deltaTime;
                yield return null;
            }
            goTreble_Beat.gameObject.SetActive(false);
        }

        public void SibilanceBeatTriggered()
        {
            StartCoroutine(SibilanceBeat());
        }
        IEnumerator SibilanceBeat()
        {
            goSibilance_Beat.gameObject.SetActive(true);
            float animationTimeElapsed = 0f;
            while (animationTimeElapsed < sibilanceBeatTriggerDurationMinimum)
            {
                animationTimeElapsed += Time.deltaTime;
                yield return null;
            }
            goSibilance_Beat.gameObject.SetActive(false);
        }
        #endregion

    }
}