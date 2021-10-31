using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunController : MonoBehaviour
{
    //Rayを飛ばせる距離
    private float shootRange = 50;

    //発射間隔
    private float shootInterval = 0.15f;

    //発射ボタンを押しているかどうかの判定
    private bool shootButton = false;

    //撃っているかどうかの判定
    private bool shooting = false;

    //着弾エフェクト
    public GameObject hitEffectPrefab;
    private GameObject hitEffect;

    //銃口の発射エフェクト
    public GameObject muzzleFlashPrefab;
    private GameObject muzzleFlash;
    //エフェクトの大きさ
    private Vector3 muzzleFlashScale = new Vector3(1.3f, 1.3f, 1.3f);

    //銃口の位置
    private GameObject Muzzle;

    //銃声
    public AudioClip[] SE;
    private AudioSource audiosource;

    //銃(MG main)の向きを格納する変数
    private Vector2 newAngle = new Vector2(0, 0);

    //マウス座標を格納する変数
    private Vector2 lastMousePosition;

    //回転速度を格納
    private Vector2 rotationSpped = new Vector2(0.03f, 0.03f);

    //照準
    public GameObject Aim;


    // Start is called before the first frame update
    void Start()
    {
        //コンポーネント取得
        audiosource = GetComponent<AudioSource>();
        //銃口オブジェクト（子）取得
        Muzzle = transform.Find("Muzzle").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンが押されている間は発射
        if (shootButton == true) StartCoroutine(ShootTimer());

        //以下銃の向きをドラッグで操作
        //左クリックした時
        if (Input.GetMouseButtonDown(0))
        {
            //銃の角度を格納
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

    //弾発射メソッド
    public void Shoot()
    {
        audiosource.PlayOneShot(SE[0]);

        /* Ray(Rayの発生地点,進む方向）
         *今回の場合Ray(スクリプトをアタッチしているGameObjectの位置,向いてる方向)*/      
        Ray ray = new Ray(transform.position, transform.forward);
        //hitにRayが当たったオブジェクトの情報が格納される
        RaycastHit hit;
        //格納した情報を取得
        if(Physics.Raycast(ray,out hit,shootRange))
        {
            //着弾エフェクトを表示
            if (hitEffectPrefab != null)
            {
                if(hitEffect!=null)
                {
                    hitEffect.transform.position = hit.point;
                    /*FromToRotation(開始方向,終了方向)、開始方向を終了方向へ最小の回転で向ける
                     *Vector3.forwardはVector3(0,0,1)と同じ
                     *hit.normalは着弾した面の方向を返す*/
                    hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    hitEffect.SetActive(true);
                }
                else
                {
                    //エフェクトが出てない場合は着弾点にエフェクトを生成
                    hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
                }
            }

            //着弾対象へのダメージ処理などの追加欄             
        }
    }

    //コルーチン実行用
    private IEnumerator ShootTimer()
    {
        //撃っていない場合
        if (shooting == false)
        {
            shooting = true;

            //銃口の発射エフェクトを表示
            if (muzzleFlashPrefab != null)
            {
                if (muzzleFlash != null)
                {
                    muzzleFlash.SetActive(true);
                }
                else
                {
                    muzzleFlash = Instantiate(muzzleFlashPrefab, Muzzle.transform.position, Muzzle.transform.rotation);
                    muzzleFlash.transform.SetParent(Muzzle.transform);
                    //エフェクトの大きさ指定
                    muzzleFlash.transform.localScale = muzzleFlashScale;
                }
            }
            //弾発射
            Shoot();

            //指定秒数処理を止める
            yield return new WaitForSeconds(shootInterval);

            //銃口の発射エフェクトを非表示
            if (muzzleFlash != null) muzzleFlash.SetActive(false);

            //着弾エフェクトを非表示
            //activeSelfはアクティブかどうかを判定するもの
            if (hitEffect != null && hitEffect.activeSelf) hitEffect.SetActive(false);

            shooting = false;
        }
        //撃ってる最中は1フレーム処理を中断してから再開する
        else yield return null;
    }

    public void OnButtonDown()
    {
        shootButton = true;
    }
    public void OnButtonUp()
    {
        shootButton = false;
    }
}
