using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class ComicObjMovemet : MonoBehaviour
{
    [Header("���̃X�N���v�g�����Ă���I�u�W�F�N�g"),SerializeField]
    private GameObject _targetObj;

    [Header("�X�P�[���ύX���̒l")]
    [SerializeField]
    private Vector3 _endScalSise;
    private Vector3 _startScalSize;

    [Header("�ʒu�ύX���̒l�i���݂̈ʒu����ŏI�ʒu�܂ł̋����j")]
    [SerializeField]
    private Vector3 _endPos;
    private Vector3 _startPos;

    [Header("�㉺�ړ����̒l")]
    [SerializeField]
    private float _maxY;
    [SerializeField]
    private float _jumpUpDuration = 1.0f;
    [SerializeField]
    private float _jumpDownDuration = 1.0f;

    [Header("�W�����v������Ƃ��̉�"),SerializeField]
    private int _jumpTime = 3;

    [Header("�ŏI�I�ȓ����x")]
    [SerializeField]
    private float _endAlpha = 1.0f;

    [SerializeField]
    private float _duration = 2f;
    private float _elapsedTime = 0f;

    [Header("���̃I�u�W�F�N�g�̓�����SE���Ǘ����Ă�����́]"),SerializeField]
    private ComicSEController _seController;

    [Header("�g��k���̑傫��"),SerializeField]
    private float amplitude = 0.3f;
    [Header("�g��k���̑���"), SerializeField]
    private float frequency = 1.0f;
    private Vector3 initialScale = new Vector3(1, 1, 1);

    [Header("���̃I�u�W�F�N�g�̓����ɉ��������Ȃ���")]
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
    /// �ʒu�̕ω��E�����x�̕ω��E�傫���̕ω����s���R���[�`��
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
    /// �傫������ɕω�������R���[�`���i�g��k�����J��Ԃ��j
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
    /// �傫����ς���R���[�`���iRoopScaleChange���g������Ɏg���j
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
    /// �I�u�W�F�N�g�̏c�̑傫���̂݊g��k��������R���[�`��
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

            // Y�������X�P�[���ω�������
            transform.localScale = new Vector3(
                initialScale.x,                       // X�͌Œ�
                initialScale.y + scaleOffset,         // Y�����ω�
                initialScale.z                        // Z���Œ�
            );

            yield return null;
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�̑傫����ω�������R���[�`��
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
    /// �I�u�W�F�N�g�̈ʒu���ړ�������R���[�`��
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
    /// �I�u�W�F�N�g���㉺�Ɉړ�������R���[�`��
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
                // duretion�ň�ԏ�܂ŏオ��
                _targetObj.transform.position = Vector3.Lerp(_startPos, jumpUpPos, _elapsedTime / _jumpUpDuration);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ���̈ʒu�܂�duratio�̈ʒu�܂Ŗ߂�
            _targetObj.transform.position = jumpUpPos;

            _elapsedTime = 0f;

            while (_elapsedTime < _jumpDownDuration)
            {
                // duretion�ň�ԏ�܂ŏオ��
                _targetObj.transform.position = Vector3.Lerp(jumpUpPos, jumpDownPos, _elapsedTime / _jumpDownDuration);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            _targetObj.transform.position = jumpDownPos;
        }

    }

    /// <summary>
    /// �I�u�W�F�N�g�̓����x��ω�������R���[�`��
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
    /// �I�u�W�F�N�g���w��񐔉�]������R���[�`��
    /// </summary>
    /// <param name="rotateCount">��]�������</param>
    /// <returns></returns>
    public IEnumerator ObjRotater(int rotateCount)
    {
        float duration = 0.2f; // ��]�ɂ����鎞��
        Quaternion startRotation = transform.rotation;
        Quaternion leftRotation = Quaternion.Euler(0, 0, 10);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -10);

        for(int i =  0; i < rotateCount; i++)
        {
            // ���ɉ�]
            _seController.SEPlayer();
            yield return StartCoroutine(RotateOverTime(startRotation, leftRotation, duration));
            // ���̈ʒu�ɖ߂�
            yield return StartCoroutine(RotateOverTime(leftRotation, startRotation, duration));
            // �E�ɉ�]
            _seController.SEPlayer();
            yield return StartCoroutine(RotateOverTime(startRotation, rightRotation, duration));
            // ���̈ʒu�ɖ߂�
            yield return StartCoroutine(RotateOverTime(rightRotation, startRotation, duration));
        }


    }

    /// <summary>
    /// �I�u�W�F�N�g���̉�]������ʒu�܂ŉ�]������R���[�`��
    /// </summary>
    /// <param name="from">�ŏ��̉�]</param>
    /// <param name="to">�ŏI�I�ȉ�]</param>
    /// <param name="duration">��]����܂łɂ����鎞��</param>
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
    /// ����P�̂ŗ����֐��i�I�u�W�F�N�g�̓����Ɂj
    /// </summary>
    public void OnSound()
    {
        _seController.SEPlayer();
    }

}
