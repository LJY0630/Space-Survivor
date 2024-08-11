/*
작성자 : 20184038 이준영
코드 이름 : 플레이어의 행동 모음
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 필요한 오디오, 스크립트, 콜라이더, 벡터 등을 선언
    public AudioClip Jumpsound;
    public AudioClip Deathsound;
    public AudioClip Damagedsound;
    public GameManager gameManager;
    Rigidbody2D rigid;
    public Playerflip playersc;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spriteArm;
    AudioSource audioSource;
    CapsuleCollider2D collider;

    // 필요한 변수들 선언
    public float maxSpeed;
    public float jumpPower;
    bool isDie;
    bool isDamaged;
    bool double_damage;

    // Start is called before the first frame update
    void Start()
    {
        // 필요한 컴포넌트들을 시작 때 연결
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        
        // 필요한 변수들 설정
        isDie = false;
        isDamaged = false;
        double_damage = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDamaged && !isDie) // 죽지 않고 데미지를 입지 않으면
        {
            // 점프 키를 눌렀는데 점프 한 상태가 아니면 점프
            if (Input.GetButtonDown("Jump") && !playersc.JumpingCheck())
            {
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // 한 번 위로 힘을 주어 점프
                playersc.SetJumpingbool(true); // 점프 하는 중이라 설정
                SoundManager("JUMP"); // 점프 소리
            }

            // 좌 우 키를 때면 속도 0
            if (Input.GetButtonUp("Horizontal"))
            {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0f, rigid.velocity.y);
            }
        }

        // 속도가 0.2 아래면 가만히 있는 애니매이션, 이상이면 걷는 애니매이션
        if (Mathf.Abs(rigid.velocity.x) < 0.2)
            playersc.SetWalkingbool(false);
        else
            playersc.SetWalkingbool(true);
    }

    private void FixedUpdate()
    {
        if (!isDie)
        {
            if (!isDamaged)
            {
                //Move By Key Control
                float h = Input.GetAxisRaw("Horizontal");
                rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
            }

            //MaxSpeed
            if (rigid.velocity.x > maxSpeed) // right Max Speed
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            else if (rigid.velocity.x < -1 * maxSpeed) // lefr Max Speed
                rigid.velocity = new Vector2(-1 * maxSpeed, rigid.velocity.y);

            // 착지 판정
            if (rigid.velocity.y <= 0) // 속력이 0이나 0 아래일 때
            {
                if (rigid.velocity.y < 0) // 특히 0 아래일 때는
                {
                    playersc.SetJumpingbool(true); // 계속 점프 상태이다
                }

                RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 레이를 밑에 쏴서 어떤 오브젝트에 닿았는지 확인
                if (rayHit.collider != null) // 플랫폼이 맞고
                {
                    if (rayHit.distance < 0.75f) // 거리가 0.75 이하면
                    {
                        playersc.SetJumpingbool(false); // 점프 해제
                    }
                }// 부딫히면 콜라이더가 들어가 있다
            }
        }
    }

    // 사운드 선택 후 재생 함수
    void SoundManager(string sound) 
    {
        switch (sound)
        {
            case "JUMP": // 점프 소리 설정
                audioSource.clip = Jumpsound;
                audioSource.volume = 0.155f;
                break;
            case "DIE": // 죽음 소리 설정
                audioSource.clip = Deathsound;
                audioSource.volume = 0.100f;
                break;
            case "DAMAGED": // 데미지 받는 소리 설정
                audioSource.clip = Damagedsound;
                audioSource.volume = 0.275f;
                break;
        }
        audioSource.Play(); // 설정한 소리 재생
    }

    // 콜라이더 충돌 판정
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spike" | collision.gameObject.tag == "Dead") // 가시나 낙사 구간이면
        {
                OnDie(); // 죽음
        }

        if (collision.gameObject.tag == "Enemy" | collision.gameObject.tag == "EnemyBullet") // 적이나 총알이면
        {
            OnDamaged(collision.transform.position); // 데미지를 입음
        }

        if (collision.gameObject.tag == "TwoDamage") // 보스의 특수 총알이면
        {
            double_damage = true; // 두배 데미지 활성화
            OnDamaged(collision.transform.position); // 데미지를 입음
        }

    }

    // 트리거 충돌 판정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Teleport") { // 텔레포트면
            gameManager.NextStage(); // 다음 스테이지 이동
        }
    }

    // 데미지를 입으면
    void OnDamaged(Vector2 targetPos) {

        if (gameManager.playerhealth > 1) // 플레이어 체력이 1 초과면
        {
            SoundManager("DAMAGED"); // // 데미지 소리
            if (double_damage) // 더블 데미지면
            {
                gameManager.DownHealth(); // 한번더 데미지를 입음
                double_damage = false; // 더블 데미지 해제
            }
        }
        else  // 아니면 1인 상태에서 데미지
        {
            SoundManager("DIE"); // 죽음 소리 출력
        }
        gameManager.DownHealth(); // 체력을 감소 시킴
        gameManager.addScore(-50); // 점수 - 50
        isDamaged = true; // 데미지를 입어서 참
        gameObject.layer = 12; // 잠깐 동안 몬스터에게 데미지를 안받는 상태로 전환

        // 데미지를 입었다는 것을 알려주기 위해 스프라이터를 빨갛게 
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        spriteArm.color = new Color(1, 0, 0, 1.0f);

        // 플레이어와 적과의 x 좌표 위치를 계산하여 적의 반대 방향을 파악하고 그 방향으로 후 밀침
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        Vector2 damagep = new Vector2(dirc * 2.7f, 0);
        VelocityZero();
        rigid.AddForce(damagep, ForceMode2D.Impulse);

        // 0.6초 후에 데미지를 받아 조작 불가 상태 풀림, 그 후 0.3 초 후에 밀리다가 멈춤, 그 후 2 초후에 데미지를 입었다고 색깔을 바꾼 스프라이트를 원래대로 무적 시간 끔
        Invoke("setisDamagedf", 0.6f);
        Invoke("VelocityZero", 0.3f);
        Invoke("OffDamaged", 2);
    }

    // 데미지에서 벗어났다는 것을 알려주는 것
    void OffDamaged()
    {
        if (!isDie) // 죽지 않으면 
        {
            // 스프라이트 색깔 다시 돌아옴
            spriteRenderer.color = new Color(1, 1, 1, 1);
            spriteArm.color = new Color(1, 1, 1, 1);
        }
        gameObject.layer = 10; // 다시 데미지를 받는 상태로 레이어 전환
    }

    // 데미지를 받은 상태인 것을 푸는 함수
    void setisDamagedf() {
        isDamaged = false;
    }

    // 죽음 함수
    public void OnDie()
    {
        if (!isDie) // 죽지 않은 상태에서 죽음 상태가 되어야
        {
            gameManager.AddDeathCount(); // 데스카운트 추가
        }
        isDie = true; // 죽음 참으로 설정

        // 콜라이더 충돌 판정을 끔
        collider.enabled = false;

        // 총 스프라이트 해제, 색깔을 빨간색으로, 위 아래 반전
        spriteRenderer.color = new Color(1, 0, 0, 1.0f);
        spriteRenderer.flipY = true;
        spriteArm.enabled = false;

        // 죽으면 속도를 일시적으로 0으로 만든 후 위로 힘을 한 번줘 점프 하고 죽도록 함
        VelocityZero();
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        int num = gameManager.playerhealth; // 즉사 구간에서는 모든 피를 달아야하기 때문에 죽기 직전의 체력을 받음

        for (int i = num; i >= 1; i--) // 남은 체력을 전부 깎음
        {
            gameManager.DownHealth();
        }

        SoundManager("DIE"); // 죽는 소리 재생

        // 5초 후 오브젝트 비활성화
        Invoke("DeActive", 5);
    }

    // 죽으면 스프라이트 비활성화
    void DeActive()
    {
        if (isDie)
        {
            spriteRenderer.enabled = false;
        }

    }

    // 플레이어 체크포인트 부활
    public void Revive() {
        
        // 죽음 해제
        isDie = false;

        // 스프라이트 색깔 다시 원래대로 복구
        spriteRenderer.color = new Color(1, 1, 1, 1.0f);
        spriteArm.color = new Color(1, 1, 1, 1.0f);

        // 스프라이트 복구
        spriteRenderer.enabled = true;
        spriteArm.enabled = true;
        spriteRenderer.flipY = false;

        //콜라이더 복구
        collider.enabled = true;

        // 부활 직후 속도 0
        VelocityZero();
    }

    // 플레이어가 죽었는지 판단
    public bool get_isDie() 
    {
        return isDie;
    }

    // 플레이어 죽음 참
    public void set_isDie()
    {
        isDie = true;
    }

    // 속도 0으로 설정
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
