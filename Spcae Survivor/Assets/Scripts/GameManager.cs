/*
작성자 : 20184038 이준영
코드 이름 : 인게임 유아이와 플레이어 죽음 재시작, 종료 같은 게임의 동작을 관리하는 게임 메니저
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 오디오 소스
    AudioSource STAGETHEME;
    public AudioSource BOSSTHEME;
    public AudioSource ENDTHEME;

    // 다른 스크립트와 버튼, 텍스트, 이미지
    public GameObject[] Stages;
    public GameObject MainGameUI;
    public GameObject EndUI;
    public Text UIPoint;
    public Text UIStage;
    public Text GameOver;
    public Text FinalScore;
    public Text DeathCount;
    public Button RestartCheck;
    public Button Restart;
    public Button Exit;
    public Image[] UIhealth;
    public PlayerMove player;

    // 필요한 변수들
    private int stageIndex;
    public int totalPoint;
    public int playerhealth;
    private int Maxhealth;
    private int deathCount;

    // Start is called before the first frame update
    void Start()
    {
        // 컴포넌트 연결
        STAGETHEME = GetComponent<AudioSource>();

        // 초기 UI 설정
        MainGameUI.gameObject.SetActive(true);
        EndUI.gameObject.SetActive(false);

        // 초기 변수 설정
        deathCount = 0;
        stageIndex = 0;
        Maxhealth = playerhealth;

        // 초기 스테이지 텍스트 설정
        UIStage.text = "Tutorial";
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1) // 스테이지가 남았으면
        {
            // 체력 보충
            healthRegenarate();

            // 스테이지 이동
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            // 플레이어 재배치
            PlayerReposition();

            // 마지막 스테이지면 배경음악 변경과 스테이지 이름 변경
            if (stageIndex == Stages.Length - 1)
            {
                BOSSTHEME.enabled = true;
                STAGETHEME.enabled = false;
                UIStage.text = "BOSS STAGE";
            }
            else // 아니면 보통 스테이지 세팅
            {
                UIStage.text = "STAGE " + stageIndex;
            }
        }
    }

    public void DownHealth()
    {
        
        if (playerhealth > 1) // 1보다 크면 아직 죽지 않아 체력 깎고 체력 스트라이트 교체
        {
            playerhealth--;
            UIhealth[playerhealth].sprite = Resources.Load("NoHealth", typeof(Sprite)) as Sprite;
        }
        else if(playerhealth == 1) // 1이면 죽기 때문에 죽고 비활성화된 게임오버 UI 활성화 추가
        {
            playerhealth--;
            UIhealth[playerhealth].sprite = Resources.Load("NoHealth", typeof(Sprite)) as Sprite;
            player.OnDie();
            GameOver.gameObject.SetActive(true);
            RestartCheck.gameObject.SetActive(true);
            Restart.gameObject.SetActive(true);
            Exit.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //포인트 텍스트는 계속 갱신
        UIPoint.text = totalPoint.ToString();
    }

    public void addScore(int score) // 스코어 늘리는 함수
    {
        totalPoint += score;
    }

    void PlayerReposition() { // 플레이어를 초기 위치로 재배치, 속력 0 설정
        player.transform.position = new Vector3(-4.52f, -0.34f, 0);
        player.VelocityZero();
    }

    public void RestarttoCheck() // 체크 포인트 재시작 함수
    {
        // 부활
        player.Revive();

        // 게임 오버 인터페이스 요소 비활성화
        RestartCheck.gameObject.SetActive(false);
        GameOver.gameObject.SetActive(false);
        Restart.gameObject.SetActive(false);
        Exit.gameObject.SetActive(false);

        // 체력 보충, 재배치 체크포인트 부활 대가로 300점 깎음
        healthRegenarate();
        PlayerReposition();
        totalPoint -= 300;
    }

    public void RestarttoTutorial() // 초기 씬 (맨 처름 상태)로 돌아가서 처음부터 재시작 
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame() // 게임 종료
    {
        Application.Quit();
    }

    void healthRegenarate() // 체력 보충
    {
        if (Maxhealth != playerhealth) // 체력이 풀피가 아니면
        {
            for (int i = Maxhealth; i > playerhealth; i--) // 체력이 없는 만큼 스프라이트 다시 설정
            {
                UIhealth[i - 1].sprite = Resources.Load("Health", typeof(Sprite)) as Sprite;
            }
        }
        playerhealth = Maxhealth; // 체력을 최대로 설정
    }

    public void AddDeathCount() // 죽은 횟수 증가
    {
        deathCount++;
    }

    public void Victory() { // 승리
        
        // 죽음 상태로 변경 (아무것도 하지 못하게)
        player.set_isDie();

        // 끝 배경음악로 변경
        BOSSTHEME.enabled = false;
        ENDTHEME.enabled = true;

        // 끝 UI로 변경
        MainGameUI.gameObject.SetActive(false);
        EndUI.gameObject.SetActive(true);

        // 최종 스코어, 죽음을 텍스트로 보여줌
        FinalScore.text = "Final Score : " + totalPoint;
        DeathCount.text = "Death Count : " + deathCount;
    }
}
