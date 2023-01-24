using BepInEx.IL2CPP.Utils;
using DillyzRoleApi_Rewritten;
using System.Collections;
using UnityEngine;

namespace DillyzLegacyPack
{
    // carried over from ClassicRolePackage
    [Il2CppItem]
    class FlashOverlay : MonoBehaviour
    {
        public SpriteRenderer sprrend;

        private Color ogColor;
        private Color targetColor;
        private int a = 0;

        public void FadeToColor(float time, Color to)
        {
            FadeToColor(time, sprrend.color, to);
        }

        public void FadeToColor(float time, Color from, Color to)
        {
            a++;
            ogColor = from;
            targetColor = to;
            MonoBehaviourExtensions.StartCoroutine(this, FadeOut(time));
        }

        public void SetStaticColor(Color c)
        {
            a++;
            ogColor = targetColor = sprrend.color = c;
        }

        IEnumerator FadeOut(float time)
        {
            int b = a;
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
                if (a != b)
                    yield return null;
                float timeProgress = t / time;
                float curEase = 1f + ((timeProgress - 0.5f) / 4f);
                sprrend.color = Color.Lerp(ogColor, targetColor, timeProgress * curEase);
                yield return null;
            }
            yield break;
        }
    }
}
