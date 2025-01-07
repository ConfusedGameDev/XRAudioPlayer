// GETS THE VALUE OF AN AUDIO DYNAMIC

using UnityEngine;

namespace Synesthesure
{
    public class AudioReaction : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes reactionSource;
        [Space]
        [SerializeField] string description;
        [SerializeField] ResponseCurve _responseCurve;
        [HideInInspector] public float value;
        public AnimationCurve responseCurve;

        void Awake()
        {
            if (_responseCurve != null)
            {
                description = _responseCurve.responseCurve.description;
                responseCurve = _responseCurve.responseCurve.curve;
            }
            else if (responseCurve == null || responseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                responseCurve = curve;
            }
        }

        private void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();
        }


        void Update()
        {
            if (!audioSource.isPlaying) return;

            if (reactionSource == AudioVisual.DynamicTypes.None) value = 0f;
            else value = responseCurve.Evaluate(AV.ReactionValue(reactionSource));
        }
    }
}