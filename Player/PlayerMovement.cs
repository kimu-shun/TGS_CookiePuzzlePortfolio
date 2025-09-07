using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ړ����x"), SerializeField] private float _moveSpeed = 3.0f;
    [Header("�W�����v�̑傫��"), SerializeField] private float _jumpForece = 5.0f;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Time.timeScale = 1.0f;
    }

    public void MovePlayer(Rigidbody2D playerRb, Vector2 controllInput,Animator _anim)
    {
        // ���͂��ړ��ɕϊ�
        Vector2 moveDirection = new Vector2(controllInput.x, 0) * _moveSpeed;
        playerRb.velocity = new Vector2(moveDirection.x, playerRb.velocity.y); // Y�����x��ێ�

        // �R���g���[���[�̓��͂�0�łȂ�
        if (controllInput.x != 0)
        {
            // �A�j���[�V�����J�n
            _anim.SetBool("isRun", true);

            // �E�ړ��Ȃ�E��������
            if(controllInput.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else
            {
                // ����������
                _spriteRenderer.flipX= true;
            }
        }
        else
        {
            // �����A�j���[�V�������I��点��
            _anim.SetBool("isRun", false);
        }
    }

    public void Jump(Rigidbody2D playerRb)
    {
        // Y�����̑��x�𒼐ڐݒ肵�ăW�����v������
        playerRb.velocity = new Vector2(playerRb.velocity.x, _jumpForece);
    }

}
