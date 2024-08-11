/*
작성자 : 20184038 이준영
코드 이름 : 플레이어의 총 발사 동작
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update

    // 필요한 스프라이트, 스크립트, 사운드
    public AudioClip Attacksound;
    public PlayerMove playermove;
    public Playerflip player;
    public RotateWeapon weaponSprite;
    public GameObject bullet;
    AudioSource audioSource;
    public Transform pos;

    // 필요한 변수들
    public float cooltime;
    private float curtime = 0;
    int switch_flag = 0;

    void Start()
    {
        // 초기에 오디오 소스 연결
        audioSource = GetComponent<AudioSource>();
    }

    void AttackSound() // 공격 소리 실행
    {
        // 클립을 연결하고, 소리 크기 설정, 소리 실행
        audioSource.clip = Attacksound;
        audioSource.volume = 0.420f;
        audioSource.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playermove.get_isDie()) // 죽지 않았으면 총을 쏠 수 있음
        {
            // 마우스와 물체 간의 각도를 구하고 이를 기반으로 물체 회전
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            direction.Normalize();
            float ratationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, ratationZ);


            if (Input.GetMouseButtonUp(0)) // 버튼 때면 0으로 설정 다시 누를 때 바로 총이 나갈 수 있게
            {
                curtime = 0;
            }
            else if (Input.GetMouseButton(0)) // 버튼을 누르고 있으면 총알 발사와 사운드 재생
            {
                if (curtime <= 0) // 쿨타임이 됬으면
                {
                    Instantiate(bullet, pos.position, transform.rotation);
                    AttackSound();
                    curtime = cooltime; // 다시 쿨타임을 돌게 함
                }

                // 쿨타임을 프레임에 구애받지 않는 시간으로 돌게 함
                curtime -= Time.deltaTime;
            }

            // 마우스가 왼쪽을 보고 있어 물체(팔)이 왼쪽 회전, 플레이어는 오른쪽을 보고 있으면
            if (transform.eulerAngles.z <= 270 && transform.eulerAngles.z > 90 && switch_flag == 0)
            {
                // 플레이어 왼쪽을 보게 하며 팔 스프라이트 좌우 변경, 왼쪽을 보고 있다고 플래그 설정
                player.switch_ratate();
                weaponSprite.weapon_rotate();
                switch_flag = 1;
            } // 반대의 상황이면
            else if ((transform.eulerAngles.z > 270 || transform.eulerAngles.z <= 90) && switch_flag == 1)
            {
                // 반대로 설정
                player.re_switch_rotate();
                weaponSprite.re_weapon_rotate();
                switch_flag = 0;
            }
        }
    }
}
