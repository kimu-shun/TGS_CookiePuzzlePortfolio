using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndingComicManager : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;
    [Header("1ページ目"),SerializeField]
    private GameObject _firstPage;
    [SerializeField]
    private List<GameObject> _comicComaPanelsObj = new List<GameObject>();
    [Header("コミックのコマ"),SerializeField]
    private List<ComicMovement> _comicComaPanels = new List<ComicMovement>();

    [Header("1コマ目のサブキャラ"),SerializeField]
    private EndingComicObjMovement _1comaSubChar;

    [Header("1コマ目のメインキャラ"), SerializeField]
    private EndingComicObjMovement _1comaMainChar;

    [Header("2コマ目のメインキャラ"), SerializeField]
    private EndingComicObjMovement _2comaMainChar;

    [Header("2コマ目のびっくりマーク"), SerializeField]
    private EndingComicObjMovement _2comaDiscovery;

    [Header("2コマ目の光"), SerializeField]
    private EndingComicObjMovement _2comaShiny;

    [Header("3コマ目の背景"), SerializeField]
    private EndingComicObjMovement _3leftBG;
    [Header("3コマ目の光"), SerializeField]
    private EndingComicObjMovement[] _3comaStars;
    [Header("3コマ目の左の壁"), SerializeField]
    private EndingComicObjMovement _3leftWall;
    [Header("3コマ目の右の壁"),SerializeField]
    private EndingComicObjMovement _3rightWall;
    [Header("3コマ目宝箱"),SerializeField]
    private EndingComicObjMovement _3chest;

    [Header("4コマ目のメインキャラ"), SerializeField]
    private EndingComicObjMovement _4comaMainChar;

    [Header("4コマ目の宝箱"), SerializeField]
    private EndingComicObjMovement _4comaChest;


    [Header("4コマ目の雲"), SerializeField]
    private List<EndingComicObjMovement> _clouds = new List<EndingComicObjMovement>();
    [SerializeField]
    private GameObject _cloudParent;

    [Header("2ページ目"), SerializeField]
    private GameObject _2page;

    [SerializeField]
    private GameObject _stick;

    [Header("2ページ目のズームを管理するコード"),SerializeField]
    private CameraZoomToController _cameraZoomToController;
    public bool _canZoom = false;

    [Header("2ページ目で透明にするイラスト"), SerializeField]
    private EndingComicObjMovement _2pageAlphaChangeObj;

    [Header("2ページ目のメインキャラ"), SerializeField]
    private EndingComicObjMovement _2pageMainChar;

    [Header("2ページ目のサブキャラ"), SerializeField]
    private EndingComicObjMovement _2pageSubChar;

    [SerializeField]
    private GameObject _3page;

    [Header("3ページ目で透明にするイラスト"), SerializeField]
    private EndingComicObjMovement _3pageAlphaChangeObj;

    [Header("3コマ目のアイスのスクリプト"),SerializeField]
    private IceEatManager _iceEatManager;

    [SerializeField]
    private GameObject _4page;

    [Header("4コマ目の雲"),SerializeField]
    private EndingComicObjMovement[] _4pageClouds;

    [Header("4コマ目の星"),SerializeField]
    private EndingComicObjMovement[] _4pageStars;
    [Header("4コマ目の息"),SerializeField]
    private EndingComicObjMovement[] _4pageBreath;

    [SerializeField]
    private GameObject _spkipButton;

    [SerializeField]
    private ComicSEController _secontroller;

    private BGMManager _bgmManager;
    private FadeManager _fadeManager;

    public bool isPaused = false;
    public bool _canMoveScene = false;

    [SerializeField]
    private AudioClip _iceEatBGM;
    [SerializeField]
    private AudioClip _endingBGm;


    [Header("コマを暗くするときの色")]
    [SerializeField]
    private Color _firstHideColor;
    [SerializeField]
    private Color _secondHideColor;
    [SerializeField]
    private Color _thirdHideColor;

    private void Start()
    {
        _fadeManager = GameObject.Find("FadeCanvas").GetComponent<FadeManager>();
        _bgmManager = GameObject.Find("BGMManager").GetComponent<BGMManager>();
        StartCoroutine(EndComicManger());
    }

    private void Update()
    {
        if (Gamepad.current != null && _canZoom && !isPaused)
        {
            Vector2 input = Gamepad.current.leftStick.ReadValue();
            _cameraZoomToController.SetZoomInput(input.y);
            _cameraZoomToController.ZoomLogic();

        }

        if(Gamepad.current != null && _canMoveScene && !isPaused)
        {
            if (Gamepad.current.bButton.wasPressedThisFrame)
            {
                Debug.Log("〇ボタンが押されました！");
                // ここでFadeMangaer使ってScene遷移する
                StartCoroutine(_fadeManager.FadeIn("TitleScene", 1f));
            }

        }
    }

    public IEnumerator EndComicManger()
    {
        //ーーー1ページ目ーーー//
        // 1コマ目を指定位置まで移動させる
        yield return new WaitUntil(() => isPaused == false); // 一時停止
        yield return new WaitForSeconds(1f);
        // 1コマ目を移動させる
        var FirstComic = StartCoroutine(_comicComaPanels[0].PanelMove());
        yield return FirstComic;
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 1コマ目のアニメーションを開始
        // サブキャラを移動させる
        var subCharMove = StartCoroutine(_1comaSubChar.AnimAndScalePosChange("isWalk", 3f));
        // 歩く音を出す
        StartCoroutine(_1comaSubChar.PlayMoveMentSERoop(3f));
        // メインキャラをきょろきょろさせる
        var mainCharCareful = StartCoroutine(_1comaMainChar.AnimAndScalePosChange("isCareful", 3f));

        yield return subCharMove;
        yield return mainCharCareful;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.2f);
        // 2コマ目を指定位置まで移動
        // 1コマ目の色をを時間経過で暗くする
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _firstHideColor, 1f));
        // 2コマ目を移動させる
        var secondComic = StartCoroutine(_comicComaPanels[1].PanelMove());
        yield return secondComic;
        yield return new WaitForSeconds(0.5f);

        // 2コマ目のキャラを大きくして元の大きさに戻す
        var mainCharScale = StartCoroutine(_2comaMainChar.AnimChange("isSuprised"));
        //ビックリマークも同じタイミングで大きき差を変える
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(_2comaDiscovery.AnimChange("isChange"));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(_2comaShiny.AnimChange("isChange"));

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(2.5f);


        // 3コマ目を指定位置まで移動
        // 1,2コマ目の色を時間経過で暗くする
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _secondHideColor, 1.5f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[1], _firstHideColor, 1.5f));
        // 3コマ目を移動じゃなくてα値を0~1にする
        var thirdComic = StartCoroutine(ChangeComaColor(_comicComaPanelsObj[2],Color.white,1.5f));
        // 宝箱の周りのキラキラを拡縮させる
        StartCoroutine(_3comaStars[0].RoopScaleChange());
        StartCoroutine(_3comaStars[3].RoopScaleChange());
        StartCoroutine(_3comaStars[2].RoopScaleChange());
        StartCoroutine(_3comaStars[1].RoopScaleChange());
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return thirdComic;
        yield return new WaitForSeconds(1f);

        // 3コマ目全部大きくする
        StartCoroutine(_3leftBG.ScaleChange(2.5f));
        // 洞窟の外壁を移動させて洞窟内に入っている感じを演出
        StartCoroutine(_3leftWall.PlayMoveMentSE());
        var leftwall = StartCoroutine(_3leftWall.SmoothPosChange(2f));
        var rightWall = StartCoroutine(_3rightWall.SmoothPosChange(2f));

        
        // カメラをズームさせる
        //var zoom = StartCoroutine(CameraZoom(2.5f, 4.07f));

        //yield return new WaitForSeconds(1f);
        // 宝箱の位置と大きさを変える
        var chest = StartCoroutine(_3chest.PosAndScaleChange(2.5f));

        yield return new WaitUntil(() => isPaused == false); // 一時停止
        yield return chest;
        yield return new WaitForSeconds(2.0f);

        // 3コマ目の色を時間経過で暗くする
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _thirdHideColor, 1f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[1], _secondHideColor, 1f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[2], _firstHideColor, 1f));
        // 4コマ目を指定位置まで移動
        // 4コマ目を移動させる
        var fourthcomic = StartCoroutine(_comicComaPanels[3].PanelMove());
        // BGMの音量を下げる
        StartCoroutine(BGMQuieter(0.7f, 0.15f));

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return fourthcomic;
        yield return new WaitForSeconds(1f);
        // 4コマ目のキャラのアニメーションを開始
        StartCoroutine(_4comaMainChar.PlayMovementSEFadeStop(2f,0.5f));
        var fourthmainchar = StartCoroutine(_4comaMainChar.AnimChange("isNotice"));
        // 4コマ目の宝箱を空ける
        var fourthchest = StartCoroutine(_4comaChest.AnimChange("isOpen"));

        yield return new WaitUntil(() => isPaused == false); // 一時停止
        yield return fourthchest;
        yield return new WaitForSeconds(2.9f);


        // 雲を時間経過で表示させる
        // 雲の音を出す
        StartCoroutine(_clouds[0].PlayMoveMentSE());
        var cloud1 = StartCoroutine(_clouds[0].AlphaAndPosAndScaleChange(3f,0f,1f));
        yield return new WaitForSeconds(0.1f);
        var cloud2 = StartCoroutine(_clouds[1].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        yield return new WaitForSeconds(0.1f);
        var cloud3 = StartCoroutine(_clouds[2].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        yield return new WaitForSeconds(0.1f);
        var cloud4 = StartCoroutine(_clouds[3].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        // カメラも徐々にズームアウト
        //StartCoroutine(CameraZoom(3, 5.4f));
        //_mainCam.orthographicSize = 5.4f;

        // BGMの音量を上げる
        StartCoroutine(BGMLouder(2f, 0.5f));
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return cloud4;
        yield return new WaitForSeconds(1f);

        // 2ページ目を出す
        _2page.SetActive(true);

        float scale = 1.5f;
        // 雲の大きさと位置を変える
        _clouds[0].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[0].transform.position = new Vector3(-6.87f, 4.215f, 0);

        _clouds[1].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[1].transform.position = new Vector3(8.895f, 4.695f, 0);

        _clouds[2].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[2].transform.position = new Vector3(-7.125f, -6.33f, 0);

        _clouds[3].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[3].transform.position = new Vector3(7.56f, -4.095f, 0);



        // 雲の位置とカメラの位置を指定の位置に移動
        _mainCam.transform.position = new Vector3(16, -3.4f, -10);
        _mainCam.orthographicSize = 8.1f;
        _cloudParent.transform.localPosition = new Vector3(16f, -3.4f, 0);
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(1f);

        // 雲を非表示にする
        var cloud1end = StartCoroutine(_clouds[0].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud2end = StartCoroutine(_clouds[1].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud3end = StartCoroutine(_clouds[2].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud4end = StartCoroutine(_clouds[3].AlphaChange(3f, 1f, 0f));

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return cloud4end;
        // 1ページ目を非表示

        // 2ページ目のイラストを変えるアニメーションでやる
        StartCoroutine(_2pageMainChar.AnimChange("isChange"));
        StartCoroutine(_2pageSubChar.AnimChange("isChange"));
        _stick.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // スティックの入力でカメラをズームする(これが終わるまで待機)
        _canZoom = true;
        while (_canZoom)
        {
            yield return null;
        }
        StartCoroutine(_cameraZoomToController.PlayMoveMentSE());
        _stick.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        _3page.SetActive(true);
        _mainCam.orthographicSize = 5.4f;

        // 2ページ目のスプライトを透明度をいじって非表示          // 3ページ目を出すα値を時間経過で変化させて
        StartCoroutine(_2pageAlphaChangeObj.AlphaChange(0.5f, 1f, 0f)); ;
        StartCoroutine(_3pageAlphaChangeObj.AlphaChange(0.5f, 0f, 1f));
        // BGと1ページ2ページ目を非表示
        _firstPage.SetActive(false);
        _2page.SetActive(false);
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.5f);
        // BGM変更
        StartCoroutine(BGMChanger(0.5f, _iceEatBGM));
        // 3ページ目の動き開始
        var thirdPage = StartCoroutine(_iceEatManager.IceEatCorutine(3f,4f,4f,3f));
        StartCoroutine(_iceEatManager.PlayMoveMentSERoop(12f,1.5f));
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return thirdPage;
        yield return new WaitForSeconds(1f);
        _spkipButton.SetActive(false);
        // 4ページ目を表示
        // カメラの位置は上のコードで止まった場所でアルファ値徐々に上げて、
        StartCoroutine(BGMChanger(0.5f, _endingBGm));

        _mainCam.transform.position = new Vector3(0, -14.44f, -10f);
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return StartCoroutine(ChangeComaColor(_4page, Color.white, 2f));

        _secontroller.SEPlayer();
        StartCoroutine(_4pageBreath[0].AlphaAndPosChange(1f,0,1));
        StartCoroutine(_4pageBreath[1].AlphaAndPosChange(1f, 0, 1));


        // 雲をずっと上下移動（両方同時に）
        StartCoroutine(_4pageClouds[0].RoopVerticalMove());
        StartCoroutine(_4pageClouds[1].RoopVerticalMove());
        // 星はバラバラに上下移動
        StartCoroutine(_4pageStars[0].RoopVerticalMove());
        StartCoroutine(_4pageStars[1].RoopVerticalMove());
        StartCoroutine(_4pageStars[2].RoopVerticalMove());
        StartCoroutine(_4pageStars[3].RoopVerticalMove());
        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 息を吹く（アルファ値変えながら横移動）
        _canMoveScene = true;

    }

    /// <summary>
    /// 親オブジェクトと子オブジェクトの色をすべて変える
    /// </summary>
    /// <param name="parentObj"></param>
    /// <param name="targetColor"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator ChangeComaColor(GameObject parentObj, Color targetColor, float duration)
    {
        SpriteRenderer[] objSpriteRenderers = parentObj.GetComponentsInChildren<SpriteRenderer>();

        Color[] startColors = new Color[objSpriteRenderers.Length];
        for (int i = 0; i < objSpriteRenderers.Length; i++)
        {
            startColors[i] = objSpriteRenderers[i].color;
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            for (int i = 0; i < objSpriteRenderers.Length; i++)
            {
                objSpriteRenderers[i].color = Color.Lerp(startColors[i], targetColor, t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最後に完全にターゲット色に揃える
        for (int i = 0; i < objSpriteRenderers.Length; i++)
        {
            objSpriteRenderers[i].color = targetColor;
        }
    }

    private IEnumerator CameraZoom(float duration, float endOrthographicSize)
    {
        float elapsedTime = 0f;
        float startOrthographicSize = _mainCam.orthographicSize;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _mainCam.orthographicSize = Mathf.Lerp(startOrthographicSize, endOrthographicSize, t);
            yield return null;
        }

        _mainCam.orthographicSize = endOrthographicSize;
    }

    private IEnumerator BGMQuieter(float duration,float minVolume)
    {
        float elapsedTime = 0f;
        float startVolume = _bgmManager._bgmSourvce.volume;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _bgmManager._bgmSourvce.volume = Mathf.Lerp(startVolume, minVolume, elapsedTime / duration);
            yield return null;
        }
        _bgmManager._bgmSourvce.volume = minVolume;

    }

    private IEnumerator BGMLouder(float duration, float MaxVolume)
    {
        float elapsedTime = 0f;
        float startVolume = _bgmManager._bgmSourvce.volume;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _bgmManager._bgmSourvce.volume = Mathf.Lerp(startVolume, MaxVolume, elapsedTime / duration);
            yield return null;
        }
        _bgmManager._bgmSourvce.volume = MaxVolume;

    }

    private IEnumerator BGMChanger(float duration,AudioClip _targetBGM)
    {
        float elapsedTime = 0f;
        float startVolume = _bgmManager._bgmSourvce.volume;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _bgmManager._bgmSourvce.volume = Mathf.Lerp(startVolume, 0.05f, elapsedTime / duration);
            yield return null;
        }
        _bgmManager._bgmSourvce.volume = 0.1f;

        _bgmManager._bgmSourvce.clip = _targetBGM;
        _bgmManager._bgmSourvce.Play();

        elapsedTime = 0f;
        startVolume = _bgmManager._bgmSourvce.volume;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _bgmManager._bgmSourvce.volume = Mathf.Lerp(startVolume, 0.5f, elapsedTime / duration);
            yield return null;
        }
        _bgmManager._bgmSourvce.volume = 0.5f;
        yield return null;
    }
}
