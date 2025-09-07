using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimController : MonoBehaviour
{
    [SerializeField]
    private StageSelect _stageSelect;
    [SerializeField]
    private int unlockNum;

    [SerializeField]
    private Animator _signBoardAnim;

    [SerializeField]
    private Animator _starAnim;

    [SerializeField]
    private Animator _waveAnim;

    private FadeManager _fadeManager;

    public void UnlockTile()
    {
        StartCoroutine(UnlockAnim());
    }

    // �X�e�[�W5���N���A�������̃A�j���[�V�����ŌĂԊ֐�
    public void LastUnlockAnim()
    {

        StartCoroutine(LastMove());
    }

    private IEnumerator LastMove()
    {
        _fadeManager = GameObject.Find("FadeCanvas").GetComponent<FadeManager>();
        var lastCharMove = StartCoroutine(_stageSelect.CharIconMovement(unlockNum,5));
        yield return lastCharMove;
        // �V�[���J��
        StartCoroutine(_fadeManager.FadeIn("EndingScene", 2f));

    }

    public void StarAnim()
    {
        _starAnim.SetBool("isSprash", true);
    }

    public void WaveAnim()
    {
        _waveAnim.SetBool("isAnim", true);
    }

    // �^�C���̃O���[����Ԃɕς��A�j���[�V�����̍Ō�Ɏ��s�����ăL�������ړ�������̂ƊŔ𓮂���
    private IEnumerator UnlockAnim()
    {
        int unlocmovekNum = unlockNum - 1;
        var charMove = StartCoroutine(_stageSelect.CharIconMovement(unlockNum, unlocmovekNum));
        //yield return charMove;
        yield return new WaitForSeconds(0.9f);
        _signBoardAnim.SetBool("isFlip",true);
        yield return new WaitForSeconds(1f);
        _signBoardAnim.SetBool("isFlip",false);

    }


}
