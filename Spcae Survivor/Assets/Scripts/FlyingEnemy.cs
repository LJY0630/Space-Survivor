/*
작성자 : 20184038 이준영
코드 이름 : 공중 근거리 몬스터
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    // 필요한 오디오, 스크립트, 콜라이더, 벡터 등을 선언
    public AudioClip Deathsound;
    public GameManager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public PlayerMove player;
    CircleCollider2D collider;
    Vector2 firstPosition;
    AudioSource audioSource;

    // 필요한 변수들 선언
    public int health;
    public float moveSpeed;
    private int nextMove;
    private bool isChase;
    private bool isDie;
    private float curtime;
    public float cooltime;
    // Start is called before the first frame update
    void Start()
    {
        // 필요한 컴포넌트들을 연결과 벡터 값 설정
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        firstPosition = transform.position;

        // 초기값이 필요한 변수들 초기값 설정
        nextMove = 1;
        isChase = false;
        isDie = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 죽지 않았으면
        if (!isDie)
        {
            // 플레이어가 범위에 들어오지 않았으면 평소 행동, 평소 속도
            if (!isChase)
            {
                Turn();
                rigid.velocity = new Vector2(nextMove * 1.5f, 0);
            }
            else // 플레이어가 범위 안에 들어오면 플레이어와 자신 사이의 벡터값을 계산 후 쫒는 스피드로 추적
            {
                Vector3 direction = player.transform.position - transform.position;
                direction.Normalize();
                rigid.MovePosition((Vector2)transform.position + ((Vector2)direction * moveSpeed * Time.deltaTime));
            }

            // 플레이어가 죽으면 추적 종료, 자신의 원래 위치로 복귀
            if (player.get_isDie())
            {
                isChase = false;
                transform.position = firstPosition;
            }
        }
    }

    // 쿨 타임마다 움직임의 방향을 바꿔 좌, 우로 일정하게 움직임
    void Turn() {

        if (curtime <= 0)
        {
            if (nextMove == 1)
                nextMove = -1;
            else if (nextMove == -1)
                nextMove = 1;

            curtime = cooltime;
        }
        curtime -= Time.deltaTime;
    }

    // 추적 범위에 플레이어 콜라이더가 들어오면 추적 시작
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isChase = true;
        }
    }

    // 자신의 몸의 콜라이더가 총알에 맞으면 데미지를 입고 낙사 콜라이더에 부딫히면 죽음
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            OnDamage();
        }
        else if (collision.gameObject.tag == "Dead")
        {
            OnDie();
        }
    }

    // 데미지를 받으면 스프라이드 색깔 빨갛게 변경하고 체력 1감소, 0.3 초 후에 다시 스프라이트가 원색으로 돌아오도록 설정
    void OnDamage()
    {
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        DownHealth();
        Invoke("OffDamage", 0.3f);

    }

    // 죽은 상태가 아니면 스프라이트의 색깔을 원래대로 복귀 시켜줌
    void OffDamage()
    {
        if (!isDie)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    // 죽는 소리의 음량을 조절한 후에 재생 시키는 함수
    void DeathSound()
    {
        audioSource.clip = Deathsound;
        audioSource.volume = 0.100f;
        audioSource.Play();
    }

    // 죽으면 작동하는 함수
    public void OnDie()
    {
        // 이 적을 죽이면 게임 점수 300점 추가
        gameManager.addScore(300);

        // 스프라이트의 색깔을 적색으로, 스프라이트 위 아래 반전
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        spriteRenderer.flipY = true;

        // 콜라이더의 충돌 판정을 끄기 위해 콜라이더 비활성화
        collider.enabled = false;

        // 공중에 있는 몹이기 때문에 중력이 0으로 설정되있으므로 다시 1로 바꾸고 속력을 0으로 바꾼뒤 위로 5의 힘으로 한번 밀어줘서 점프 후 사망 구현 
        rigid.gravityScale = 1.0f;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 죽는 소리 재생
        DeathSound();

        // 5초 후에 오브젝트 비활성화
        Invoke("DeActive", 5);
    }

    // 게임 오브젝트 비활성화
    void DeActive()
    {
        gameObject.SetActive(false);
    }

    // 데미지를 입는데 체력이 1이었을 때 데미지를 입으면 죽고 피를 깎음
    void DownHealth() 
    {
        if (health > 1)
        {
            health--;
        }
        else 
        {
            isDie = true;
            OnDie();
            health--;
        }
    }
}
