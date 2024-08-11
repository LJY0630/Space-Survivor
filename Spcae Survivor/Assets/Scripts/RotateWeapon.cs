/*
작성자 : 20184038 이준영
코드 이름 : 무기 스프라이트 반전 관리
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeapon : MonoBehaviour
{
    // Start is called before the first frame update

    // 필요한 스프라이트 
    SpriteRenderer weaponSprite;

    void Start()
    {
        // 필요한 스프라이트 연결
        weaponSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 무기의 Y축 반전 설정
    public void weapon_rotate() {
        weaponSprite.flipY = true;
    }

    // 무기의 Y축 반전 취소
    public void re_weapon_rotate() {
        weaponSprite.flipY = false;
    }
}
