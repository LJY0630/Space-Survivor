/*
작성자 : 20184038 이준영
코드 이름 : 플레이어가 쏘는 기본 총알 특성
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 필요한 변수 설정
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        // 생성된지 1.3초 후 총알을 파괴
        Invoke("DestroyBullet", 1.3f);
    }

    void FixedUpdate()
    {   // 총알이 한 방향으로 설정한 속도로 계속 나가게 설정
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // 총알이 콜라이더가 있는 어디든지 부딫히면 총알 파괴
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroyBullet();
    }

    // 총알 파괴
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
