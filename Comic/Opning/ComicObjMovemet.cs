using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class ComicObjMovemet : MonoBehaviour
{
    [Header("このスクリプトがついているオブジェクト"),SerializeField]
    private GameObject _targetObj;

    [Header("スケール変更時の値")]
    [SerializeField]
    private Vector3 _endScalSise;
    private Vector3 _startScalSize;

    [Header("位置変更時の値（現在の位置から最終位置までの距離）")]
    [SerializeField]
    private Vector3 _endPos;
    private Vector3 _startPos;

    [Header("上下移動時の値")]
    [SerializeField]
    private float _maxY;
    [SerializeField]
    private float _jumpUpDuration = 1.0f;
    [SerializeField]
    private float _jumpDownDuration = 1.0f;

    [Header("ジャンプさせるときの回数"),SerializeField]
    private int _jumpTime = 3;

    [Header("最終的な透明度")]
    [SerializeField]
    private float _endAlpha = 1.0f;

    [SerializeField]
    private float _duration = 2f;
    private float _elapsedTime = 0f;

    [Header("このオブジェクトの動きのSEを管理しているもの‐"),SerializeField]
    private ComicSEController _seController;

    [Header("拡大縮小の大きさ"),SerializeField]
    private float amplitude = 0.3f;
    [Header("拡大縮小の速さ"), SerializeField]
    private float frequency = 1.0f;
    private Vector3 initialScale = new Vector3(1, 1, 1);

    [Header("このオブジェクトの動きに音が歩かないか")]
    private bool _isSound = true;

    private void Start()
    {
        _startPos = transform.position;
        _startScalSize = _targetObj.transform.localScale;
    }

    public IEnumerator SubJump()
    {
        StartCoroutine(ObjBounce());
        yield return null;
    }

    /// <summary>
    /// 位置の変化・透明度の変化・大きさの変化を行うコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator AlphaScaleMove()
    {
        StartCoroutine(PosChange());
        StartCoroutine(AlphaChange());
        var end = StartCoroutine(ScaleChange());
        yield return end;

    }

    /// <summary>
    /// 大きさを常に変化させるコルーチン（拡大縮小を繰り返す）
    /// </summary>
    /// <returns></returns>
    public IEnumerator RoopScaleChange()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            float scaleOffset = Mathf.Sin(timer * frequency) * amplitude;
            transform.localScale = initialScale + Vector3.one * scaleOffset;
            yield return null;
        }
    }

    /// <summary>
    /// 大きさを変えるコルーチン（RoopScaleChangeを使った後に使う）
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="amplitude"></param>
    /// <returns></returns>
    public IEnumerator CustomRoopScaleChange(float frequency,float amplitude)
    {
        float timer = 0f;
        initialScale = transform.localScale;

        while (true)
        {
            timer += Time.deltaTime;
            float scaleOffset = Mathf.Sin(timer * frequency) * amplitude;
            transform.localScale = initialScale + Vector3.one * scaleOffset;
            yield return null;
        }
    }

    /// <summary>
    /// オブジェクトの縦の大きさのみ拡大縮小させるコルーチン
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="amplitude"></param>
    /// <returns></returns>
    public IEnumerator RoopScaleYChange(float frequency, float amplitude)
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            float scaleOffset = Mathf.Sin(timer * frequency) * amplitude;

            // Y軸だけスケール変化させる
            transform.localScale = new Vector3(
                initialScale.x,                       // Xは固定
                initialScale.y + scaleOffset,         // Yだけ変化
                initialScale.z                        // Zも固定
            );

            yield return null;
        }
    }

    /// <summary>
    /// オブジェクトの大きさを変化させるコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator ScaleChange()
    {
        _startScalSize = _targetObj.transform.localScale;
        float elapsedTime = 0f;
        if(_seController != null && _isSound)
        {
            _seController.SEPlayer();

        }
        while (elapsedTime < _duration)
        {
            _targetObj.transform.localScale = Vector3.Lerp(_startScalSize, _endScalSise, elapsedTime / _duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _targetObj.transform.localScale = _endScalSise;

    }

    /// <summary>
    /// オブジェクトの位置を移動させるコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator PosChange()
    {
        _startPos = transform.position;
        float elapsedTime = 0f;
        Vector3 endPos = transform.position + _endPos;
        if (_seController != null && _isSound)
        {
            _seController.SEPlayer();
        }

        while (elapsedTime < _duration)
        {
            _targetObj.transform.position = Vector3.Lerp(_startPos, endPos, elapsedTime / _duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _targetObj.transform.position = endPos;
        yield return null;
    }

    /// <summary>
    /// オブジェクトを上下に移動させるコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator ObjBounce()
    {
        _startPos = transform.position;
        for (int i = 0; i < _jumpTime; i++)
        {
            _elapsedTime = 0f;
            Vector3 jumpDownPos = _targetObj.transform.position;
            Vector3 jumpUpPos = _targetObj.transform.position + new Vector3(0, _maxY, 0);
            if (_seController != null && _isSound)
            {
                _seController.SEPlayer();

            }
            while (_elapsedTime < _jumpUpDuration)
            {
                // duretionで一番上まで上がる
                _targetObj.transform.position = Vector3.Lerp(_startPos, jumpUpPos, _elapsedTime / _jumpUpDuration);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 元の位置までduratioの位置まで戻る
            _targetObj.transform.position = jumpUpPos;

            _elapsedTime = 0f;

            while (_elapsedTime < _jumpDownDuration)
            {
                // duretionで一番上まで上がる
                _targetObj.transform.position = Vector3.Lerp(jumpUpPos, jumpDownPos, _elapsedTime / _jumpDownDuration);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            _targetObj.transform.position = jumpDownPos;
        }

    }

    /// <summary>
    /// オブジェクトの透明度を変化させるコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator AlphaChange()
    {
        float elapsedTime = 0f;
        SpriteRenderer objSp = _targetObj.GetComponent<SpriteRenderer>();
        Color objColor = objSp.color;
        if (_seController != null && _isSound)
        {
            _seController.SEPlayer();

        }
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            objColor.a = Mathf.Lerp(0, _endAlpha, elapsedTime / _duration);
            objSp.color = objColor;
            yield return null;
        }
        objColor.a = _endAlpha;
        objSp.color = objColor;
    }

    /// <summary>
    /// オブジェクトを指定回数回転させるコルーチン
    /// </summary>
    /// <param name="rotateCount">回転させる回数</param>
    /// <returns></returns>
    public IEnumerator ObjRotater(int rotateCount)
    {
        float duration = 0.2f; // 回転にかかる時間
        Quaternion startRotation = transform.rotation;
        Quaternion leftRotation = Quaternion.Euler(0, 0, 10);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -10);

        for(int i =  0; i < rotateCount; i++)
        {
            // 左に回転
            _seController.SEPlayer();
            yield return StartCoroutine(RotateOverTime(startRotation, leftRotation, duration));
            // 元の位置に戻る
            yield return StartCoroutine(RotateOverTime(leftRotation, startRotation, duration));
            // 右に回転
            _seController.SEPlayer();
            yield return StartCoroutine(RotateOverTime(startRotation, rightRotation, duration));
            // 元の位置に戻る
            yield return StartCoroutine(RotateOverTime(rightRotation, startRotation, duration));
        }


    }

    /// <summary>
    /// オブジェクトをの回転をある位置まで回転させるコルーチン
    /// </summary>
    /// <param name="from">最初の回転</param>
    /// <param name="to">最終的な回転</param>
    /// <param name="duration">回転するまでにかかる時間</param>
    /// <returns></returns>
    public IEnumerator RotateOverTime(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
    }

    /// <summary>
    /// 音を単体で流す関数（オブジェクトの動きに）
    /// </summary>
    public void OnSound()
    {
        _seController.SEPlayer();
    }

}
