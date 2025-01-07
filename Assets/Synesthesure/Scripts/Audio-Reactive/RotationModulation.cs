using UnityEngine;

namespace Synesthesure
{
    public class RotationModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes controlSource;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve;

        [Space]
        [SerializeField] Transform _transform;

        [Tooltip("Degrees per Second.")]
        [SerializeField] Vector3 speed = new Vector3(0, 0, 0);
        [SerializeField] float minSpeed = 0f;
        [SerializeField] float maxSpeed = 2f;
        float reaction;
        float deltaTime;

        void Awake()
        {
            if (_transform == null) _transform = transform;
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


        private void Update()
        {
            if (!audioSource.isPlaying) return;
            
            if (controlSource != AudioVisual.DynamicTypes.None)
            {
                deltaTime = Time.deltaTime;
                reaction = deltaTime * Mathf.Lerp(minSpeed, maxSpeed, responseCurve.Evaluate(AV.ReactionValue(controlSource)));
                _transform.rotation *= Quaternion.Euler(reaction * speed.x, reaction * speed.y, reaction * speed.z);
            }
        }

    }
}
