using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomToController : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;

    [Header("�Y�[�����鑬��"), SerializeField]
    private float _zoomSpeed = 1.0f;

    [Header("�ŏ��Y�[��"), SerializeField]
    private float _minZoom = 8.55f;

    [Header("�ő�Y�[��"),SerializeField]
    private float _maxZoom = 21.6f;

    [Header("���̒l���珟��ɃY�[���A�E�g����"),SerializeField]
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
        // ���͂ɂ��Y�[���C���E�A�E�g����
        if (_zoomInput != 0)
        {
            _mainCam.orthographicSize -= _zoomInput * _zoomSpeed * Time.deltaTime;
        }

        // �����Y�[���A�E�g����
        if (_mainCam.orthographicSize >= _aoutZoomValue && _mainCam.orthographicSize < _maxZoom)
        {
            _mainCam.orthographicSize += _zoomSpeed * Time.deltaTime;
        }

        // �J�����ʒu��ԁi�Y�[���l�ɉ����āj
        float t = Mathf.InverseLerp(_minZoom, _maxZoom, _mainCam.orthographicSize);
        _mainCam.transform.position = Vector3.Lerp(camStartPos.transform.position, camEndPos.transform.position, t);

        // �Y�[���͈͂𐧌�
        _mainCam.orthographicSize = Mathf.Clamp(_mainCam.orthographicSize, _minZoom, _maxZoom);

        // �ő�l�ɒB������Y�[���𖳌���
        if (_mainCam.orthographicSize >= _maxZoom)
        {
            _endingComicManager._canZoom = false;
        }
    }

    // �R���g���[���[�̓��͂��擾
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
