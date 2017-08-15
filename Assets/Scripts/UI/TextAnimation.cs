using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected UnityEngine.UI.Text _animatedText;
    [SerializeField]
    protected AnimationCurve _fontSizeCurve;
    [SerializeField]
    protected RangeInt _fontSizeRange;
    [SerializeField]
    protected float _fontSizeAnimationSpeed;
    [Tooltip("Set this below 0 to make animation infinite")]
    public float AnimationDuration;
    
    #endregion

    #region Unity Methods

    protected void OnEnable()
    {
        StartCoroutine(ProcessFontSize());
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Methods

    protected IEnumerator ProcessFontSize()
    {
        float timer = 0.0f;
        float duration = AnimationDuration;
        while (true)
        {
            timer += Time.unscaledDeltaTime * _fontSizeAnimationSpeed;
            if(duration > 0)
            {
                duration -= Time.unscaledDeltaTime;
                if(duration < 0.0f)
                {
                    gameObject.SetActive(false);
                    yield return null;
                }
            }
            if (timer > 1.0f)
            {
                timer = 0.0f;
            }
            float sizeT = _fontSizeCurve.Evaluate(timer);

            _animatedText.fontSize = (int)Mathf.Lerp(_fontSizeRange.Min, _fontSizeRange.Max, sizeT);

            yield return null;
        }
    }

    #endregion
}
