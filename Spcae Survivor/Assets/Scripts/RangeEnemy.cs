/*
작성자 : 20184038 이준영
코드 이름 : 원거리 몬스터
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    // 필요한 오디오, 스크립트, 콜라이더, 벡터 등을 선언
    public GameManager gameManager;
    public Transform player;
    Rigidbody2D rigid;
    Animator anim;
    LayerMask enemy;
    SpriteRenderer spriteRenderer;
    public GameObject bullet;
    public Transform pos;
    CapsuleCollider2D collider;
    public AudioClip Attacksound;
    public AudioClip Deathsound;
    AudioSource audioSource;

    // 필요한 변수들 선언
    public int health;
    bool isDie;
    private int nextMove;
    private int sightDirection;
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
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        // 시야를 가로막으면 안되는 레이어들을 제외하기 위해 설정
        enemy = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Trigger") | LayerMask.GetMask("FlyingEnemy") | LayerMask.GetMask("Bullet") | LayerMask.GetMask("EnemyBullet") | LayerMask.GetMask("Spike");

        // 초기 값이 필요한 변수들 설정
        sightDirection = 1;
        isDie = false;
        isChase = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!isDie) // 죽지 않은 상태
        {
            if (!isChase) // 추적이 아니면 추적하지 않음
            {
                NotAttack();
            }

            if (isChase) // 추적이면 공격함
            {
                RangeAttack();
            }

            Debug.DrawRay(transform.position, Vector3.right * sightDirection * 7, new Color(1, 0, 1)); // 디버그용 레이 함수
            RaycastHit2D sightHit = Physics2D.Raycast(transform.position, Vector3.right * sightDirection, 7, ~enemy); // 제외할 레이어 외의 다른 레이어들을 파악
            if (sightHit.collider != null) // 다른 레이어가 인식이 되면
            {
                if (sightHit.collider.tag == "Platform" && sightHit.distance <= 0.5f) // 그것이 플랫폼(벽)이고 거리가 0.5 이하면
                {
                    Turn(); // 뒤를 돈다
                }

                if (sightHit.collider.tag == "Player") // 플레이어면
                {
                    isChase = true; // 추적 활성화
                }
                else // 플레이어가 아니면
                {
                    isChase = false; // 추적 비활성화
                }
            }
            else // 제외한 레이어를 빼고 아무것도 닿지 않으면
            {
                isChase = false; // 추적 비활성화
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

    // 다음 움직임 생각 함수
    void Think()
    {
        // -1, 0 ,1 에서 하나 받아옴
        nextMove = Random.Range(-1, 2);

        // 0 즉, 가만히 있는 것이 아니면
        if (nextMove != 0)
        {
            sightDirection = nextMove; // 전까지 움직였던 방향이 바라보는 방향
        }

        animationControl(); // 움직임에 맞는 애니메이션 동작 

        // 다음 움직임까지 걸리는 시간을 2 ~ 5 초 사이의 랜덤 난수로 설정하여 넣음
        float nextThinkTime = Random.Range(2f, 5f);
        cooltime = nextThinkTime;
    }

    // 벽이나 낭떠러지를 만나면
    void Turn()
    {
        if (!isChase) // 쫒지 않으면
        {
            nextMove *= -1; // 움직이는 방향 스위치
            sightDirection *= -1; // 바라보는 방향도 스위치
            Flip(); // 오브젝트를 Y축 회전을 이용하여 좌 우 반전
        }
        else if (isChase) // 쫒으면
        {
            nextMove = 0; // 가만히 서서 플레이어를 바라보도록
            animationControl(); // 알맞은 애니메이션 재생
        }
    }

    // 상황에 맞는 애니메이션
    void animationControl()
    {
        if (nextMove != 0) // 움직이면
        {
            Flip(); // 오브젝트 좌우 반전 판단
            anim.SetBool("IsWalking", true); // 걷는 모션 재생
        }
        else if (nextMove == 0) // 가만히 있으면
        {
            anim.SetBool("IsWalking", false); // 걷지 않아 가만히 있는 모션 재생
        }
    }

    // 쫒지 않은 상태
    void NotAttack()
    {
        // 쿨타임마다 다음 움직임 설정
        if (curtime <= 0)
        {
            Think();
            curtime = cooltime;
        }
        curtime -= Time.deltaTime;

        // 더 가서 밑이 낭떠러지가 아니면 바라보는 방향으로 움직임
        if (!CheckBlock())
            rigid.velocity = new Vector2(nextMove, 0);
        else // 더 가서 밑에 낭떠러지면
        {
            Turn(); // 반대편으로 돌음
        }
    }

    // 원거리 공격 함수
    void RangeAttack()
    {
        nextMove = 0; // 가만히 서있는다
        animationControl(); // 적합한 애니메이션 재생

        if (Rangecurtime <= 0) // 쿨 타임마다 바라보는 방향으로 총알 발사, 총알 사운드 재생, 쿨타임 초기화
        {
            Instantiate(bullet, pos.position, transform.rotation);
            SoundManager("ATTACK");
            Rangecurtime = Rangecooltime;
        }
        Rangecurtime -= Time.deltaTime;
    }

    // 밑이 낭떠러진지 확인하는 불 함수
    bool CheckBlock()
    {
        Vector2 frontdownVector = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y); // 오브젝트의 바로 앞의 위치의 벡터를 구함
        Debug.DrawRay(frontdownVector, Vector3.down, new Color(0, 1, 0)); // 구한 벡터를 토대로 디버그용 레이케스트를 그림
        // 바로 앞 위치부터 플랫폼을 인식하는 아래로 내리는 레이케스트 (낭떠러지 확인)
        RaycastHit2D rayHitDown = Physics2D.Raycast(frontdownVector, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHitDown.collider == null) // null이면
        {
            return true; // 바로 앞은 낭떠러지
        }
        else // 아니면 
        {
            return false; // 바로 앞은 낭떠러지가 아님
        }
    }

    // 움직일 때 오브젝트 축 변경
    void Flip() 
    {
        if (nextMove == -1) // 왼쪽이면
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // y축으로 180도 회전
        }
        else if(nextMove == 1) // 오른쪽이면
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // 다시 원래대로 y축 회전
        }
    }

    // 콜라이더 충돌
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet") // 총알이면 데미지
        {
            OnDamage();
        }
        else if (collision.gameObject.tag == "Dead") // 혹시 낙사 구간에 진입하면 사망
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
    void OffDamage() {
        if (!isDie)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnDie()
    {
        // 이 적을 죽이면 게임 점수 200점 추가
        gameManager.addScore(200);

        // 스프라이트의 색깔을 적색으로, 스프라이트 위 아래 반전
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        spriteRenderer.flipY = true;

        // 콜라이더의 충돌 판정을 끄기 위해 콜라이더 비활성화
        collider.enabled = false;

        // 속력을 0으로 바꾼뒤 위로 5의 힘으로 한번 밀어줘서 점프 후 사망 구현
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
