/*
작성자 : 20184038 이준영
코드 이름 : 적의 총알
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // 필요한 변수 선언
    public float speed;
    public float destroy;

    // Start is called before the first frame update
    void Start()
    {
        // 설정한 초가 되면 총알이 파괴되도록 설정
        Invoke("DestroyBullet", destroy);
    }

    // Update is called once per frame
    void FixedUpdate()
    {       // 계속 한 방향으로 특정 속도로 총알이 나가게 함
            transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
            // 플레이어와 플랫폼에 총알이 충돌하면 총알 파괴
            if (collision.gameObject.tag == "Player" | collision.gameObject.tag == "Platform")
            {
                DestroyBullet();
            }
    }

    // 총알 파괴
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
