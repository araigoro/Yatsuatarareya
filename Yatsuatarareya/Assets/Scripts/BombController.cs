using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    //爆発エフェクト
    public GameObject explosionEffect;

    //爆発音
    public AudioClip SE;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //爆弾オブジェクトを破壊
        Destroy(this.gameObject);
        //着弾点にエフェクト生成
        Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        /*爆発音再生
         * PlayOneShotだとオブジェクトと一緒にaudiosourceも消えるので、
         * PlayClipAtPointで一時的に新しいオブジェクトを生成し、鳴らしてから破壊する
         *(効果音,生成する位置)、カメラの位置に生成した時に音量最大*/
        AudioSource.PlayClipAtPoint(SE, new Vector3(0, 2.49f, -8));
    }
}
