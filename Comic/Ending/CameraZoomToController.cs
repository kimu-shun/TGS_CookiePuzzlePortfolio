using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomToController : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;

    [Header("ズームする速さ"), SerializeField]
    private float _zoomSpeed = 1.0f;

    [Header("最小ズーム"), SerializeField]
    private float _minZoom = 8.55f;

    [Header("最大ズーム"),SerializeField]
    private float _maxZoom = 21.6f;

    [Header("この値から勝手にズームアウトする"),SerializeField]
    private float _aoutZoomValue = 16f;

    [SerializeField]
    private GameObject camStartPos;
    [SerializeField]
    private GameObject camEndPos;

    [SerializeField]
    private GameObject _mainCharSprite;
    [SerializeField]
    private GameObject _subCharSprite;

    [SerializeField]
    private EndingComicManager _endingComicManager;

    [SerializeField]
    private ComicSEController _comicSEController;


    private float _zoomInput;

    public void SetZoomInput(float value)
    {
        _zoomInput = value;
    }

    public void ZoomLogic()
    {
        // 入力によるズームイン・アウト処理
        if (_zoomInput != 0)
        {
            _mainCam.orthographicSize -= _zoomInput * _zoomSpeed * Time.deltaTime;
        }

        // 自動ズームアウト処理
        if (_mainCam.orthographicSize >= _aoutZoomValue && _mainCam.orthographicSize < _maxZoom)
        {
            _mainCam.orthographicSize += _zoomSpeed * Time.deltaTime;
        }

        // カメラ位置補間（ズーム値に応じて）
        float t = Mathf.InverseLerp(_minZoom, _maxZoom, _mainCam.orthographicSize);
        _mainCam.transform.position = Vector3.Lerp(camStartPos.transform.position, camEndPos.transform.position, t);

        // ズーム範囲を制限
        _mainCam.orthographicSize = Mathf.Clamp(_mainCam.orthographicSize, _minZoom, _maxZoom);

        // 最大値に達したらズームを無効化
        if (_mainCam.orthographicSize >= _maxZoom)
        {
            _endingComicManager._canZoom = false;
        }
    }

    // コントローラーの入力を取得
    /*public void ZoomToController(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _zoomInput = input.y;
    }*/

    public void MainCharSpriteChange()
    {
        StartCoroutine(AlphaChange(1f,_mainCharSprite));
    }

    public void SubCharSpriteChange()
    {
        StartCoroutine(AlphaChange(1f, _subCharSprite));
    }

    public IEnumerator AlphaChange(float duration, GameObject targetObj)
    {
        float elapsedTime = 0f;
        SpriteRenderer targetSR = targetObj.GetComponent<SpriteRenderer>();
        Color objColor = targetSR.color;
        float startAlpha = objColor.a;


        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            objColor.a = Mathf.Lerp(startAlpha, 0, t);
            targetSR.color = objColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objColor.a = 0;
        targetSR.color = objColor;

    }

    public IEnumerator PlayMoveMentSE()
    {
        _comicSEController.SEPlayer();
        yield return null;
    }
}
