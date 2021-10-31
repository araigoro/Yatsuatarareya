using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBallBomb : MonoBehaviour
{
    //ボールのプレハブを入れる
    public GameObject[] prefab;
    //プレハブ切り替え用
    public int prefabnum;

    //発射済みのプレハブリスト
    private List<GameObject> generatedprefab = new List<GameObject>();

    //発射速度
    public int power;

    //オブジェクト生成時の角度
    private Quaternion RandomQ;

    //カメラの右から発射するか左から発射するか決めるときに使用
    private int[] half = new int[] { -1, 1 };

    //ターゲットの座標
    private Vector3 target;

    //投げる音
    public AudioClip SE;
    private AudioSource audio;

    //発射ボタンを押しているかどうかの判定
    private bool shootButton = false;

    //矢印の向きを格納する変数
    private Vector2 newAngle = new Vector2(0, 0);

    //マウス座標を格納する変数
    private Vector2 lastMousePosition;

    //回転速度を格納
    private Vector2 rotationSpped = new Vector2(0.03f, 0.03f);


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
        if (shootButton == true) Fire();

        //矢印の向きをドラッグで操作
        //左クリックした時
        if (Input.GetMouseButtonDown(0))
        {
            //矢印の角度を格納
            newAngle = gameObject.transform.localEulerAngles;
            //マウス座標を格納
            lastMousePosition = Input.mousePosition;
        }
        //ドラッグしている間
        else if (Input.GetMouseButton(0))
        {
            //Y軸の回転:ドラッグ方向と逆方向に回転
            newAngle.y -= (Input.mousePosition.x - lastMousePosition.x) * rotationSpped.y;
            //X軸も同様
            newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpped.x;
            //newAngleの角度をオブジェクト(MG main)の角度に代入
            gameObject.transform.localEulerAngles = newAngle;
            //マウス座標をlastMousePositionに格納
            lastMousePosition = Input.mousePosition;
        }
    }
    public void Fire()
    {
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,100f))
        {
            //プレハブ生成(発射位置を画面の右左上下に散らす）&角度もまばらに
            RandomQ = Quaternion.Euler(Random.Range(-45f, 45f), Random.Range(-180f, 180f), Random.Range(-45f, 45f));
            GameObject ball = (GameObject)Instantiate(
                prefab[prefabnum],
                new Vector3(half[Random.Range(0, half.Length)] * 1.5f, Random.Range(3.0f, 6.0f), -10),
                RandomQ);
            //リストに追加
            generatedprefab.Add(ball);
            //発射
            ball.GetComponent<Rigidbody>().AddForce(transform.up* power);
            //発射音
            audio.PlayOneShot(SE);

            //10個以上生成したら古いのを削除
            while (generatedprefab.Count > 10) DestroyBall();

            //リセット
            shootButton = false;
        }

        //空に照準が向くと投げられないが、室内にするので問題ないはず
    }

    //ボール削除メソッド
    public void DestroyBall()
    {
        //リストの中で一番古いものを取得
        GameObject oldball = generatedprefab[0];
        //削除＆破壊
        generatedprefab.RemoveAt(0);
        Destroy(oldball);
    }

    //ボタン用
    public void OnButtonDown()
    {
        shootButton = true;
    }

}
