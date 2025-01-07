using System.Collections;
using UnityEngine;

namespace Synesthesure
{
    public class CameraShake : MonoBehaviour
    {
        public float magnitude = .2f;
        public bool zShake; // shake on the X, and the Y or the Z axis (not both)
        bool isShaking;
        [HideInInspector]
        public bool shakeConstantly;  // used to remotely stop ShakeConstantly coroutine

        // Shake on Command
        public void Shake(float duration = .1f)
        {
            StartCoroutine(ShakeNow(duration));
        }
        IEnumerator ShakeNow(float duration = .1f)
        {
            if (!isShaking)
            {
                isShaking = true;
                Vector3 originalPosition = transform.localPosition;
                float elapasedTime = 0f;
                while (elapasedTime < duration)
                {
                    float x = Random.Range(-1f, 1f) * magnitude;
                    float y = Random.Range(-1f, 1f) * magnitude;
                    if (!zShake)
                    {
                        transform.localPosition = new Vector3(x, y, originalPosition.z);
                    }
                    else
                    {
                        transform.localPosition = new Vector3(x, originalPosition.y, y);
                    }
                    elapasedTime += Time.deltaTime;
                    yield return null;
                }
                transform.localPosition = originalPosition;
                isShaking = false;
            }
        }

        // Shake on Constantly
        public void ShakeConstantly()
        {
            StartCoroutine(ShakeConstantlyNow());
        }
        IEnumerator ShakeConstantlyNow()
        {
            shakeConstantly = true;
            Vector3 originalPosition = transform.localPosition;
            while (shakeConstantly)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                if (!zShake)
                {
                    transform.localPosition = new Vector3(x, y, originalPosition.z);
                }
                else
                {
                    transform.localPosition = new Vector3(x, originalPosition.y, y);
                }
                yield return null;
            }
            transform.localPosition = originalPosition;
        }
        public void StopShaking()
        {
            shakeConstantly = false;
        }
    }
}