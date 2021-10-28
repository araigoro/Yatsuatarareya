using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //ボールのプレハブを入れる
    public GameObject prefab;

    //発射済みのボールリスト
    private List<GameObject> generatedball = new List<GameObject>();

    //発射速度
    public int power;

    //カメラの右から発射するか左から発射するか決めるときに使用
    private int[] half = new int[] { -1, 1 };

    //ターゲットの座標
    private Vector3 target;

    //フリック処理用
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;

    //投げる音
    public AudioClip SE;
    private AudioSource audio;
   

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネント取得
        audio = gameObject.GetComponent<AudioSource>();
        target = new Vector3(0.0f, 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Flick();
    }

    //フリック処理メソッド
    public void Flick()
    {
        //フリックの始点座標を記録
        if (Input.GetKeyDown(KeyCode.Mouse0)) touchStartPos = Input.mousePosition;
        //フリックの終点座標を記録
        else if (Input.GetKeyUp(KeyCode.Mouse0)) touchEndPos = Input.mousePosition;
        //フリック座標の差分を計算
        Vector3 diff = touchEndPos - touchStartPos;

        //縦の差分のほうが大きい、かつ大きさが30以上の場合ボール発射
        //Mathf.Absは絶対値をとるもの
        if (Mathf.Abs(diff.x) < Mathf.Abs(diff.y) && 30 < diff.y) Fire();
    }

    public void Fire()
    {
        //ボール生成(発射位置を画面の右左上下に散らす）
        GameObject ball = (GameObject)Instantiate(
            prefab,
            new Vector3(half[Random.Range(0,half.Length)]*1.5f,Random.Range(3.0f, 6.0f), -10),
            Quaternion.identity);

        Debug.Log(half[Random.Range(0, half.Length)] * 1.5f);
        //リストに追加
        generatedball.Add(ball);
        //発射
        ball.GetComponent<Rigidbody>().AddForce(target * power);
        //発射音
        audio.PlayOneShot(SE);

        //無限に発射しないようリセット
        touchStartPos = Vector3.zero;
        touchEndPos = Vector3.zero;

        //10個以上になった生成したら古いのを削除
        while (generatedball.Count > 10) DestroyBall();

    }

    //ボール削除メソッド
    public void DestroyBall()
    {
        //リストの中で一番古いものを取得
        GameObject oldball = generatedball[0];
        //削除＆破壊
        generatedball.RemoveAt(0);
        Destroy(oldball);
    }

}
