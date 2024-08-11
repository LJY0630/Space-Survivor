/*
작성자 : 20184038 이준영
코드 이름 : 게임 시작 화면에서의 버튼 동작
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 게임 시작 버튼을 누르면 게임 씬으로 넘어감
    public void StartGame() 
    {
        SceneManager.LoadScene("Main");
    }

    // 게임 끝 버튼을 누르면 게임이 꺼짐
    public void EndGame()
    {
        Application.Quit();
    }
}
