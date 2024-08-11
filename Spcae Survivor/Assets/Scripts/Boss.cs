/*
작성자 : 20184038 이준영
코드 이름 : 보스
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // 필요한 스크립트, 오브젝트, 콜리더, 사운드
    public GameManager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public Transform player;
    public GameObject[] bullet;
    BoxCollider2D collider1;
    public BoxCollider2D collider2;
    public BoxCollider2D collider3;
    public AudioClip Attacksound;
    public AudioClip Deathsound;
    AudioSource audioSource;
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;

    // 필요한 변수들 
    private bool isDie;
    private int nextMove;
    private float curtime;
    public float cooltime;
    public int health;
    private int Maxthealth;
    private float Rangecurtime1, Rangecurtime2;
    public float Rangecooltime1, Rangecooltime2;
    private float PhasetwoRange1, PhasetwoRange2;
    private float PhasetwoRangecool1, PhasetwoRangecool2;

    // Start is called before the first frame update
    void Start()
    {
        // 필요한 컴포넌트 연결
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider1 = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        // 필요한 변수들 초기 설정
        isDie = false;
        nextMove = 1;
        Maxthealth = health;
        PhasetwoRangecool1 = 0.3f;
        PhasetwoRangecool2 = 1.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie) // 죽지 않았으면
        {
            // 시간에 맞춰 설정한 속력으로 좌우로 움직임
            Turn();
            rigid.velocity = new Vector2(nextMove * 2.5f, 0);

            // 체력이 절반 이하면 페이즈2 실행
            if (Maxthealth / 2 >= health)
            {
                Phase2();
            }
            else  // 아니면 1 실행
            {
                Phase1();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌했는데 플레이어 총알이면 데미지를 입음
        if (collision.gameObject.tag == "Bullet")
        {
            OnDamage();
        }
    }

    void Turn()
    {
        // 쿨타임에 맞춰 움직이는 좌 우 방향을 설정해줌
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

    void SoundManager(string sound)
    {
        // 상황에 맞는 사운드, 적절한 음량으로 조절
        switch (sound)
        {
            case "DIE":
                audioSource.clip = Deathsound;
                audioSource.volume = 0.100f;
                break;
            case "ATTACK":
                audioSource.clip = Attacksound;
                audioSource.volume = 0.420f;
                break;
        }
        audioSource.Play();
    }

    void Phase1() 
    {
        // 총알이 나가는 곳에서 플레이어와의 각도를 계산하여 회전시킴
        Vector3 direction1 = player.transform.position - pos1.transform.position;
        Vector3 direction2 = player.transform.position - pos2.transform.position;
        Vector3 direction3 = player.transform.position - pos3.transform.position;
        direction1.Normalize();
        direction2.Normalize();
        direction3.Normalize();
        float ratationZ1 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
        float ratationZ2 = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg;
        float ratationZ3 = Mathf.Atan2(direction3.y, direction3.x) * Mathf.Rad2Deg;
        pos1.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ1);
        pos2.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ2);
        pos3.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ3);

        // 두 개의 총알 나가는 곳은 동일한 쿨타임으로 보통 총알 발사
        if (Rangecurtime1 <= 0)
        {
            Instantiate(bullet[0], pos1.position, pos1.transform.rotation);
            Instantiate(bullet[0], pos3.position, pos3.transform.rotation);
            SoundManager("ATTACK");
            Rangecurtime1 = Rangecooltime1;
        }
        // 하나의 총알 나가는 곳은 두 곳과 다른 쿨타임으로 보통 총알 발사
        if (Rangecurtime2 <= 0)
        {
            Instantiate(bullet[0], pos2.position, pos2.transform.rotation);
            SoundManager("ATTACK");
            Rangecurtime2 = Rangecooltime2;
        }
        Rangecurtime1 -= Time.deltaTime;
        Rangecurtime2 -= Time.deltaTime;
    }

    void Phase2()
    {
        // 총알이 나가는 곳에서 플레이어와의 각도를 계산하여 회전시킴
        Vector3 direction1 = player.transform.position - transform.position;
        Vector3 direction2 = player.transform.position - pos2.transform.position;
        Vector3 direction3 = player.transform.position - transform.position;
        direction1.Normalize();
        direction2.Normalize();
        direction3.Normalize();
        float ratationZ1 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
        float ratationZ2 = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg;
        float ratationZ3 = Mathf.Atan2(direction3.y, direction3.x) * Mathf.Rad2Deg;
        pos1.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ1);
        pos2.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ2);
        pos3.transform.rotation = Quaternion.Euler(0f, 0f, ratationZ3);

        // 두 곳은 동일한 쿨타임으로 일반 총알 발사
        if ( PhasetwoRange1<= 0)
        {
            Instantiate(bullet[0], pos1.position, pos1.transform.rotation);
            Instantiate(bullet[0], pos3.position, pos3.transform.rotation);
            SoundManager("ATTACK");
            PhasetwoRange1 = PhasetwoRangecool1;
        }
        // 한 곳은 다른 쿨타임으로 데미지 2 총알 발사 
        if (PhasetwoRange2 <= 0)
        {
            Instantiate(bullet[1], pos2.position, pos2.transform.rotation);
            SoundManager("ATTACK");
            PhasetwoRange2 = PhasetwoRangecool2;
        }
        PhasetwoRange1 -= Time.deltaTime;
        PhasetwoRange2 -= Time.deltaTime;
    }

    // 데미지를 입으면 스프라이트 빨간색 변경, 체력 감소, 이후 0.3초 이후 스프라이트 색깔 다시 돌아옴
    void OnDamage()
    {
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        DownHealth();
        Invoke("OffDamage", 0.3f);
    }

    // 죽지 않았으면 스프라이트 색깔이 다시 돌아옴
    void OffDamage()
    {
        if (!isDie)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    // 죽으면
    public void OnDie()
    {
        // 점수 1000점 추가
        gameManager.addScore(1000);

        // 스프라이트 빨간색, 상하 반전
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        spriteRenderer.flipY = true;

        // 리지드 바디를 다이나믹으로 설정하고 중력 설정하여 떨어뜨리기
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.gravityScale = 1.0f;

        // 충돌하는 콜라이더 전부 비활성화
        collider1.enabled = false;
        collider2.enabled = false;
        collider3.enabled = false;

        // 속도를 멈췄다가 위로 힘을 가함
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 죽는 소리 재생
        SoundManager("DIE");

        // 1.5초 후 비활성화
        Invoke("DeActive", 1.5f);
    }

    // 보스 몹 비활성화하고 승리창으로 넘어감
    void DeActive()
    {
        gameObject.SetActive(false);
        gameManager.Victory();
    }

    // 피가 1 초과 -> 피를 깎음, 아니면 죽고 피를 0으로 만듬
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
