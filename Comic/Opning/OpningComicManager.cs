using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpningComicManager : MonoBehaviour
{
    [Header("�ړ�������R�~�b�N�̃R�}")]
    [SerializeField]
    private List<ComicMovement> _comicMovementList = new List<ComicMovement>();
    [Header("1�R�}�ڂ̃T�u�L����"),SerializeField]
    private ComicObjMovemet _1comaSubChara;
    [Header("2�R�}�ڂ̃T�u�L����"), SerializeField]
    private ComicObjMovemet _2comaSubChar;
    [Header("2�R�}�ڂ̂͂Ă�"), SerializeField]
    private ComicObjMovemet _2comaQuestion;

    [Header("2�R�}�ڂ̐�"), SerializeField]
    private List<ComicObjMovemet> _2comaStars = new List<ComicObjMovemet>();

    [Header("2�R�}�ڂ̃��C���L����"), SerializeField]
    private ComicObjMovemet _2comaChar;

    [Header("2�R�}�ڂ̃��C���L����"), SerializeField]
    private ComicObjMovemet _3comaChar;

    [Header("3�R�}�ڂ̃��C���L�����̖�"), SerializeField]
    private List<ComicObjMovemet> _3ComaEyes = new List<ComicObjMovemet>();

    [Header("3�R�}�ڂ̔�����̌�"), SerializeField]
    private List<ComicObjMovemet> _boxShines = new List<ComicObjMovemet>();

    [Header("3�R�}�ڂ̃A�C�X"), SerializeField]
    private ComicObjMovemet _3comaIce;

    [Header("3�R�}�ڂ̐�"), SerializeField]
    private List<ComicObjMovemet> _3comaStars = new List<ComicObjMovemet>();

    [Header("�Ō�̃p�l��"), SerializeField]
    private ComicObjMovemet _panle;

    [Header("�Ō�̃L����"), SerializeField]
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
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~
        yield return new WaitForSeconds(1f);
        // ��R�}�ڂ��E����o��
        var FirstComic = StartCoroutine(_comicMovementList[0].PanelMove());
        yield return FirstComic;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // ��R�}�ڂ̂܂���܂���ړ�������
        var subCharMove = StartCoroutine(_1comaSubChara.ScaleChange());
        yield return subCharMove;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // 2�R�}�ڂ��ォ��o��
        var secondComic = StartCoroutine(_comicMovementList[1].PanelMove());
        yield return secondComic;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // �܂���܂���W�����v������
        var subCharJump = StartCoroutine(_2comaSubChar.SubJump());
        yield return subCharJump;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // ���A��������͂ĂȂ��o��������@�ő�l�܂ōs������g�k
        var question = StartCoroutine(_2comaQuestion.AlphaScaleMove());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.5f);

        // ������������Ԃ���o���čŏI�ʒu&�X�P�[���܂ōs������A���̏�Ŋg�k
        // �ŏ��ɑ傫������
        var star1 = StartCoroutine(_2comaStars[0].AlphaScaleMove());
        var star1Scale = StartCoroutine(_2comaStars[0].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.1f);

        var star2 = StartCoroutine(_2comaStars[1].AlphaScaleMove());
        var star2Scale = StartCoroutine(_2comaStars[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.3f);
        // �����҂ď�������2��
        var star3 = StartCoroutine(_2comaStars[2].AlphaScaleMove());
        var star3Scale = StartCoroutine(_2comaStars[2].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.1f);

        var star4 = StartCoroutine(_2comaStars[3].AlphaScaleMove());
        var star4Scale = StartCoroutine(_2comaStars[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        //�@�͂ĂȖړI�n�܂ōs������g�k
        yield return question;
        var questionScale =  StartCoroutine(_2comaQuestion.RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~



        // �N�b�L�[�����E��]
        var comaChar = StartCoroutine(_2comaChar.ObjRotater(3));
        yield return comaChar;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // �O�R�}�ډE����o��
        var thierdComic = StartCoroutine(_comicMovementList[2].PanelMove());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // 2�R�}�ڂ̂͂ĂȂƃL���L���̊g�k������������
        // �������܂ł̏������~�߂�
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

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return thierdComic;
        // �L�������㉺�ɓ������i������菬���݁j
        var thirdChar = StartCoroutine(_3comaChar.ObjBounce());
        _3comaChar.OnSound();

        // �ڂ��L���L��������
        var leftEye = StartCoroutine(_3ComaEyes[0].RoopScaleChange());
        var rightEye = StartCoroutine(_3ComaEyes[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(1f);

        // �󔠂�����o��
        var boxShine1 = StartCoroutine(_boxShines[0].ScaleChange());

        var boxShine2 = StartCoroutine(_boxShines[1].ScaleChange());


        var boxShine3 = StartCoroutine(_boxShines[2].ScaleChange());


        var boxShine4 = StartCoroutine(_boxShines[3].ScaleChange());

        // �����x��ăA�C�X���o��

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.005f);
        StartCoroutine(_3comaIce.AlphaScaleMove());

        // �����x��ăL���L���o��
        // �ŏ��ɑ傫������
        var comic3Star1 = StartCoroutine(_3comaStars[0].AlphaScaleMove());
        StartCoroutine(_3comaStars[0].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.1f);

        var comic3Star2 = StartCoroutine(_3comaStars[1].AlphaScaleMove());
        StartCoroutine(_3comaStars[1].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.3f);
        // �����҂ď�������2��
        var comic3Star3 = StartCoroutine(_3comaStars[2].AlphaScaleMove());
        StartCoroutine(_3comaStars[2].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.1f);

        var comic3Star4 = StartCoroutine(_3comaStars[3].AlphaScaleMove());
        StartCoroutine(_3comaStars[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // �~�܂���������g��k��
        yield return boxShine4;
        StartCoroutine(_boxShines[0].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[1].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[2].RoopScaleYChange(0.3f, 2f));
        StartCoroutine(_boxShines[3].RoopScaleYChange(0.3f, 2f));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // �����ړI�n�ɂ��ǂ蒅������g�k
        yield return boxShine4;
        StartCoroutine(_boxShines[0].RoopScaleChange());
        StartCoroutine(_boxShines[1].RoopScaleChange());
        StartCoroutine(_boxShines[2].RoopScaleChange());
        StartCoroutine(_boxShines[3].RoopScaleChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(2.5f);

        // �p�l���o��
        var panle = StartCoroutine(_panle.AlphaChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~
        yield return panle;

        // ������L�����o��
        var last = StartCoroutine(_lastChar.PosChange());

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return last;
        yield return new WaitForSeconds(2f);
        // ��ʑJ��
        var fade = StartCoroutine(_fadeManager.FadeIn("StageSlectSceneTest", 1f));
        yield return fade;


    }
}
