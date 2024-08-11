/*
작성자 : 20184038 이준영
코드 이름 : 원거리 비행 몬스터
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeFlyingEnemy : MonoBehaviour
{
    // 필요한 오디오, 스크립트, 콜라이더 등을 선언
    public GameManager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public Transform player;
    public GameObject bullet;
    public Transform pos;
    public Collider2D triggercollider;
    CircleCollider2D collider;
    public AudioClip Attacksound;
    public AudioClip Deathsound;
    AudioSource audioSource;

    // 필요한 변수들 선언
    public int health;
    private bool isDie;
    private int nextMove;
    private bool isChase;
    private float curtime;
    private float Rangecurtime;
    public float cooltime;
    public float Rangecooltime;
    // Start is called before the first frame update
    void Start()
    {
        // 필요한 컴포넌트들을 시작 때 연결
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        // 초기 값이 필요한 변수들 설정
        nextMove = 1;
        isDie = false;
        isChase = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie) // 죽지 않았으면
        {
            if (!isChase) // 추적이 아니면 추적하지 않음
            {
                Turn(); 
                rigid.velocity = new Vector2(nextMove * 1.5f, 0);
            }
            else // 추적이면 원거리 공격
            {
                rigid.velocity = new Vector2(0, 0); // 몬스터 제자리에 가만히 있게 설정
                // 플레이어와 자신과의 각도 계산
                Vector3 direction = player.position - transform.position;
                direction.Normalize();
                float ratationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // 각도 계산 후 총이 나오는 곳의 각도를 설정
                pos.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ);

                if (Rangecurtime <= 0) // 쿨 타임마다 총알 발사, 총알 사운드 재생, 쿨타임 초기화
                {
                    Instantiate(bullet, pos.position, pos.transform.rotation);
                    SoundManager("ATTACK");
                    Rangecurtime = Rangecooltime;
                }
                Rangecurtime -= Time.deltaTime;
            }
        }
    }

    // 사운드 선택 후 재생 함수
    void SoundManager(string sound)
    {
        switch (sound)
        {
            case "DIE": // 죽음 소리 설정
                audioSource.clip = Deathsound;
                audioSource.volume = 0.100f;
                break;
            case "ATTACK": // 공격 소리 설정
                audioSource.clip = Attacksound;
                audioSource.volume = 0.420f;
                break;
        }
        audioSource.Play(); // 설정한 사운드 재생
    }

    // 쿨 타임마다 움직임의 방향을 바꿔 좌, 우로 일정하게 움직임
    void Turn()
    {
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

    // 추적 범위에 플레이어 콜라이더가 들어오면 공격 시작
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isChase = true;
        }
    }

    // 추적 범위에 플레이어 콜라이더가 나가면 공격 종료
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isChase = false;
        }
    }

    // 자신의 몸의 콜라이더가 총알에 맞으면 데미지를 입고 낙사 콜라이더에 부딫히면 죽음
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            OnDamage();
        }
        if (collision.gameObject.tag == "Dead") 
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

    public void OnDie()
    {
        // 이 적을 죽이면 게임 점수 350점 추가
        gameManager.addScore(350);

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
        SoundManager("DIE");

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
