using UnityEngine;

namespace Synesthesure
{
    public class ObjectsArrangementModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes controlSource;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve;

        [Space]
        [SerializeField] ObjectsArrangement objectsArrangement;
        [SerializeField] float minRadius = 1f;
        [SerializeField] float maxRadius = 20f;
        
        void Awake()
        {
            if (_responseCurve != null) responseCurve = _responseCurve.responseCurve.curve;
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

            if (controlSource != AudioVisual.DynamicTypes.None)
            {
                objectsArrangement.radius = Mathf.Lerp(minRadius, maxRadius, responseCurve.Evaluate(AV.ReactionValue(controlSource)));
            }
        }
    }
}
