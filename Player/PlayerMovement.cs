using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移動速度"), SerializeField] private float _moveSpeed = 3.0f;
    [Header("ジャンプの大きさ"), SerializeField] private float _jumpForece = 5.0f;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Time.timeScale = 1.0f;
    }

    public void MovePlayer(Rigidbody2D playerRb, Vector2 controllInput,Animator _anim)
    {
        // 入力を移動に変換
        Vector2 moveDirection = new Vector2(controllInput.x, 0) * _moveSpeed;
        playerRb.velocity = new Vector2(moveDirection.x, playerRb.velocity.y); // Y軸速度を保持

        // コントローラーの入力が0でない
        if (controllInput.x != 0)
        {
            // アニメーション開始
            _anim.SetBool("isRun", true);

            // 右移動なら右向かせる
            if(controllInput.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else
            {
                // 左向かせる
                _spriteRenderer.flipX= true;
            }
        }
        else
        {
            // 歩くアニメーションを終わらせる
            _anim.SetBool("isRun", false);
        }
    }

    public void Jump(Rigidbody2D playerRb)
    {
        // Y方向の速度を直接設定してジャンプさせる
        playerRb.velocity = new Vector2(playerRb.velocity.x, _jumpForece);
    }

}
