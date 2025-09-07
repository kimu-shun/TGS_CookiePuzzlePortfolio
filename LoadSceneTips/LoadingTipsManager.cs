using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadingTipsManager : MonoBehaviour
{
    private Canvas _thisCanvas;
    [SerializeField]
    private GameObject[] _pages;

    [SerializeField]
    private Image[] _hitnCounts;

    [SerializeField]
    private Sprite[] _hintCountCircles;

    [SerializeField]
    private GameObject pageCenter;

    [SerializeField]
    private GameObject _pageAllowRight;

    [SerializeField]
    private GameObject _pageAllowLeft;
    [SerializeField]
    private GameObject _startButton;

    [SerializeField]
    private int _minPageNum;
    [SerializeField]
    private int _maxPageNum;

    private AudioSource _mainAudio;

    [SerializeField]
    private AudioClip _movePageSE;

    [SerializeField]
    private AudioClip _decidSE;


    public int _currentPageNum;

    public bool _canOnStartButton = false;
    public bool _canStart = false;
    private bool _isPageMoving = false;
    private bool _stickRetrunedToNutral = true;

    private void Start()
    {
        _mainAudio = GetComponent<AudioSource>();
        _mainAudio.clip = _movePageSE;
        _thisCanvas = GetComponent<Canvas>();
        _thisCanvas.worldCamera = Camera.main;
        _currentPageNum = 1;
        AllowHideAndShow();
        HintCountCView();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            if (_currentPageNum < _maxPageNum)
            {
                StartCoroutine(PageMove(pageCenter, 0.5f, -1f));
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_minPageNum < _currentPageNum)
            {
                StartCoroutine(PageMove(pageCenter, 0.5f, 1f));
            }
        }
    }

    public void PageMovement(InputAction.CallbackContext context)
    {
        var controllerInput = context.ReadValue<Vector2>();

        Debug.Log(controllerInput);

        if (!_isPageMoving && _stickRetrunedToNutral)
        {
            if (controllerInput.x < -0.5f)
            {
                _stickRetrunedToNutral = false;
                if (_minPageNum < _currentPageNum)
                {
                    StartCoroutine(PageMove(pageCenter, 0.5f, 1f));
                }
            }
            else if (controllerInput.x > 0.5f)
            {
                _stickRetrunedToNutral = false;
                if (_currentPageNum < _maxPageNum)
                {
                    StartCoroutine(PageMove(pageCenter, 0.5f, -1f));
                }
            }
        }
        else if (Mathf.Abs(controllerInput.x) < 0.2f)
        {
            _stickRetrunedToNutral = true;
        }
    }

    public void OnStartButton(InputAction.CallbackContext context)
    {
        if(_canOnStartButton && !_canStart)
        {
            if (context.performed)
            {
                // ���J���Ă���y�[�W�ȊO�����
                _mainAudio.clip = _decidSE;
                _mainAudio.Play(); 
                HidePage(_currentPageNum);
                _canStart = true;
            }
        }
    }

    private void AllowHideAndShow()
    {
        if(_currentPageNum == _maxPageNum &&  _minPageNum == _currentPageNum)
        {
            // �E���A����������
            _pageAllowRight.SetActive(false);
            _pageAllowLeft.SetActive(false);
            if (!_startButton.activeSelf)
            {
                _startButton.SetActive(true);
                _canOnStartButton = true;
            }
        }
        else if(_currentPageNum == _maxPageNum)
        {
            // �E��������
            _pageAllowRight.SetActive(false);
            _pageAllowLeft.SetActive(true);
            if (!_startButton.activeSelf)
            {
                _startButton.SetActive(true);
                _canOnStartButton = true;
            }
        }
        else if( _currentPageNum == _minPageNum)
        {
            // ����������
            _pageAllowRight.SetActive(true);
            _pageAllowLeft.SetActive(false);
        }
        else
        {
            _pageAllowRight.SetActive(true);
            _pageAllowLeft.SetActive(true);
        }
    }

    private void HintCountCView()
    {
        if(_hitnCounts.Length == 1)
        {
            _hitnCounts[0].sprite = _hintCountCircles[1];
        }
        else if(_hitnCounts.Length == 2)
        {
            if(_currentPageNum == _maxPageNum)
            {
                _hitnCounts[0].sprite = _hintCountCircles[0];
                _hitnCounts[1].sprite = _hintCountCircles[1];
            }
            else
            {
                _hitnCounts[0].sprite = _hintCountCircles[1];
                _hitnCounts[1].sprite = _hintCountCircles[0];
            }
        }
        else if(_hitnCounts.Length == 3)
        {
            if (_currentPageNum == _maxPageNum)
            {
                _hitnCounts[0].sprite = _hintCountCircles[0];
                _hitnCounts[1].sprite = _hintCountCircles[0];
                _hitnCounts[2].sprite = _hintCountCircles[1];
            }
            else if (_currentPageNum == _minPageNum)
            {
                _hitnCounts[0].sprite = _hintCountCircles[1];
                _hitnCounts[1].sprite = _hintCountCircles[0];
                _hitnCounts[2].sprite = _hintCountCircles[0];
            }
            else
            {
                _hitnCounts[0].sprite = _hintCountCircles[0];
                _hitnCounts[1].sprite = _hintCountCircles[1];
                _hitnCounts[2].sprite = _hintCountCircles[0];
            }
        }


    }

    private void HidePage(int current)
    {
        int targetIndex = current - 1;

        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(i == targetIndex);
        }

    }

    private IEnumerator PageMove(GameObject centerObj,float duration,float direction)
    {
        RectTransform rect = centerObj.GetComponent<RectTransform>();
        _isPageMoving = true;
        if (direction == 1)
        {
            _currentPageNum--;
        }
        else if (direction == -1)
        {
            _currentPageNum++;
        }
        float elapsedTime = 0;
        var startPos = rect.anchoredPosition;
        var endPos = rect.anchoredPosition + new Vector2(1920f, 0) * direction;

        _mainAudio.Play();

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rect.anchoredPosition = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            yield return null;
        }

        rect.anchoredPosition = endPos;
        AllowHideAndShow();
        HintCountCView();
        _isPageMoving = false;
        yield return null;
    }
}
