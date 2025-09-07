using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndingComicManager : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;
    [Header("1�y�[�W��"),SerializeField]
    private GameObject _firstPage;
    [SerializeField]
    private List<GameObject> _comicComaPanelsObj = new List<GameObject>();
    [Header("�R�~�b�N�̃R�}"),SerializeField]
    private List<ComicMovement> _comicComaPanels = new List<ComicMovement>();

    [Header("1�R�}�ڂ̃T�u�L����"),SerializeField]
    private EndingComicObjMovement _1comaSubChar;

    [Header("1�R�}�ڂ̃��C���L����"), SerializeField]
    private EndingComicObjMovement _1comaMainChar;

    [Header("2�R�}�ڂ̃��C���L����"), SerializeField]
    private EndingComicObjMovement _2comaMainChar;

    [Header("2�R�}�ڂ̂т�����}�[�N"), SerializeField]
    private EndingComicObjMovement _2comaDiscovery;

    [Header("2�R�}�ڂ̌�"), SerializeField]
    private EndingComicObjMovement _2comaShiny;

    [Header("3�R�}�ڂ̔w�i"), SerializeField]
    private EndingComicObjMovement _3leftBG;
    [Header("3�R�}�ڂ̌�"), SerializeField]
    private EndingComicObjMovement[] _3comaStars;
    [Header("3�R�}�ڂ̍��̕�"), SerializeField]
    private EndingComicObjMovement _3leftWall;
    [Header("3�R�}�ڂ̉E�̕�"),SerializeField]
    private EndingComicObjMovement _3rightWall;
    [Header("3�R�}�ڕ�"),SerializeField]
    private EndingComicObjMovement _3chest;

    [Header("4�R�}�ڂ̃��C���L����"), SerializeField]
    private EndingComicObjMovement _4comaMainChar;

    [Header("4�R�}�ڂ̕�"), SerializeField]
    private EndingComicObjMovement _4comaChest;


    [Header("4�R�}�ڂ̉_"), SerializeField]
    private List<EndingComicObjMovement> _clouds = new List<EndingComicObjMovement>();
    [SerializeField]
    private GameObject _cloudParent;

    [Header("2�y�[�W��"), SerializeField]
    private GameObject _2page;

    [SerializeField]
    private GameObject _stick;

    [Header("2�y�[�W�ڂ̃Y�[�����Ǘ�����R�[�h"),SerializeField]
    private CameraZoomToController _cameraZoomToController;
    public bool _canZoom = false;

    [Header("2�y�[�W�ڂœ����ɂ���C���X�g"), SerializeField]
    private EndingComicObjMovement _2pageAlphaChangeObj;

    [Header("2�y�[�W�ڂ̃��C���L����"), SerializeField]
    private EndingComicObjMovement _2pageMainChar;

    [Header("2�y�[�W�ڂ̃T�u�L����"), SerializeField]
    private EndingComicObjMovement _2pageSubChar;

    [SerializeField]
    private GameObject _3page;

    [Header("3�y�[�W�ڂœ����ɂ���C���X�g"), SerializeField]
    private EndingComicObjMovement _3pageAlphaChangeObj;

    [Header("3�R�}�ڂ̃A�C�X�̃X�N���v�g"),SerializeField]
    private IceEatManager _iceEatManager;

    [SerializeField]
    private GameObject _4page;

    [Header("4�R�}�ڂ̉_"),SerializeField]
    private EndingComicObjMovement[] _4pageClouds;

    [Header("4�R�}�ڂ̐�"),SerializeField]
    private EndingComicObjMovement[] _4pageStars;
    [Header("4�R�}�ڂ̑�"),SerializeField]
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


    [Header("�R�}���Â�����Ƃ��̐F")]
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
                Debug.Log("�Z�{�^����������܂����I");
                // ������FadeMangaer�g����Scene�J�ڂ���
                StartCoroutine(_fadeManager.FadeIn("TitleScene", 1f));
            }

        }
    }

    public IEnumerator EndComicManger()
    {
        //�[�[�[1�y�[�W�ځ[�[�[//
        // 1�R�}�ڂ��w��ʒu�܂ňړ�������
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~
        yield return new WaitForSeconds(1f);
        // 1�R�}�ڂ��ړ�������
        var FirstComic = StartCoroutine(_comicComaPanels[0].PanelMove());
        yield return FirstComic;
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // 1�R�}�ڂ̃A�j���[�V�������J�n
        // �T�u�L�������ړ�������
        var subCharMove = StartCoroutine(_1comaSubChar.AnimAndScalePosChange("isWalk", 3f));
        // ���������o��
        StartCoroutine(_1comaSubChar.PlayMoveMentSERoop(3f));
        // ���C���L����������낫��낳����
        var mainCharCareful = StartCoroutine(_1comaMainChar.AnimAndScalePosChange("isCareful", 3f));

        yield return subCharMove;
        yield return mainCharCareful;

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.2f);
        // 2�R�}�ڂ��w��ʒu�܂ňړ�
        // 1�R�}�ڂ̐F�������Ԍo�߂ňÂ�����
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _firstHideColor, 1f));
        // 2�R�}�ڂ��ړ�������
        var secondComic = StartCoroutine(_comicComaPanels[1].PanelMove());
        yield return secondComic;
        yield return new WaitForSeconds(0.5f);

        // 2�R�}�ڂ̃L������傫�����Č��̑傫���ɖ߂�
        var mainCharScale = StartCoroutine(_2comaMainChar.AnimChange("isSuprised"));
        //�r�b�N���}�[�N�������^�C�~���O�ő傫������ς���
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(_2comaDiscovery.AnimChange("isChange"));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(_2comaShiny.AnimChange("isChange"));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(2.5f);


        // 3�R�}�ڂ��w��ʒu�܂ňړ�
        // 1,2�R�}�ڂ̐F�����Ԍo�߂ňÂ�����
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _secondHideColor, 1.5f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[1], _firstHideColor, 1.5f));
        // 3�R�}�ڂ��ړ�����Ȃ��ă��l��0~1�ɂ���
        var thirdComic = StartCoroutine(ChangeComaColor(_comicComaPanelsObj[2],Color.white,1.5f));
        // �󔠂̎���̃L���L�����g�k������
        StartCoroutine(_3comaStars[0].RoopScaleChange());
        StartCoroutine(_3comaStars[3].RoopScaleChange());
        StartCoroutine(_3comaStars[2].RoopScaleChange());
        StartCoroutine(_3comaStars[1].RoopScaleChange());
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return thirdComic;
        yield return new WaitForSeconds(1f);

        // 3�R�}�ڑS���傫������
        StartCoroutine(_3leftBG.ScaleChange(2.5f));
        // ���A�̊O�ǂ��ړ������ē��A���ɓ����Ă��銴�������o
        StartCoroutine(_3leftWall.PlayMoveMentSE());
        var leftwall = StartCoroutine(_3leftWall.SmoothPosChange(2f));
        var rightWall = StartCoroutine(_3rightWall.SmoothPosChange(2f));

        
        // �J�������Y�[��������
        //var zoom = StartCoroutine(CameraZoom(2.5f, 4.07f));

        //yield return new WaitForSeconds(1f);
        // �󔠂̈ʒu�Ƒ傫����ς���
        var chest = StartCoroutine(_3chest.PosAndScaleChange(2.5f));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~
        yield return chest;
        yield return new WaitForSeconds(2.0f);

        // 3�R�}�ڂ̐F�����Ԍo�߂ňÂ�����
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[0], _thirdHideColor, 1f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[1], _secondHideColor, 1f));
        StartCoroutine(ChangeComaColor(_comicComaPanelsObj[2], _firstHideColor, 1f));
        // 4�R�}�ڂ��w��ʒu�܂ňړ�
        // 4�R�}�ڂ��ړ�������
        var fourthcomic = StartCoroutine(_comicComaPanels[3].PanelMove());
        // BGM�̉��ʂ�������
        StartCoroutine(BGMQuieter(0.7f, 0.15f));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return fourthcomic;
        yield return new WaitForSeconds(1f);
        // 4�R�}�ڂ̃L�����̃A�j���[�V�������J�n
        StartCoroutine(_4comaMainChar.PlayMovementSEFadeStop(2f,0.5f));
        var fourthmainchar = StartCoroutine(_4comaMainChar.AnimChange("isNotice"));
        // 4�R�}�ڂ̕󔠂��󂯂�
        var fourthchest = StartCoroutine(_4comaChest.AnimChange("isOpen"));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~
        yield return fourthchest;
        yield return new WaitForSeconds(2.9f);


        // �_�����Ԍo�߂ŕ\��������
        // �_�̉����o��
        StartCoroutine(_clouds[0].PlayMoveMentSE());
        var cloud1 = StartCoroutine(_clouds[0].AlphaAndPosAndScaleChange(3f,0f,1f));
        yield return new WaitForSeconds(0.1f);
        var cloud2 = StartCoroutine(_clouds[1].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        yield return new WaitForSeconds(0.1f);
        var cloud3 = StartCoroutine(_clouds[2].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        yield return new WaitForSeconds(0.1f);
        var cloud4 = StartCoroutine(_clouds[3].AlphaAndPosAndScaleChange(3f, 0f, 1f));
        // �J���������X�ɃY�[���A�E�g
        //StartCoroutine(CameraZoom(3, 5.4f));
        //_mainCam.orthographicSize = 5.4f;

        // BGM�̉��ʂ��グ��
        StartCoroutine(BGMLouder(2f, 0.5f));
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return cloud4;
        yield return new WaitForSeconds(1f);

        // 2�y�[�W�ڂ��o��
        _2page.SetActive(true);

        float scale = 1.5f;
        // �_�̑傫���ƈʒu��ς���
        _clouds[0].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[0].transform.position = new Vector3(-6.87f, 4.215f, 0);

        _clouds[1].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[1].transform.position = new Vector3(8.895f, 4.695f, 0);

        _clouds[2].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[2].transform.position = new Vector3(-7.125f, -6.33f, 0);

        _clouds[3].transform.localScale = new Vector3(scale, scale, scale);
        _clouds[3].transform.position = new Vector3(7.56f, -4.095f, 0);



        // �_�̈ʒu�ƃJ�����̈ʒu���w��̈ʒu�Ɉړ�
        _mainCam.transform.position = new Vector3(16, -3.4f, -10);
        _mainCam.orthographicSize = 8.1f;
        _cloudParent.transform.localPosition = new Vector3(16f, -3.4f, 0);
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(1f);

        // �_���\���ɂ���
        var cloud1end = StartCoroutine(_clouds[0].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud2end = StartCoroutine(_clouds[1].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud3end = StartCoroutine(_clouds[2].AlphaChange(3f, 1f, 0f));
        yield return new WaitForSeconds(0.1f);
        var cloud4end = StartCoroutine(_clouds[3].AlphaChange(3f, 1f, 0f));

        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return cloud4end;
        // 1�y�[�W�ڂ��\��

        // 2�y�[�W�ڂ̃C���X�g��ς���A�j���[�V�����ł��
        StartCoroutine(_2pageMainChar.AnimChange("isChange"));
        StartCoroutine(_2pageSubChar.AnimChange("isChange"));
        _stick.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // �X�e�B�b�N�̓��͂ŃJ�������Y�[������(���ꂪ�I���܂őҋ@)
        _canZoom = true;
        while (_canZoom)
        {
            yield return null;
        }
        StartCoroutine(_cameraZoomToController.PlayMoveMentSE());
        _stick.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        _3page.SetActive(true);
        _mainCam.orthographicSize = 5.4f;

        // 2�y�[�W�ڂ̃X�v���C�g�𓧖��x���������Ĕ�\��          // 3�y�[�W�ڂ��o�����l�����Ԍo�߂ŕω�������
        StartCoroutine(_2pageAlphaChangeObj.AlphaChange(0.5f, 1f, 0f)); ;
        StartCoroutine(_3pageAlphaChangeObj.AlphaChange(0.5f, 0f, 1f));
        // BG��1�y�[�W2�y�[�W�ڂ��\��
        _firstPage.SetActive(false);
        _2page.SetActive(false);
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return new WaitForSeconds(0.5f);
        // BGM�ύX
        StartCoroutine(BGMChanger(0.5f, _iceEatBGM));
        // 3�y�[�W�ڂ̓����J�n
        var thirdPage = StartCoroutine(_iceEatManager.IceEatCorutine(3f,4f,4f,3f));
        StartCoroutine(_iceEatManager.PlayMoveMentSERoop(12f,1.5f));
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return thirdPage;
        yield return new WaitForSeconds(1f);
        _spkipButton.SetActive(false);
        // 4�y�[�W�ڂ�\��
        // �J�����̈ʒu�͏�̃R�[�h�Ŏ~�܂����ꏊ�ŃA���t�@�l���X�ɏグ�āA
        StartCoroutine(BGMChanger(0.5f, _endingBGm));

        _mainCam.transform.position = new Vector3(0, -14.44f, -10f);
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        yield return StartCoroutine(ChangeComaColor(_4page, Color.white, 2f));

        _secontroller.SEPlayer();
        StartCoroutine(_4pageBreath[0].AlphaAndPosChange(1f,0,1));
        StartCoroutine(_4pageBreath[1].AlphaAndPosChange(1f, 0, 1));


        // �_�������Ə㉺�ړ��i���������Ɂj
        StartCoroutine(_4pageClouds[0].RoopVerticalMove());
        StartCoroutine(_4pageClouds[1].RoopVerticalMove());
        // ���̓o���o���ɏ㉺�ړ�
        StartCoroutine(_4pageStars[0].RoopVerticalMove());
        StartCoroutine(_4pageStars[1].RoopVerticalMove());
        StartCoroutine(_4pageStars[2].RoopVerticalMove());
        StartCoroutine(_4pageStars[3].RoopVerticalMove());
        yield return new WaitUntil(() => isPaused == false); // �ꎞ��~

        // ���𐁂��i�A���t�@�l�ς��Ȃ��牡�ړ��j
        _canMoveScene = true;

    }

    /// <summary>
    /// �e�I�u�W�F�N�g�Ǝq�I�u�W�F�N�g�̐F�����ׂĕς���
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

        // �Ō�Ɋ��S�Ƀ^�[�Q�b�g�F�ɑ�����
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
