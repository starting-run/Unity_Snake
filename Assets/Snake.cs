using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    float speedMove = 3f; //이동속도
    float speedRot = 120f; //회전속도
    List<Transform> tails = new List<Transform>();

    bool isDead = false;
    Transform coin;


    //UI
    GameObject PanelOver;
    Text TxtCoin;
    Text TxtTime;
    int coinCnt = 0;
    float startTime;

    void Start() {
    }

    void Awake() {
        InitGame();
        startTime = Time.time;
    }

    void Update() {
        if (!isDead) {
            MoveHead();
            MoveTail();
            SetTime();
        }
    }

    void MoveHead() { //머리이동
        //이동
        float amount = speedMove * Time.deltaTime;
        transform.Translate(Vector3.forward * amount);

        //회전
        amount = Input.GetAxis("Horizontal") * speedRot;
        transform.Rotate(Vector3.up * amount * Time.deltaTime);
    }

    void MoveTail () { //꼬리이동
        Transform target = transform;

        foreach (Transform tail in tails) {
            Vector3 pos = target.position;
            Quaternion rot = target.rotation;

            tail.position = Vector3.Lerp(tail.position, pos, 4 * Time.deltaTime);
            tail.rotation = Quaternion.Lerp(tail.rotation, rot, 4 * Time.deltaTime);

            target = tail;
        }
    }

    //동전처리
    void MoveCoin() {
        coinCnt++;
        float x = Random.Range(-9f, 9f);
        float z = Random.Range(-4f, 4f);
        coin.position = new Vector3(x, 0, z);
    }

    //케이스별 충돌
    void OnCollisionEnter(Collision other)
    {
        switch (other.transform.tag)
        {
            case "Coin":
                MoveCoin(); //Coin이동
                AddTail(); //꼬리 추가
                break;
            case "Wall":
            case "Tail":
                isDead = true;
                PanelOver.SetActive(isDead);
                break;
        }
    }

    //게임초기화
    void InitGame() {
        coin = GameObject.Find("Coin").transform;

        //UI
        PanelOver = GameObject.Find("PanelOver");
        PanelOver.SetActive(false);

        TxtCoin = GameObject.Find("TxtCoin").GetComponent<Text>();
        TxtTime = GameObject.Find("TxtTime").GetComponent<Text>();
    }

    //꼬리 추가
    void AddTail () {
        GameObject tail = Instantiate(Resources.Load("Tail")) as GameObject;
        Vector3 pos = transform.position;

        //처리
        int cnt = tails.Count;
        if (cnt == 0) {
            tail.tag = "Untagged";
        } else {
            pos = tails[cnt - 1].position;
        }
        tail.transform.position = pos;

        // 색 변경
        Color[] colors = { new Color(0, 0.5f, 0, 1), new Color(0, 0.5f, 1, 1) };
        int n = (cnt / 3 % 2);
        tail.GetComponent<Renderer>().material.color = colors[n];
        tails.Add(tail.transform);
    }

    //UI 표시
    void SetTime()
    {
        TxtCoin.text = coinCnt.ToString("획득 코인: 0");

        float span = Time.time - startTime;
        int h = Mathf.FloorToInt(span / 3600);
        int m = Mathf.FloorToInt(span / 60 % 60);
        float s = span % 60;

        TxtTime.text = string.Format("시간: {0:0}:{1:0}:{2:00}", h, m, s);
    }

    // UI 버튼클릭
    public void OnButtonClick(Button button) {
        switch (button.name) {
            case "BtnYes":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "BtnNo":
                Application.Quit();
                break;
        }
    }
}