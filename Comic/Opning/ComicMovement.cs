using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicMovement : MonoBehaviour
{
    [Header("コミックのコマ"),SerializeField]
    private GameObject _comicPanel;
    [Header("コマをが最終的に移動する位置"),SerializeField]
    private GameObject _movePos;
    [Header("コマが移動するまでの時間(s)"),SerializeField]
    private float _duration = 1.0f;
    private float _elapsedTime = 0.0f;

    private Vector3 _startPos;


    [Header("そのコマのSEを管理しているもの"),SerializeField]
    private ComicSEController _seController;

    private void Start()
    {
        _startPos = transform.position;
        _seController = GetComponent<ComicSEController>();
    }

    public IEnumerator PanelMove()
    {
        _elapsedTime = 0f;
        // コマが移動するときの音を流す
        _seController.SEPlayer();

        // 移動時間になるまで実行する
        while (_elapsedTime < _duration)
        {
            // Lerpで徐々にPosを移動させる
            _comicPanel.transform.position = Vector3.Lerp(_startPos, _movePos.transform.position, _elapsedTime / _duration);
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Posを最終位置にする
        _comicPanel.transform.position = _movePos.transform.position;
        yield return null;
    }
}
