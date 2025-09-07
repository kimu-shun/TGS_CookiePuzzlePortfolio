using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EndingComicObjMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject _parObj;
    [SerializeField]
    private Animator _objAnim;
    [SerializeField]
    private GameObject _thiObj;

    [Header("オブジェクトの位置変更関係")]
    [Header("オブジェクトの最終位置までの距離"), SerializeField]
    private Vector3 _endPos;
    private Vector3 _startPos;

    [Header("オブジェクトの大きさ変更関係")]
    [Header("オブジェクトの最終的な大きさ"),SerializeField]
    private Vector3 _endScale = Vector3.one;
    [Header("オブジェクト拡縮の際の拡縮の速さ"), SerializeField]
    private float frequency = 1.0f;
    [Header("オブジェクト拡縮の際の拡縮の大きさ"),SerializeField]
    private float amplitude = 1.0f;
    private Vector3 _startScale;

    [SerializeField]
    private ComicSEController _comicSEController;

    private void Start()
    {
        _thiObj = this.gameObject;
        _objAnim = this.GetComponent<Animator>();
        _startScale = this.transform.localScale;
        _startPos = this.transform.localPosition;
    }

    public IEnumerator AnimChange(string animName)
    {
        _objAnim.SetBool(animName, true);
        yield return null;
    }

    public IEnumerator AnimChageReturn(string animName,float duration)
    {
        _objAnim.SetBool(animName, true);
        yield return new WaitForSeconds(duration);
        _objAnim.SetBool(animName, false);

    }

    public IEnumerator AnimAndScalePosChange(string animName,float duration)
    {
        _objAnim.SetBool(animName, true);

        float elapsedTime = 0;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + _endPos;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // 補間（必要なら SmoothStep に変更）
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            Vector3 scale = Vector3.Lerp(_startScale, _endScale, t);

            transform.localPosition = pos;
            transform.localScale = scale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;
        transform.localScale = _endScale;

        // 今動いているアニメーションを止める
        _objAnim.enabled = false;
        yield return null;
    }

    public IEnumerator PosAndScaleChange(float duration)
    {
        StartCoroutine(SmoothPosChange(duration));
        var scale = StartCoroutine(ScaleChange(duration));
        yield return scale;
        yield return null;
    }

    public IEnumerator AlphaAndPosChange(float duration, float startAlpha, float endAlpha)
    {
        StartCoroutine(AlphaChange(duration, startAlpha, endAlpha));
        yield return StartCoroutine(PosChange(duration));
    }


    public IEnumerator AlphaAndPosAndScaleChange(float duration,float startAlpha,float endAlpha)
    {
        StartCoroutine(AlphaChange(duration, startAlpha, endAlpha));
        StartCoroutine(PosChange(duration));
        var scale = StartCoroutine(ScaleChange(duration));
        yield return scale;
    }

    public IEnumerator PosChange(float duration)
    {
        float elapsedTime = 0;
        Vector3 startPos = gameObject.transform.localPosition;
        Vector3 endPos = startPos + _endPos;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localPosition = endPos;
        yield return null;
    }

    public IEnumerator SmoothPosChange(float duration)
    {
        float elapsedTime = 0;
        Vector3 startPos = gameObject.transform.localPosition;
        Vector3 endPos = startPos + _endPos;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float easedT = Mathf.SmoothStep(0, 1, t);
            gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localPosition = endPos;
        yield return null;
    }

    public IEnumerator ScaleChange(float duration)
    {
        float elapsedTime = 0;
        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            gameObject.transform.localScale = Vector3.Lerp(_startScale, _endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localScale = _endScale;
        yield return null;
    }

    public IEnumerator RoopScaleChange()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            float scaleOffset = Mathf.Sin(timer * frequency) * amplitude;
            transform.localScale = _startScale + Vector3.one * scaleOffset;
            yield return null;
        }
    }


    public IEnumerator RoopVerticalMove()
    {
        float timer = 0f;
        Vector3 startPos = transform.localPosition;

        while (true)
        {
            timer += Time.deltaTime;
            float yOffset = Mathf.Sin(timer * frequency) * amplitude;
            transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);
            yield return null;
        }
    }

    public IEnumerator AlphaChange(float duration,float startAlpha,float endAlpha)
    {
        float elapsedTime = 0;
        SpriteRenderer objSp = _thiObj.GetComponent<SpriteRenderer>();
        Color objColor = objSp.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            objColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            objSp.color = objColor;
            yield return null;
        }

        objColor.a = endAlpha;
        objSp.color = objColor;
    }
    
    public IEnumerator PlayMoveMentSE()
    {
        _comicSEController.SEPlayer();
        yield return null;
    }

    public IEnumerator PlayMovementSEFadeStop(float delaySecond, float fadeDuration)
    {
        _comicSEController.SEPlayer();
        yield return new WaitForSeconds(delaySecond);

        AudioSource audioSource = _comicSEController._comicSESource;
        float startVolume = audioSource.volume;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0,elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public IEnumerator PlayMoveMentSERoop(float duration)
    {
        float elapsedTime = 0f;
        float seInterval = 0.5f; // SEを鳴らす間隔（秒）
        float nextSETime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= nextSETime)
            {
                _comicSEController.SEPlayer();
                nextSETime += seInterval;
            }

            yield return null;
        }
    }

}
