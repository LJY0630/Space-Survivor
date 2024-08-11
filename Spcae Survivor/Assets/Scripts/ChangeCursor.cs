/*
작성자 : 20184038 이준영
코드 이름 : 인게임 커서 변경
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    // 인스펙터 창에 뜨게 커서 이미지 선언, 이미지 위치 조정을 위한 벡터
    [SerializeField] Texture2D cursorImg;
    Vector2 hotSpot;

    // Start is called before the first frame update
    void Start()
    {
        // 중앙에 오도록 커서 이미지에서 각 2로 나눔
        hotSpot.x = cursorImg.width / 2;
        hotSpot.y = cursorImg.height / 2;

        // 커서를 소프트웨어에서 그리도록 설정한다
        Cursor.SetCursor(cursorImg, hotSpot, CursorMode.ForceSoftware);
    }
}
