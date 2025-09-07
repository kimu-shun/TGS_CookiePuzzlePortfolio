using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicMovement : MonoBehaviour
{
    [Header("�R�~�b�N�̃R�}"),SerializeField]
    private GameObject _comicPanel;
    [Header("�R�}�����ŏI�I�Ɉړ�����ʒu"),SerializeField]
    private GameObject _movePos;
    [Header("�R�}���ړ�����܂ł̎���(s)"),SerializeField]
    private float _duration = 1.0f;
    private float _elapsedTime = 0.0f;

    private Vector3 _startPos;


    [Header("���̃R�}��SE���Ǘ����Ă������"),SerializeField]
    private ComicSEController _seController;

    private void Start()
    {
        _startPos = transform.position;
        _seController = GetComponent<ComicSEController>();
    }

    public IEnumerator PanelMove()
    {
        _elapsedTime = 0f;
        // �R�}���ړ�����Ƃ��̉��𗬂�
        _seController.SEPlayer();

        // �ړ����ԂɂȂ�܂Ŏ��s����
        while (_elapsedTime < _duration)
        {
            // Lerp�ŏ��X��Pos���ړ�������
            _comicPanel.transform.position = Vector3.Lerp(_startPos, _movePos.transform.position, _elapsedTime / _duration);
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Pos���ŏI�ʒu�ɂ���
        _comicPanel.transform.position = _movePos.transform.position;
        yield return null;
    }
}
