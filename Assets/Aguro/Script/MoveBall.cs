using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    [SerializeField] private List<GameObject> whiteHole;
    [SerializeField] private List<GameObject> blackHole;
    private Vector3 pastPosition;

    public GameObject planet; // 引力の発生する星

    // public GameObject MainCamera;
    // public float accelerationScale; // 加速度の大きさ
    // public AnemoneManager anemoneManager;
    // private Vector3 Player_pos; //プレイヤーのポジション
    // public long AirtapCountPast = 0;
    [SerializeField] private ControllerManager _controllerManager;

    void Start()
    {
        GameObject[] tmpBlackHoles = GameObject.FindGameObjectsWithTag("BlackHole");
        for (int i = 0; i < tmpBlackHoles.Length; i++)
        {
            blackHole.Add(tmpBlackHoles[i]);
        }

        GameObject[] tmpWhiteHoles = GameObject.FindGameObjectsWithTag("WhiteHole");
        for (int i = 0; i < tmpWhiteHoles.Length; i++)
        {
            whiteHole.Add(tmpWhiteHoles[i]);
        }

        _controllerManager = GameObject.Find("ControllerManager").GetComponent<ControllerManager>();

        _controllerManager.BlackHoleAdded += tmpBlackHoleGameObject => { blackHole.Add(tmpBlackHoleGameObject); };
        _controllerManager.WhiteHoleAdded += tmpWhiteHoleGameObject => { whiteHole.Add(tmpWhiteHoleGameObject); };

        pastPosition = transform.position;
    }

    void Update()
    {
        for (int i = 0; i < blackHole.Count; i++)
        {
            if (!blackHole[i])
            {
                blackHole.Remove(blackHole[i]);
                continue;
            }

            // 星に向かう向きの取得
            var direction = blackHole[i].transform.position - transform.position;
            direction.Normalize();
            // 加速度与える
            float gravityDistance = (transform.position - blackHole[i].transform.position).sqrMagnitude;
            GetComponent<Rigidbody>().AddForce(direction / (gravityDistance + 1.0f), ForceMode.Acceleration);
        }

        for (int i = 0; i < whiteHole.Count; i++)
        {
            // 星に向かう向きの取得
            var direction = whiteHole[i].transform.position - transform.position;
            direction.Normalize();
            // 加速度与える
            float gravityDistance = (transform.position - whiteHole[i].transform.position).sqrMagnitude;
            GetComponent<Rigidbody>().AddForce(-direction / (gravityDistance + 1.0f), ForceMode.Acceleration);
        }

        Vector3 diff = transform.position - pastPosition; //プレイヤーがどの方向に進んでいるかがわかるように、初期位置と現在地の座標差分を取得
        if (diff.magnitude > 0.001f) //ベクトルの長さが0.001fより大きい場合にプレイヤーの向きを変える処理を入れる(0では入れないので）
        {
            transform.rotation = Quaternion.LookRotation(diff); //ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる
        }

        pastPosition = transform.position; //プレイヤーの位置を更新
        // if (anemoneManager.anemoneCount - AirtapCountPast >= 20)
        // {
        //     Destroy(this.gameObject);
        // }
    }
}