using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class StageSelect : MonoBehaviour
{
    [Header("キャラクターアイコン"), SerializeField]
    private GameObject _charIcon;
    [Header("キャラアニメーション"), SerializeField]
    private Animator _charIconAnim;
    public int _currentCharIconPos = 1;
    private bool _isIconMove = false;
    [Header("キャラアイコンが移動にかかる時間"), SerializeField]
    private float _charMoveDuration = 1f;

    [Header("ステージタイルのポジション"), SerializeField]
    private List<GameObject> _stageTilePos = new List<GameObject>();
    [Header("タイルアニメーション"), SerializeField]
    private List<Animator> _stageTileAnims = new List<Animator>();
    [Header("現在クリアしているステージ番号（例：ステージ1→1、ステージ2→2）")]
    public int _currentClearStageNum = 0;

    [Header("ステージのUI"), SerializeField]
    private List<GameObject> _stageUIs = new List<GameObject>();

    [SerializeField]
    private List<Animator> _starAnim = new List<Animator>();

    [SerializeField]
    private CharSEManager _charSEManager;

    public bool _showUI = false;


    public bool _iconMoved = false;
    private bool _stickRetrunedToNutral = true;

    Vector2 _playerInput;

    private void Start()
    {
        _charIcon.transform.position = _stageTilePos[0].transform.position;
        _currentCharIconPos = 0;

        // ステージ4,5のリスポーン情報をリセット
        PlayerPrefs.SetInt("HitRespawnPoint4", 0);
        PlayerPrefs.SetInt("HitRespawnPoint5", 0);


        // 現在どこまでステージをクリアしたか取得する
        GetClearStageNum();
        StartCoroutine(StageSelectLoadScene(_currentClearStageNum));

    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(StageSelectLoadScene(0));
            _currentClearStageNum = 0;
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(StageSelectLoadScene(1));
            _currentClearStageNum = 1;


        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(StageSelectLoadScene(2));
            _currentClearStageNum = 2;

        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(StageSelectLoadScene(3));
            _currentClearStageNum = 3;


        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(StageSelectLoadScene(4));
            _currentClearStageNum = 4;
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(StageSelectLoadScene(5));
            _currentClearStageNum = 5;
        }
    }

    public void OnCharIconMoveLeftStickHandle(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();

        // 移動中は入力拒否
        if (_playerInput.magnitude <= 0.5f)
        {
            _stickRetrunedToNutral = true;
            _iconMoved = false;
        }

        if (_playerInput != Vector2.zero && _stickRetrunedToNutral && !_iconMoved && !_isIconMove && !_showUI)
        {
            //_stickRetrunedToNutral = false;
            CharIconMoveCheck(_playerInput);
        }
    }

    public void OnDecisionStage(InputAction.CallbackContext context)
    {
        if (context.performed && _stickRetrunedToNutral && !_iconMoved && !_isIconMove)
        {
            Debug.Log($"_currentCharIconPos : {_currentClearStageNum} <= _currentClearNum : {_currentClearStageNum + 1} === {_currentCharIconPos <= _currentClearStageNum + 1}");
            if(_currentCharIconPos <= _currentClearStageNum + 1)
            {
                ShowStageUI();
            }
            else
            {
                Debug.Log("まだクリアしていないから遊べない");
            }
        }
    }

    // 現在のクリアステージを取得する
    public void GetClearStageNum()
    {
        _currentClearStageNum = PlayerPrefs.GetInt("ClearedStage");
        Debug.Log($"現在クリアしているステージは{_currentClearStageNum}までクリアしている");
    }

    public IEnumerator StageSelectLoadScene(int clearStage)
    {
        var set = StartCoroutine(SetStageSelectScene(clearStage));
        yield return set;
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(UnlockTile(clearStage));

    }

    // 現在クリアしたステージ数に応じてキャラアイコンの位置やステージのクリア状態にする
    public IEnumerator SetStageSelectScene(int clearStageNum)
    {
        if(clearStageNum != 0)
        {
            for (int i = 0; i < _stageTileAnims.Count -1; i++)
            {
                _stageTileAnims[i].SetBool("isGreen", false);
                _stageTileAnims[i].SetBool("isRed", false);
                _stageTileAnims[i].SetBool("isGrayChange", false);
                _stageTileAnims[i].SetBool("isRedChange", false);
            }
            yield return null;

            // すでにクリア済みなら緑にしておく(現在クリアしたステージの値が2以上)
            if (clearStageNum >= 2)
            {
                for (int i = clearStageNum - 1; i >= 0; i--)
                {
                    // 緑にする
                    _stageTileAnims[i].SetBool("isGreen", true);
                }
            }

            // ちょうどクリアしたマスは赤(現在クリアしたステージの値)
            _stageTileAnims[clearStageNum -1].SetBool("isRed", true);
        }
        
        // キャラの位置をクリアした位置にする
        _charIcon.transform.position = _stageTilePos[clearStageNum].transform.position;
        yield return null;
    }

    // プレイヤー入力方向から現在位置を元に移動先のタイルを決定
    public void CharIconMoveCheck(Vector3 plaeyrInput)
    {
        // 入力を取得
        Vector2 input = plaeyrInput;
        Debug.Log($"inptu.X : {input.x}     input.Y : {input.y}");
        int moveTile = 0;

        if (_currentCharIconPos == 0)
        {
            if (input.x >= 0.12f && input.x <= 0.96f && input.y >= -0.99f && input.y <= -0.25f)
            {
                // ステージ1マスに移動
                moveTile = 1;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,0));

            }

        }
        else if (_currentCharIconPos == 1)
        {
            if (input.x >= -0.99f && input.x <= -0.22f && input.y >= 0.082f && input.y <= 0.974f)
            {
                // タイトルマスに移動
                moveTile = 0;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile, 0));

            }
            else if (input.x >= -0.99f && input.x <= -0.66f && input.y >= -0.74f && input.y <= 0.003f)
            {
                moveTile = 2;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,1));


            }
        }
        else if (_currentCharIconPos == 2)
        {
            if (input.x >= 0.55f && input.x <= 0.68f && input.y >= -0.26f && input.y <= 0.72f)
            {
                // ステージ1に移動
                moveTile = 1;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile, 0));

            }
            else if (input.x >= -0.95f && input.x <= -0.82f && input.y >= -0.5f && input.y <= 0.3f)
            {
                moveTile = 3;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,2));
            }

        }
        else if (_currentCharIconPos == 3)
        {
            if (input.x >= 0.65f && input.x <= 0.9f && input.y >= -0.18f && input.y <= 0.75f)
            {
                // ステージ2移動
                moveTile = 2;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,1));

            }
            else if (input.x >= 0.16f && input.x <= 0.88f && input.y >= -0.98f && input.y <= -0.45f)
            {
                moveTile = 4;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,3));


            }

        }
        else if (_currentCharIconPos == 4)
        {
            if (input.x >= -0.88f && input.x <= -0.26f && input.y >= 0.46f && input.y <= 0.96f)
            {
                // ステージ3移動
                moveTile = 3;
                _currentCharIconPos = moveTile;
                Debug.Log("�X�e�[�W�^�C��3�Ɉړ�����");
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,2));

            }
            else if (input.x >= 0.81f && input.x <= 0.92f && input.y >= -0.58f && input.y <= 0.36f)
            {
                moveTile = 5;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,4));

            }

        }
        else if (_currentCharIconPos == 5)
        {
            if (input.x >= -0.92f && input.x <= -0.86f && input.y >= -0.50f && input.y <= 0.3f)
            {
                // ステージ4移動
                moveTile = 4;
                _currentCharIconPos = moveTile;
                _iconMoved = true;
                StartCoroutine(CharIconMovement(moveTile,3));
            }

        }

    }

    public IEnumerator CharIconMovement(int movePos, int moveTileNum)
    {
        Debug.Log($"movePos {movePos}   moveTileNum {moveTileNum}");
        _isIconMove = true;
        _iconMoved = true;
        _charIconAnim.SetBool("isMove", true);
        bool ischange = false;
        float elapsedTime = 0f;
        float duration = _charMoveDuration;
        Vector3 startPos = _charIcon.transform.localPosition;
        Vector3 endPos = _stageTilePos[movePos].transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _charIcon.transform.position = Vector3.Lerp(startPos, endPos, t);
            if(t > 0.9f && !ischange)
            {
                //到着したタイルを少し大きくして元の大きさにする
                _stageTileAnims[moveTileNum].SetBool("isLanding",true);
                ischange = true;
            }
            yield return null;
        }

        _charIconAnim.SetBool("isMove", false);
        _charIcon.transform.position = endPos;

        // 移動後到着時に音を出す
        yield return StartCoroutine(_charSEManager.LandingSEPlay());

        _currentCharIconPos = movePos;
        _isIconMove = false;
        _iconMoved = false;

        yield return new WaitForSeconds(1f);
        _stageTileAnims[moveTileNum].SetBool("isLanding", false);

        yield return null;
    }

    // 現在選択中のステージに対応したUIを表示
    private void ShowStageUI()
    {
        _showUI = true;
        switch (_currentCharIconPos)
        {
            case 0:
                _stageUIs[0].SetActive(true);
                break;
            case 1:
                _stageUIs[1].SetActive(true);
                break;
            case 2:
                _stageUIs[2].SetActive(true);
                break;
            case 3:
                _stageUIs[3].SetActive(true);
                break;
            case 4:
                _stageUIs[4].SetActive(true);
                break;
            case 5:
                _stageUIs[5].SetActive(true);
                break;
        }
    }


    // 指定されたステージ番号のタイルをアンロック（演出開始）
    public IEnumerator UnlockTile(int unlockNum)
    {
        // クリアしたステージのタイルのを赤から緑に変えるアニメーション開始
        if(unlockNum != 0)
        {
            _stageTileAnims[unlockNum - 1].SetBool("isRedChange", true);
            yield return new WaitForSeconds(1.5f);
            _stageTileAnims[unlockNum - 1].SetBool("isRedChange", false);

        }

        if (unlockNum == 5)
        {
            // 上のアニメーション終わったら次に遊べるマスを開放してキャラを移動させる
            _stageTileAnims[unlockNum].SetBool("isEnd", true);
            yield return new WaitForSeconds(1.5f);
            _stageTileAnims[unlockNum].SetBool("isEnd", false);

        }
        else
        {
            // 上のアニメーション終わったら次に遊べるマスを開放してキャラを移動させる
            _stageTileAnims[unlockNum].SetBool("isGrayChange", true);
            yield return new WaitForSeconds(1f);
            _stageTileAnims[unlockNum].SetBool("isGrayChange", false);
        }



    }
}