using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerControllManager : MonoBehaviour
{
    [Header("通常重力"), SerializeField]
    private float _normalGravityForce = 10.0f;
    [Header("最大重力"), SerializeField]
    private float _maxGravityForce = 30.0f;
    [Header("重力加速度"), SerializeField]
    private float _gravityAcceleration = 5.0f;
    [Header("地面判定の距離"), SerializeField]
    private float _rayDistance = 5.0f;
    [Header("地面のレイヤー"),SerializeField]
    private LayerMask _groundLayers;

    [Header("ブロックの設置場所表示オブジェクト"),SerializeField]
    private GameObject _viewPutObj;

    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    [Header("ステージの中心"),SerializeField]
    private Transform _fieldCenter;
    [Header("ステージ全体を写すためのOrthographicSize"),SerializeField]
    private float _orthographicSizeForStageMode;

    [Header("プレイモード表示UI"),SerializeField]
    private GameObject _charModeUI;
    [Header("プレイモード表示UIのAnimator"), SerializeField]
    private Animator _charModeUIAnim;
    [Header("ステージモード中のグリッド"), SerializeField]
    private GameObject _stageGrid;
    [Header("頭上の設置説明UI"), SerializeField]
    public GameObject _hintOverhead;

    [Header("ゲームクリアUI"), SerializeField]
    private GameObject _gameClearUI;
    [Header("ゲームオーバーUI"), SerializeField]
    private GameObject _gameOverUI;
    [Header("メニューUI"), SerializeField]
    private GameObject _menuUI;

    [Header("メインのCanvas"), SerializeField]
    private Canvas _mainCanvas;
    [Header("とったカギを移動させるときのUIオブジェクト"), SerializeField]
    private GameObject _spawnUIKeyObj;

    // 現在ステージモードならTrue
    public bool _isStageMode = true;
    // カメラのモードを変えている最中はTrue（変えている最中に変更させないように）
    public bool _changeCamMode = false;

    // プレイヤーのRigidbody2D
    public Rigidbody2D _playerRb;

    // プレイヤーが地面についているか
    public bool _isGround = false;

    // プレイヤーから下に伸ばしている線が三本それぞれは地面についているか
    public bool _isGround1 = false;
    public bool _isGround2 = false;
    public bool _isGround3 = false;

    //  ブロック設置する位置が表示されていたらTrue
    public bool _isView = false;
    // 歩いていたらTure
    private bool isWalking = false;
    // ジャンプしていたらTrue
    public bool _isJump = false;

    // 現在の重力
    private float _currentGravityForce;
    // カギを取得したらTrue
    public bool _haveKey = false;
    // 初めてカギを取得したらTrue
    public bool _firstHaveCookie = false;
    // ワッフルブロックを持っていたらTrue
    public bool _haveWaffle = false;
    // 4んだらTrue
    public bool _isDead = false;
    private bool _hasStartedDeath = false;

    // クリアしたらTrue
    public bool _isClear = false;
    // ゴールしたらTrue
    private bool _isGoal = false;
    // ポーズ中ならTure
    public bool _isPause = false;

    // 操作説明に使う
    public bool _firstMove =false;          // 初めて移動したらTrue
    public bool _firstJump =false;          // 初めてジャンプしたらTrue
    public bool _firstModeChage =false;     // 初めてモードを切り替えたらTrue
    public bool _firstStageMove =false;     // 初めてステージを切り替えたらTrue
    public bool _firstPutBlock = false;     // 初めてブロック設置したらTrue

    [Header("キャラのAnimot"),SerializeField] 
    public Animator _animor;
    [Header("移動系のスクリプト"),SerializeField] 
    private PlayerMovement _playerMovement;
    [Header("ブロック設置のスクリプト"),SerializeField] 
    private BlockPutManager _blockPutManager;
    [Header("ステージ移動のスクリプト"),SerializeField] 
    private TilemapMover tilemapMover; // TilemapMover.csを参照
    [Header("プレイヤーのSEなどを管理するスクリプト"),SerializeField]
    private PlayerSoundManager playerSoundManager;
    [Header("頭上の設置説明ヒントのオブジェクトを表示非表示するときに使うスクロール"),SerializeField] 
    private TextFadeController textFadeController;
    [Header("水に触れたときの制御スクリプト"),SerializeField]
    private WettingController _wettingController;

    private float footstepInterval = 0.5f; // 足音の間隔
    private float nextStepTime = 0f;

    [Header("ブロック回収エフェクトのイラスト"),SerializeField]
    private Image _fillCircle;
    // ボタンを長押ししている時間
    private float _buttonHoldTime = 0f;
    // ブロック回収するボタンを押していたらTrue
    public bool _isPressing = false;


    private Vector2 _controllerInput;

    [Header("リスポーンポイントに触れた後のリスポーン地点"), SerializeField]
    private GameObject _respawnPos;
    [Header("リスポーンポイントで使う"), SerializeField]
    private int _stageNum = 0;

    private int respawnNum = 0;

    [SerializeField]
    private GameObject _waffleObj;
    [SerializeField]
    private GameObject _keyObj;
    [SerializeField]
    private Image _keyUIObj;

    private TransitionManager _transitionManager;

    // 2025/08/22 16:00 菅原が追加
    [System.NonSerialized] public int gravityLocks = 0;

    private bool CanGameplay => _transitionManager == null ? true : _transitionManager._canGamePlay;


    private void Awake()
    {
        respawnNum = PlayerPrefs.GetInt($"HitRespawnPoint{_stageNum}");
        Debug.Log($"_stageNum: {_stageNum}");
        Debug.Log( respawnNum );
        if(respawnNum == 1)
        {
            transform.position = _respawnPos.transform.position;
            Debug.Log($"リスポーンポイント番号は{respawnNum}で何も持っていない");
        }
        else if(respawnNum == 2)
        {

            transform.position = _respawnPos.transform.position;
            _firstHaveCookie = true;
            _haveWaffle = true;
            // ワッフル消す
            _waffleObj.SetActive(false);
            Debug.Log($"リスポーンポイント番号は{respawnNum}でワッフルを持っている");

            
        }
        else if(respawnNum == 3)
        {

            transform.position = _respawnPos.transform.position;
            _haveKey = true;
            // カギ消す
            _keyObj.SetActive(false);
            Debug.Log($"リスポーンポイント番号は{respawnNum}でカギを持っている");
            // カギUIを持ってる状態にする
            _keyUIObj.color = Color.white;

        }
        else if(respawnNum == 4)
        {
            transform.position = _respawnPos.transform.position;
            _firstHaveCookie = true;
            _haveWaffle = true;
            _haveKey = true;
            // ワッフル消す
            _waffleObj.SetActive(false);
            // カギ消す
            _keyObj.SetActive(false);
            // カギUIを持ってる状態にする
            _keyUIObj.color = Color.white;
            Debug.Log($"リスポーンポイント番号は{respawnNum}でワッフルとカギをを持っている");
        }
    }

    void Start()
    {
        _charModeUIAnim = _charModeUI.GetComponent<Animator>();             // モード切替UIからAnimatorを取得
        _animor = GetComponent<Animator>();                                 // このオブジェクトのAnimorを取得
        _playerRb = GetComponent<Rigidbody2D>();                            // このオブジェクトRigidbody2Dを取得
        _playerRb.gravityScale = 0.0f;                                      // Rigidbody2Dの重力を0にする（自分で作った重力を使うため）
        _isStageMode = false;                                               // 最初はStageModeじゃないのでfalse
        _viewPutObj.gameObject.SetActive(false);                            // 設置ブロック表示を非表示にする
        _transitionManager = GameObject.Find("FadeCanvas").GetComponent<TransitionManager>();

        // ステージモードでなければ実行
        if (!_isStageMode)
        {
            _hintOverhead.gameObject.SetActive(true);                       // 頭上の設置ヒントを表示
            _stageGrid.gameObject.SetActive(false);                         // ステージのグリッド
        }
        else
        {
            _hintOverhead.gameObject.SetActive(false);                      // 頭上の設置ヒントを非表示  
            _stageGrid.gameObject.SetActive(true);                          // ステージのグリッド

        }
        // カメラのモードを切り替える
        StartCoroutine(ChangeCameraMode());
    }

    // Update is called once per frame
    void Update()
    {
        // 地面に接しているかのチェックを実行
        GroundCheck();

        // ステージモードでなく4んでもいない、クリアもしていない、ポーズ中でなければ実行
        if (!_isStageMode  && !_isDead && !_isClear && !_isPause)
        {
            // プレイヤーの移動処理を実行
            _playerMovement.MovePlayer(_playerRb,_controllerInput,_animor);

            // X軸移動 & 地面にいる
            if (_playerRb.velocity.x != 0.0f && _isGround)
            {
                if (!isWalking) // 初めて歩き始めた時
                {
                    playerSoundManager.PlayFootstepSE();
                    isWalking = true;
                    nextStepTime = Time.time + footstepInterval; // 次のステップ音の時間を設定
                }
                else if (Time.time >= nextStepTime) // 足音の間隔で再生
                {
                    playerSoundManager.PlayFootstepSE();
                    nextStepTime = Time.time + footstepInterval;
                }
            }
            else // 停止したらリセット
            {
                isWalking = false;
            }

        }

        // 横の移動がなければ実行
        if(_playerRb.velocity.magnitude == 0)
        {
            _animor.SetBool("isRun", false);        // 走りアニメーションをfalse
        }

        // 設置回収ボタンが押されていたら実行
        if (_isPressing)
        {
            // ボタンが押されている時間を計測
            _buttonHoldTime += Time.deltaTime;
            _fillCircle.fillAmount = _buttonHoldTime / 1f;
            if (_buttonHoldTime >= 1f)
            {
                _isPressing = false; // 一度だけ実行されるように
                _fillCircle.gameObject.SetActive(false);
                Debug.Log("1秒間押されたのでブロックを破壊します");
                playerSoundManager.DestroyBlockSE();        // SEを流す
                _blockPutManager.DestroyPutObject();        // ワッフルブロックを消す
            }
        }
    }

    private void FixedUpdate()
    {

        // クリアして地面に立っていて
        if (_isClear && _isGround && !_isGoal)
        {
            // _playerRbの縦の力を0にする
            Vector2 currentVelocity = _playerRb.velocity;
            _animor.SetBool("isJump", false);
            _playerRb.velocity = new Vector2(currentVelocity.x, 0);
            _playerRb.gravityScale = 0;
            _isGoal = true;

        }

        // 2025/08/22 16:00 菅原が変更
        else if (!_isDead && !_isClear)
        {
            // ロック中は重力を止める
            if (gravityLocks <= 0)
            {
                if (!_isGround)
                {
                    _currentGravityForce = Mathf.Clamp(
                        _currentGravityForce + _gravityAcceleration * Time.deltaTime,
                        _normalGravityForce, _maxGravityForce
                    );
                    _playerRb.AddForce(new Vector2(0, -_currentGravityForce), ForceMode2D.Force);
                }
                else
                {
                    _currentGravityForce = _normalGravityForce;
                }
            }
        }

    }

    /// <summary>
    /// 地面についているかの判定をする
    /// </summary>
    /// <returns></returns>
    private void GroundCheck()
    {
        _isGround1 = Physics2D.Raycast(transform.position, Vector2.down, _rayDistance, _groundLayers);
        _isGround2 = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0), Vector2.down, _rayDistance, _groundLayers);
        _isGround3 = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0), Vector2.down, _rayDistance, _groundLayers);

        if( _isGround1 || _isGround2 || _isGround3)
        {
            _isGround = true;
        }
        else if(!_isGround1 && !_isGround2 && !_isGround3)
        {
            _isGround = false;

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, _rayDistance * -1, 0));
        Gizmos.DrawRay(transform.position + new Vector3(0.2f,0), new Vector3(0, _rayDistance * -1, 0));
        Gizmos.DrawRay(transform.position + new Vector3(-0.2f, 0), new Vector3(0, _rayDistance * -1, 0));

    }

    /// <summary>
    /// カメラを操作モードに合わせて切り替える
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeCameraMode()
    {
        //　キャラモードなら実行
        if(!_isStageMode)
        {
            // カメラを切り替え中
            _changeCamMode = true;
            // 追跡するのをプレイヤーにする
            _virtualCamera.Follow = this.gameObject.transform;
            // カメラのOrthographicSizeを切り替える
            StartCoroutine(ChangeOrthographicSize(5.4f,0.3f));

            // オフセットを変更
            CinemachineFramingTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if(transposer != null )
            {
                transposer.m_TrackedObjectOffset = new Vector3(2.23f, 0, 0);
            }
            // カメラを切り替え終わり
            _changeCamMode = false;

        }
        else　       // ステージモードなら実行
        {
            // カメラを切り替え中
            _changeCamMode = true;
            // 追跡するのをステージの真ん中にする
            _virtualCamera.Follow = _fieldCenter;
            // オフセットをリセット
            CinemachineFramingTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if( transposer != null )
            {
                transposer.m_TrackedObjectOffset = new Vector3(0, 0, 0);
            }
            var waitFunc = StartCoroutine(ChangeOrthographicSize(_orthographicSizeForStageMode, 0.3f));
            yield return waitFunc;
            // カメラを切り替え終わり
            _changeCamMode = false;
        }

    }

    /// <summary>
    /// 指定した時間内で仮想カメラのOrthographicSizeを滑らかに変更する処理。
    /// カメラのズーム演出や視野変更の演出に使用。
    /// </summary>
    /// <param name="targetSize">変更後の目標サイズ（視野）</param>
    /// <param name="duration">変更にかける時間（秒）</param>
    /// <returns></returns>
    IEnumerator ChangeOrthographicSize(float targetSize, float duration)
    {
        float startSize = _virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _virtualCamera.m_Lens.OrthographicSize = targetSize; // 最後に目標サイズをセット
    }



    public IEnumerator WaterDeath()
    {
        // 頭上UI消す
        _hintOverhead.SetActive(false);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        //SpriteRenderer sp = this.GetComponent<SpriteRenderer>();
        //sp.sortingOrder = -1;
        collider.enabled = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector3 hitPos = transform.position;

        playerSoundManager.PlayWaterDeathSE();
        _animor.SetBool("isDeath", true);
        // 水しみこませる
        var wwetEffect = StartCoroutine(_wettingController.WettingRoutine(0.55f));
        yield return wwetEffect;

        yield return new WaitForSeconds(0.1f);
        Debug.Log("アニメーションを止めた");
        _animor.enabled = false;

        _playerRb.gravityScale = 0.8f;

        yield return new WaitUntil(() => transform.position.y < hitPos.y - 2f);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        _playerRb.gravityScale = 0f;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ViewGameOver());
    }

    private IEnumerator TogeDeath()
    {
        playerSoundManager.PlayDeathSE();
        _animor.SetBool("isHitToge", true);

        // Rigidbody2D を取得し、速度をゼロにして動きを完全に停止
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        yield return new WaitForSeconds(1f);
        StartCoroutine(ViewGameOver());
    }


    private IEnumerator WaterFloatRoutine(float duration)
    {
        float elapsed = 0f;

        float amplitude = 0.05f; // 揺れの大きさ
        float frequency = 2f;    // 揺れの速さ

        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetY = Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * amplitude;
            transform.position = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
            yield return null;
        }
    }


    #region ControllerInputs

    /// <summary>
    /// 移動する
    /// </summary>
    /// <param name="callbackContext"></param>
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        // コントローラの入力を受け取り
        _controllerInput = callbackContext.ReadValue<Vector2>();
        // 一度も移動していなかったら実行
        if (!_firstMove)
        {
            _firstMove = true;
        }

        // 移動しているときにブロックを設置する場所のインデックスが0でなければ0にする
        if (_blockPutManager.selectPosIndex != 0)
        {
            _blockPutManager.selectPosIndex = 0;

        }

        // ブロックを設置場所が表示されていたら非表示にする
        if (_viewPutObj.activeSelf)
        {
            _viewPutObj.SetActive(false);
            _isView = false;
        }
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    /// <param name="callbackContext"></param>
    public void OnJump(InputAction.CallbackContext callbackContext)
    {
      
        if (!CanGameplay) return;
        
        //ジャンプ処理    決めた高さ飛ぶ
        if (callbackContext.performed && _isGround && !_isStageMode && !_isPause && !_isClear && !_isDead)
        {
            _isJump = true;
            _animor.SetBool("isJump", _isJump);     // ジャンプアニメを開始
            playerSoundManager.PlayJumpSE();        // ジャンプSE開始
            _playerMovement.Jump(_playerRb);
            // 一度もジャンプしてなかったら実行
            if (!_firstJump)
            {
                _firstJump = true;
            }

            // ブロックを設置場所が表示されていたら非表示にする
            if (_viewPutObj.activeSelf)
            {
                _viewPutObj.SetActive(false);
                _isView = false;
            }
        }
    }

    // ブロックを置ける場所に置く
    public void OnBlockSet(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        if (callbackContext.performed && _isGround && _isView && _haveWaffle && !_isPause && !_isStageMode && _blockPutManager._canPutBlock)
        {
            _animor.SetTrigger("isPut");        // ブロックを置くアニメーション開始
            StartCoroutine(putBlcoker());
        }
        else if (callbackContext.performed && _isGround && _isView && _haveWaffle && !_isPause && !_isStageMode && !_blockPutManager._canPutBlock)   // ブロックが置けない時
        {
            
        }
    }

    // ブロックを消すボタン
    public void OnBlockDestroy(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        if (callbackContext.started && _isGround && !_isStageMode && !_haveWaffle)
        {
            // ボタンを押し始めた瞬間
            _fillCircle.gameObject.SetActive(true);
            _isPressing = true;
            _fillCircle.fillAmount = 0;
            _buttonHoldTime = 0f;
        }
        else if (callbackContext.canceled)
        {
            // ボタンを離した瞬間
            _isPressing = false;
            _fillCircle.fillAmount = 0;
            _buttonHoldTime = 0f;
        }

    }

    /// <summary>
    /// ブロックを設置するコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator putBlcoker()
    {
        if(_transitionManager._canGamePlay)
        {
            // アニメーションが終わってからブロックを設置
            yield return new WaitForSeconds(0.4f);
            playerSoundManager.PutBlockSE();
            _blockPutManager.BlockPut();         // ブロックを設置する処理
                                                        // 一度もブロックを設置していなければ実行
            if (!_firstPutBlock)
            {
                _firstPutBlock = true;
            }
            if (_viewPutObj.activeSelf)
            {
                _viewPutObj.SetActive(false);
                _isView = false;
            }
        }
        
    }


    // ブロックを置ける場所を表示する
    public void OnPutPosView(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        if (callbackContext.performed && !_isDead && !_isPause && !_isStageMode)
        {
            if (!_isClear && _haveWaffle)
            {
                Debug.Log("ブロック配置場所表示");
                _blockPutManager.BlockPutPosView(_viewPutObj);
                _isView = true;
            }

        }
        else
        {
            //Debug.Log("ブロック配置場所非表示");
        }
    }


    // 2025/05/02 11:03 菅原が変更
    public void OnModeChage(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        // モード変更ボタンが押されたときの処理
        if (callbackContext.performed && _isGround && !_changeCamMode && !_isDead && !_isClear)
        {
            _isStageMode = !_isStageMode;
            tilemapMover.isTilemapControlMode = !_isStageMode;
            playerSoundManager.ModeChangeSe();

            if (!_isStageMode)
            {
                _charModeUIAnim.SetBool("isCharMode", false);
                Debug.Log("キャラモードのUIがステージになる");
                if (!_firstHaveCookie)
                {
                    _hintOverhead.gameObject.SetActive(false);
                    _stageGrid.gameObject.SetActive(false);
                }
                else
                {
                    _hintOverhead.gameObject.SetActive(true);
                }

            }
            else
            {
                // キャラモードのアニメーションを動かすステージが見えるようにする
                _charModeUIAnim.SetBool("isCharMode", true);
                Debug.Log("キャラモードのUIがキャラモードになる");
                _hintOverhead.gameObject.SetActive(false);

            }
            _playerRb.velocity = Vector3.zero;

            StartCoroutine(ChangeCameraMode());
            if (!_firstModeChage)
            {
                _firstModeChage = true;
            }
            Debug.Log(_isStageMode ? "ステージ操作モード" : "プレイヤー操作モード");

        }
    }

    public void OnMenuView(InputAction.CallbackContext callbackContext)
    {
        if (!CanGameplay) return;

        if (callbackContext.performed)
        {
            if (!_menuUI.activeSelf)
            {
                _isPause = true;
                _menuUI.SetActive(true);
                Time.timeScale = 1f;
            }
            else
            {
                _isPause = false;
                _menuUI.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isJump = false;
            _animor.SetBool("isJump", _isJump);
        }

        if (collision.gameObject.CompareTag("PutGround"))
        {
            _isJump = false;
            _animor.SetBool("isJump", _isJump);
        }

    }

    private IEnumerator ViewGameOver()
    {
        //_animor.SetBool("isDeath", _isDead);
        _gameOverUI.gameObject.SetActive(true);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Item"))
        {
            if(collision.gameObject.name == "WaffleItem")
            {
                // キャラ頭上に操作ヒン表示
                _hintOverhead.gameObject.SetActive(true);
                // クッキー持った
                _haveWaffle = true;
                _firstHaveCookie = true;
                // とったときの音流す
                playerSoundManager.GetWaffelSE();
                // 頭上のヒントをフェードで表示
                textFadeController.PutHintView();

                Destroy(collision.gameObject);
            }

            if (collision.gameObject.name == "GoalKey")
            {
                _haveKey = true;
                playerSoundManager.GetKeySE();

                // ワールド座標をスクリーン座標に変換
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(collision.transform.position);

                // Canvas に設定されているカメラを取得
                Camera uiCamera = _mainCanvas.worldCamera;

                // スクリーン座標を Canvas のローカル座標に変換（カメラ指定）
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _mainCanvas.GetComponent<RectTransform>(),
                    screenPoint,
                    uiCamera,
                    out Vector2 localPoint
                );

                // UIオブジェクトを生成
                GameObject newUIKey = Instantiate(_spawnUIKeyObj, _mainCanvas.transform);
                RectTransform rectTransform = newUIKey.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = localPoint;

                // 元のオブジェクトを削除
                Destroy(collision.gameObject);
            }


        }

        // とげでも同じ処理になってるから変える
        if (collision.gameObject.CompareTag("Water") && !_hasStartedDeath && !_isGround)
        {
            _isDead = true;
            _hasStartedDeath = true;
            _animor.SetBool("isJump", false);
            if (_isDead)
            {
                StartCoroutine(WaterDeath());
            }
            
        }

        if (collision.gameObject.CompareTag("Toge") && !_hasStartedDeath && !_isGround)
        {
            _hasStartedDeath = true;
            _isDead = true;
            _animor.SetBool("isJump", false);
            if (_isDead)
            {
                StartCoroutine(TogeDeath());

            }
        }
    }
}
