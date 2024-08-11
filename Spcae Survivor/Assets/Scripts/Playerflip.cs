/*
작성자 : 20184038 이준영
코드 이름 : 플레이어의 스프라이트에 관련된 동작들 관리
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerflip : MonoBehaviour
{
    // 필요한 스프라이트와 에니메이터
    SpriteRenderer ChararcterRenderer;
    Animator anim;

    void Start()
    {
        // 시작때 연결해준다
        ChararcterRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    // 180도 회전하면 좌우를 바꿈
    public void switch_ratate() {
        ChararcterRenderer.flipX = true;
    }

    // 다시 180도 회전하면 좌우를 다시 바꿔줌
    public void re_switch_rotate() {
        ChararcterRenderer.flipX = false;
    }

    // 걷고 있는 에니메이션인지 확인
    public bool WalkingCheck() {
        return anim.GetBool("IsWalking");
    }

    // 걷고 있는 에니메이션으로 설정 아님 해제
    public void SetWalkingbool(bool check) {
        anim.SetBool("IsWalking", check);
    }

    // 점프 하고 있는지 확인
    public bool JumpingCheck()
    {
        return anim.GetBool("IsJumping");
    }

    // 점프 에니메이션으로 설정 아님 해제
    public void SetJumpingbool(bool check)
    {
        anim.SetBool("IsJumping", check);
    }
}
