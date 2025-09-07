using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IceEatManager : MonoBehaviour
{
    [Header("メインカメラ"), SerializeField]
    private Camera _mainCamera;

    [Header("アイスを食べてるゲームオブジェ"),SerializeField]
    private GameObject _iceEatObj;

    [Header("アイス食べてるゲームオブジェクト"),SerializeField]
    private List<GameObject> _iceEatObjects;

    [Header("最後のカメラの位置"), SerializeField]
    private Vector3 _iceEatFirstPos;

    [Header("最後のカメラの位置"), SerializeField]
    private Vector3 _iceEatSecondPos;

    [Header("最後のカメラの位置"), SerializeField]
    private Vector3 _iceEatThirdPos;

    [Header("最後のカメラの位置"),SerializeField]
    private Vector3 _iceEatEndPos;
    [Header("カメラの移動速度"),SerializeField]
    private float _cameraSpeed;

    [SerializeField]
    private ComicSEController _comicSEController;

    public IEnumerator IceEatCorutine(float firstDuration,float secondDuration,float thirdDuration,float lastDuration)
    {
        Vector3 startPos = _mainCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < firstDuration)
        {
            elapsedTime += Time.deltaTime;
            _mainCamera.transform.position = Vector3.Lerp(startPos, _iceEatFirstPos, elapsedTime / firstDuration);
            yield return null;
        }
        StartCoroutine(AlphaChange(_iceEatObjects[0], 0.5f, 1f, 0f));
        StartCoroutine(AlphaChange(_iceEatObjects[1], 0.5f, 0f, 1f));

        startPos = _mainCamera.transform.position;
        elapsedTime = 0f;

        while (elapsedTime < secondDuration)
        {
            elapsedTime += Time.deltaTime;
            _mainCamera.transform.position = Vector3.Lerp(startPos, _iceEatSecondPos, elapsedTime / secondDuration);
            yield return null;
        }
        StartCoroutine(AlphaChange(_iceEatObjects[1], 0.5f, 1f, 0f));
        StartCoroutine(AlphaChange(_iceEatObjects[2], 0.5f, 0f, 1f));

        startPos = _mainCamera.transform.position;
        elapsedTime = 0f;

        while (elapsedTime < thirdDuration)
        {
            elapsedTime += Time.deltaTime;
            _mainCamera.transform.position = Vector3.Lerp(startPos, _iceEatThirdPos, elapsedTime / thirdDuration);
            yield return null;
        }
        StartCoroutine(AlphaChange(_iceEatObjects[2], 0.5f, 1f, 0f));
        StartCoroutine(AlphaChange(_iceEatObjects[3], 0.5f, 0f, 1f));

        startPos = _mainCamera.transform.position;
        elapsedTime = 0f;

        while (elapsedTime < lastDuration)
        {
            elapsedTime += Time.deltaTime;
            _mainCamera.transform.position = Vector3.Lerp(startPos, _iceEatEndPos, elapsedTime / lastDuration);
            yield return null;
        }

        yield return null;


        /*// 最初の1秒で_cameraSpeedを0→0.01に上げる
        float time = 0f;
        float accelDuration = 2f;
        while (time < accelDuration)
        {
            time += Time.deltaTime;
            _cameraSpeed = Mathf.Lerp(0f, 0.05f, time / accelDuration);

            _mainCamera.transform.position -= new Vector3(0, _cameraSpeed, 0);
            yield return null;
        }

        // アイス2になるまで移動しながら_cameraSpeedを0.005に減速
        while (_mainCamera.transform.position.y > -8.16f)
        {
            // 減速（0.01 → 0.005）
            _cameraSpeed = Mathf.Lerp(0.01f, 0.01f, Mathf.InverseLerp(-3.8f, -8.16f, _mainCamera.transform.position.y));
            _mainCamera.transform.position -= new Vector3(0, _cameraSpeed, 0);

            float currentY = _mainCamera.transform.position.y;

            if (Mathf.Abs(currentY - (-3.29f)) < 1.0f)
            {
                // α値を変えて表示非表示
                StartCoroutine(AlphaChange(_iceEatObjects[0], 0.5f, 1f, 0f));
                StartCoroutine(AlphaChange(_iceEatObjects[1], 0.5f, 0f, 1f));
                //_iceEatObjSR.sprite = _iceSprites[1];
            }
            else if (Mathf.Abs(currentY - (-8.96f)) < 1.0f)
            {
                StartCoroutine(AlphaChange(_iceEatObjects[1], 0.5f, 1f, 0f));
                StartCoroutine(AlphaChange(_iceEatObjects[2], 0.5f, 0f, 1f));
                //_iceEatObjSR.sprite = _iceSprites[2];
            }

            yield return null;
        }

        // 最後まで移動しながら_cameraSpeedを0まで減速
        while (_mainCamera.transform.position.y > iceEatEndPos.y)
        {
            // 減速（0.005 → 0.0）
            _cameraSpeed = Mathf.Lerp(0.007f, 0.008f, Mathf.InverseLerp(-8.16f, iceEatEndPos.y, _mainCamera.transform.position.y));
            _mainCamera.transform.position -= new Vector3(0, _cameraSpeed, 0);

            float currentY = _mainCamera.transform.position.y;

            if (Mathf.Abs(currentY - (-12.69f)) < 1.0f)
            {
                StartCoroutine(AlphaChange(_iceEatObjects[2], 0.5f, 1f, 0f));
                StartCoroutine(AlphaChange(_iceEatObjects[3], 0.5f, 0f, 1f));
                //_iceEatObjSR.sprite = _iceSprites[3];
            }

            yield return null;
        }

        // 位置をピッタリ調整
        _mainCamera.transform.position = new Vector3(
            _mainCamera.transform.position.x,
            iceEatEndPos.y,
            _mainCamera.transform.position.z
        );

        // 念のため_speedを0に
        _cameraSpeed = 0f;*/
    }

    public IEnumerator PlayMoveMentSERoop(float duration,float seInterval)
    {
        float elapsedTime = 0f;
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

    public IEnumerator AlphaChange(GameObject targetObj,float duration, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0;
        SpriteRenderer objSp = targetObj.GetComponent<SpriteRenderer>();
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


    private IEnumerator SpeedChange(float duration,float targetSpeed)
    {
        float elapsedTime = 0;
        float firstCameraSpeed = _cameraSpeed;


        while (elapsedTime > duration)
        {
            elapsedTime += Time.deltaTime;
            _cameraSpeed = Mathf.Lerp(firstCameraSpeed, targetSpeed, elapsedTime / duration);
            yield return null;
        }
        _cameraSpeed = targetSpeed;

    }
}
