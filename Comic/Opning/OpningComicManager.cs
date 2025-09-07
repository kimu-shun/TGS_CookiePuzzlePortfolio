using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpningComicManager : MonoBehaviour
{
    [Header("移動させるコミックのコマ")]
    [SerializeField]
    private List<ComicMovement> _comicMovementList = new List<ComicMovement>();
    [Header("1コマ目のサブキャラ"),SerializeField]
    private ComicObjMovemet _1comaSubChara;
    [Header("2コマ目のサブキャラ"), SerializeField]
    private ComicObjMovemet _2comaSubChar;
    [Header("2コマ目のはてな"), SerializeField]
    private ComicObjMovemet _2comaQuestion;

    [Header("2コマ目の星"), SerializeField]
    private List<ComicObjMovemet> _2comaStars = new List<ComicObjMovemet>();

    [Header("2コマ目のメインキャラ"), SerializeField]
    private ComicObjMovemet _2comaChar;

    [Header("2コマ目のメインキャラ"), SerializeField]
    private ComicObjMovemet _3comaChar;

    [Header("3コマ目のメインキャラの目"), SerializeField]
    private List<ComicObjMovemet> _3ComaEyes = new List<ComicObjMovemet>();

    [Header("3コマ目の箱からの光"), SerializeField]
    private List<ComicObjMovemet> _boxShines = new List<ComicObjMovemet>();

    [Header("3コマ目のアイス"), SerializeField]
    private ComicObjMovemet _3comaIce;

    [Header("3コマ目の星"), SerializeField]
    private List<ComicObjMovemet> _3comaStars = new List<ComicObjMovemet>();

    [Header("最後のパネル"), SerializeField]
    private ComicObjMovemet _panle;

    [Header("最後のキャラ"), SerializeField]
    private ComicObjMovemet _lastChar;

    [SerializeField]
    private FadeManager _fadeManager;

    public bool isPaused = false;
    private Coroutine comicCoroutine;


    private void Start()
    {
        _fadeManager = GameObject.Find("FadeCanvas").GetComponent<FadeManager>();
        comicCoroutine = StartCoroutine(ComicMovie());


    }

    private IEnumerator ComicMovie()
    {
        yield return new WaitUntil(() => isPaused == false); // 一時停止
        yield return new WaitForSeconds(1f);
        // 一コマ目を右から出す
        var FirstComic = StartCoroutine(_comicMovementList[0].PanelMove());
        yield return FirstComic;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 一コマ目のましゅまろを移動させる
        var subCharMove = StartCoroutine(_1comaSubChara.ScaleChange());
        yield return subCharMove;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 2コマ目を上から出す
        var secondComic = StartCoroutine(_comicMovementList[1].PanelMove());
        yield return secondComic;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // ましゅまろをジャンプさせる
        var subCharJump = StartCoroutine(_2comaSubChar.SubJump());
        yield return subCharJump;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 洞窟中央からはてなを出現させる　最大値まで行ったら拡縮
        var question = StartCoroutine(_2comaQuestion.AlphaScaleMove());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.5f);

        // 星を小さい状態から出して最終位置&スケールまで行ったら、その場で拡縮
        // 最初に大きいやつ二つ
        var star1 = StartCoroutine(_2comaStars[0].AlphaScaleMove());
        var star1Scale = StartCoroutine(_2comaStars[0].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.1f);

        var star2 = StartCoroutine(_2comaStars[1].AlphaScaleMove());
        var star2Scale = StartCoroutine(_2comaStars[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.3f);
        // 少し待て小さいの2つ
        var star3 = StartCoroutine(_2comaStars[2].AlphaScaleMove());
        var star3Scale = StartCoroutine(_2comaStars[2].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.1f);

        var star4 = StartCoroutine(_2comaStars[3].AlphaScaleMove());
        var star4Scale = StartCoroutine(_2comaStars[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        //　はてな目的地まで行ったら拡縮
        yield return question;
        var questionScale =  StartCoroutine(_2comaQuestion.RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止



        // クッキーを左右回転
        var comaChar = StartCoroutine(_2comaChar.ObjRotater(3));
        yield return comaChar;

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 三コマ目右から出す
        var thierdComic = StartCoroutine(_comicMovementList[2].PanelMove());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 2コマ目のはてなとキラキラの拡縮を小さくする
        // さっきまでの処理を止める
        if (questionScale != null)
        {
            StopCoroutine(questionScale);
            StartCoroutine(_2comaQuestion.CustomRoopScaleChange(2f, 0.2f));
            yield return new WaitForSeconds(0.18f);

        }
        if (star1Scale != null)
        {
            StopCoroutine(star1Scale);
            StartCoroutine(_2comaStars[0].CustomRoopScaleChange(2f, 0.2f));
            yield return new WaitForSeconds(0.18f);

        }
        if (star2Scale != null)
        {
            StopCoroutine(star2Scale);
            StartCoroutine(_2comaStars[1].CustomRoopScaleChange(2f, 0.2f));
            yield return new WaitForSeconds(0.18f);

        }
        if (star3Scale != null)
        {
            StopCoroutine(star3Scale);
            StartCoroutine(_2comaStars[2].CustomRoopScaleChange(2f, 0.2f));
            yield return new WaitForSeconds(0.18f);

        }
        if (star4Scale != null)
        {
            StopCoroutine(star4Scale);
            StartCoroutine(_2comaStars[3].CustomRoopScaleChange(2f, 0.2f));
            yield return new WaitForSeconds(0.18f);

        }

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return thierdComic;
        // キャラを上下に動かす（ゆっくり小刻み）
        var thirdChar = StartCoroutine(_3comaChar.ObjBounce());
        _3comaChar.OnSound();

        // 目をキラキラさせる
        var leftEye = StartCoroutine(_3ComaEyes[0].RoopScaleChange());
        var rightEye = StartCoroutine(_3ComaEyes[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(1f);

        // 宝箱から光出す
        var boxShine1 = StartCoroutine(_boxShines[0].ScaleChange());

        var boxShine2 = StartCoroutine(_boxShines[1].ScaleChange());


        var boxShine3 = StartCoroutine(_boxShines[2].ScaleChange());


        var boxShine4 = StartCoroutine(_boxShines[3].ScaleChange());

        // 少し遅れてアイスを出す

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.005f);
        StartCoroutine(_3comaIce.AlphaScaleMove());

        // 少し遅れてキラキラ出す
        // 最初に大きいやつ二つ
        var comic3Star1 = StartCoroutine(_3comaStars[0].AlphaScaleMove());
        StartCoroutine(_3comaStars[0].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.1f);

        var comic3Star2 = StartCoroutine(_3comaStars[1].AlphaScaleMove());
        StartCoroutine(_3comaStars[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.3f);
        // 少し待て小さいの2つ
        var comic3Star3 = StartCoroutine(_3comaStars[2].AlphaScaleMove());
        StartCoroutine(_3comaStars[2].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(0.1f);

        var comic3Star4 = StartCoroutine(_3comaStars[3].AlphaScaleMove());
        StartCoroutine(_3comaStars[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 止まった後光を拡大縮小
        yield return boxShine4;
        StartCoroutine(_boxShines[0].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[1].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[2].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[3].RoopScaleYChange(0.3f, 2f));

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        // 星が目的地にたどり着いたら拡縮
        yield return boxShine4;
        StartCoroutine(_boxShines[0].RoopScaleChange());
        StartCoroutine(_boxShines[1].RoopScaleChange());
        StartCoroutine(_boxShines[2].RoopScaleChange());
        StartCoroutine(_boxShines[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return new WaitForSeconds(2.5f);

        // パネル出す
        var panle = StartCoroutine(_panle.AlphaChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止
        yield return panle;

        // 下からキャラ出す
        var last = StartCoroutine(_lastChar.PosChange());

        yield return new WaitUntil(() => isPaused == false); // 一時停止

        yield return last;
        yield return new WaitForSeconds(2f);
        // 画面遷移
        var fade = StartCoroutine(_fadeManager.FadeIn("StageSlectSceneTest", 1f));
        yield return fade;


    }
}
