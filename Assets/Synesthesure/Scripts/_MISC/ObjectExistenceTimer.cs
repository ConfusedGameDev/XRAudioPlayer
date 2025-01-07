using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Synesthesure
{
    public class ObjectExistenceTimer : MonoBehaviour
    {
        public float birthTime = 1f;
        public float lifeTime = 6f;
        public float deathTime = 3f;
        public AnimationCurve growthCurve;
        public AnimationCurve shrinkageCurve;
        public UnityEvent birth;
        public UnityEvent life;
        public UnityEvent death;

        void Start()
        {
            if (birthTime > 0f)
            {
                StartCoroutine(AnimateBirth());
            }
            else
            {
                StartCoroutine(WaitForLifeEnd());
            }
        }

        IEnumerator WaitForLifeEnd()
        {
            life.Invoke();
            yield return new WaitForSeconds(lifeTime);
            if (deathTime > 0f)
            {
                StartCoroutine(AnimateDeath());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        IEnumerator AnimateBirth()
        {
            Vector3 intialScale = gameObject.transform.localScale;
            birth.Invoke();
            float stopAnimation_time = 0f;
            while (stopAnimation_time < birthTime)
            {
                stopAnimation_time += Time.deltaTime;
                float scale = growthCurve.Evaluate(stopAnimation_time / birthTime);
                gameObject.transform.localScale = intialScale * scale;
                yield return null;
            }
            StartCoroutine(WaitForLifeEnd());
        }
        IEnumerator AnimateDeath()
        {
            Vector3 intialScale = gameObject.transform.localScale;
            death.Invoke();
            float stopAnimation_time = 0f;
            while (stopAnimation_time < deathTime)
            {
                stopAnimation_time += Time.deltaTime;
                float scale = shrinkageCurve.Evaluate(stopAnimation_time / deathTime);
                gameObject.transform.localScale = intialScale * scale;
                yield return null;
            }
            Destroy(gameObject);
        }

    }
}