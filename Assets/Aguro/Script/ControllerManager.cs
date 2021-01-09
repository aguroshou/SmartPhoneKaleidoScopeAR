using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject ballGameObject;
    [SerializeField] private GameObject blackHoleGameObject;
    [SerializeField] private GameObject whiteHoleGameObject;
    [SerializeField] private GameObject CenterAxisGameObject;
    [SerializeField] private GameObject UIGameObject;
    [SerializeField] private GameObject CameraGameObject;
    [SerializeField] private GameObject transparentBlackHoleGameObject;
    [SerializeField] private List<GameObject> transparentBlackHoleGameObjectList;

    //コントローラーの位置はガタガタと震えてしまうため、滑らかにする
    private Transform smoothControllerTransform;

    //コントローラーのトリガーの値を滑らかにする
    private float smoothTriggerValue;

    public delegate void AddBallEventHandler();

    private Color ballRendererStartColor = Color.white;
    private Color ballRendererEndColor = Color.white;

    public delegate void AddBlackHoleEventHandler(GameObject blackHoleGameObject);

    public delegate void AddWhiteHoleEventHandler(GameObject whiteHoleGameObject);

    public event AddBallEventHandler BallAdded;
    public event AddBlackHoleEventHandler BlackHoleAdded;
    public event AddWhiteHoleEventHandler WhiteHoleAdded;
    [SerializeField] private TextMesh controllerModeText;
    private float controllerModeTextAlpha;

    private int resetCount;
    private int resetCountMax = 120;

    [SerializeField] private GameObject colorPaletteSpriteObject;
    [SerializeField] private GameObject ClockBlankSpriteObject;

    //(改善が必要ですが)コントローラーのトリガーの0~1の値を取得するためだけに使用しています…。
    //[SerializeField] private ControllerVisualizer _controllerVisualizer;

    //軸の対称となる数
    private int mirrorCount = 6;

    //1フレーム前にタッチパッドを触っていたか
    private bool isColorpadTouchingBefore;

    //1フレーム前にトリガーを引いていたか
    private bool isTriggerPullingBefore;

    enum ControllerMode
    {
        Kaleidoscope,

        // BlackHole,
        // WhiteHole,
        Menu
    }

    private int controllerModeMax = 2;

    private int controllerMode = (int) ControllerMode.Menu;

    void Start()
    {
        //MoveBallのBlackHoleAddedイベントを追加しなければならないため、最初にボールを出現させてエラーを回避しています。
        //この書き方は良くないです。
        GameObject tmpBall = Instantiate(ballGameObject, transform);
        Destroy(tmpBall, 0.1f);
    }

    void Update()
    {
        // こう書ける
        TouchInfo touchInfo = AppUtil.GetTouch();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateEffect(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeMode();
        }

        //if (_controllerVisualizer.ControllerTriggerValue >= 0.05f)
        //if (Input.GetKeyDown(KeyCode.S))
        if (touchInfo == TouchInfo.Began)
        {
            if (!isTriggerPullingBefore) ;
            {
                isTriggerPullingBefore = true;
                CenterAxisGameObject.transform.position = CameraGameObject.transform.position;
                CenterAxisGameObject.transform.rotation = CameraGameObject.transform.rotation;
                CenterAxisGameObject.transform.Rotate(90.0f, 0.0f, 0.0f);
                CenterAxisGameObject.transform.Translate(0.0f, 5.0f, 0.0f);
                for (int i = 0; i < 4; i++)
                {
                    if (transparentBlackHoleGameObjectList[i])
                    {
                        transparentBlackHoleGameObjectList[i].tag = "None";
                        Destroy(transparentBlackHoleGameObjectList[i], 10.0f);
                    }

                    transparentBlackHoleGameObjectList[i] = Instantiate(transparentBlackHoleGameObject);
                    transparentBlackHoleGameObjectList[i].transform.position = CenterAxisGameObject.transform.position;
                    transparentBlackHoleGameObjectList[i].transform.rotation = CenterAxisGameObject.transform.rotation;
                    transparentBlackHoleGameObjectList[i].transform
                        .Translate(0.0f, ((i - 1) * 2.5f), 0.0f);
                    transparentBlackHoleGameObjectList[i].tag = "BlackHole";
                }
            }

            GenerateEffect(0.5f);
        }
        if (!Input.GetKeyDown(KeyCode.S))
        {
            isTriggerPullingBefore = false;
        }

        controllerModeTextAlpha -= 0.0025f;
        controllerModeText.color = new Color(1.0f, 1.0f, 1.0f, controllerModeTextAlpha);
        //if (_controllerVisualizer.IsControllerTouchpadOperating)
        {
            if (!isColorpadTouchingBefore)
            {
                isColorpadTouchingBefore = true;
                ballRendererEndColor = ballRendererStartColor;
            }

            //_controllerVisualizer.IsControllerTouchpadOperating = false;
            //if (controllerMode == (int) ControllerMode.Kaleidoscope)
            {
                //float tmpSaturation = _controllerVisualizer.ControllerTouchpadSquaredDistance / (0.01f * 0.01f);
                float tmpSaturation = 0.5f;
                tmpSaturation = Mathf.Clamp(tmpSaturation, 0.0f, 1.0f);
                float tmpV = 1.0f / mirrorCount;
                Color touchpadColor = Color.HSVToRGB(180.0f / 360.0f,
                    tmpSaturation, tmpV);
                CenterAxisGameObject.GetComponent<Renderer>().material.color = touchpadColor;
                ballRendererStartColor = touchpadColor;
            }
        }
        // else
        // {
        //     isColorpadTouchingBefore = false;
        // }
    }

    /// <summary>
    /// ボタン押下時の処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="button"></param>
    // void OnButtonDown(
    //     byte controllerId,
    //     MLInput.Controller.Button button)
    // {
    //     switch (button)
    //     {
    //         case MLInput.Controller.Button.Bumper:
    //             if (controllerMode == (int) ControllerMode.Kaleidoscope)
    //             {
    //                 mirrorCount %= 12;
    //                 mirrorCount++;
    //                 controllerModeText.text =
    //                     "流れ星を放つ数：" + mirrorCount.ToString();
    //                 controllerModeTextAlpha = 1.0f;
    //                 transform.localPosition = Vector3.zero;
    //                 transform.localRotation = Quaternion.identity;
    //             }
    //
    //             //ChangeMode();
    //             break;
    //         case MLInput.Controller.Button.HomeTap:
    //             ChangeMode();
    //             //Application.Quit();
    //             break;
    //     }
    // }


    /// <summary>
    /// ボタン押上時の処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="button"></param>
    // void OnButtonUp(
    //     byte controllerId,
    //     MLInput.Controller.Button button)
    // {
    //     switch (button)
    //     {
    //         case MLInput.Controller.Button.Bumper:
    //             break;
    //
    //         case MLInput.Controller.Button.HomeTap:
    //             break;
    //     }
    // }


    /// <summary>
    /// トリガーの押下処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="value"></param>
    // void OnTriggerDown(
    //     byte controllerId,
    //     float value)
    // {
    // }


    /// <summary>
    /// トリガーの押上処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="value"></param>
    // void OnTriggerUp(
    //     byte controllerId,
    //     float value)
    // {
    // }


    /// <summary>
    /// タッチパッドのジェスチャー始点.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="gesture"></param>
    // void OnTouchPadGestureStart(
    //     byte controllerId,
    //     MLInput.Controller.TouchpadGesture gesture)
    // {
//SceneManager.LoadScene("MagicLeapArFoundationReferencePoints");

//resetCount = 0;


    /// <summary>
    /// タッチパッドのジェスチャー操作中.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="gesture"></param>
//     void OnTouchPadGestureContinue(
//         byte controllerId,
//         MLInput.Controller.TouchpadGesture gesture)
//     {
// // resetCount++;
// // if (resetCount>=resetCountMax)
// // {
// //     SceneManager.LoadScene("MagicLeapArFoundationReferencePoints");
// // }
//     }
    void GenerateEffect(float triggerPower)
    {
        switch (controllerMode)
        {
            case (int) ControllerMode.Kaleidoscope:
                AddBall(triggerPower);
                break;
            // case (int) ControllerMode.BlackHole:
            //     AddBlackHole();
            //     break;
            // case (int) ControllerMode.WhiteHole:
            //     AddWhiteHole();
            //     break;
            case (int) ControllerMode.Menu:
                // var tmpTransform = transform;
                // CenterAxisGameObject.transform.position = tmpTransform.position;
                // CenterAxisGameObject.transform.rotation = tmpTransform.rotation;
                // CenterAxisGameObject.transform.Rotate(90.0f, 0.0f, 0.0f);
                // CenterAxisGameObject.transform.Translate(0.0f, 5.0f, 0.0f);
                break;
        }
    }

    void ChangeMode()
    {
        UIGameObject.SetActive(!UIGameObject.activeSelf);
        controllerMode++;
        controllerMode %= controllerModeMax;
        switch (controllerMode)
        {
            case (int) ControllerMode.Kaleidoscope:
                //controllerModeText.text = "流れ星の放つ数：" + mirrorCount.ToString();
                // controllerModeText.text = "<万華鏡モード>\nトリガー：流れ星を放つ\nトラックパッド：色を変更する";
                // ClockBlankSpriteObject.SetActive(false);
                // colorPaletteSpriteObject.SetActive(true);
                break;
            // case (int) ControllerMode.BlackHole:
            //     controllerModeText.text = "エフェクトの種類\nブラックホール";
            //     break;
            // case (int) ControllerMode.WhiteHole:
            //     controllerModeText.text = "エフェクトの種類\nホワイトホール";
            //     break;
            case (int) ControllerMode.Menu:
                // controllerModeText.text =
                //     "<設定モード>\nトリガー：万華鏡の中心軸を配置しなおす\nトラックパッド：鏡の数を変更する\n鏡の数：" + mirrorCount.ToString();
                // ClockBlankSpriteObject.SetActive(true);
                // colorPaletteSpriteObject.SetActive(false);
                break;
        }

//controllerModeTextAlpha = 1.0f;
    }

    void AddBall(float triggerValue)
    {
        for (int i = 0;
            i < mirrorCount;
            i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpBall =
                GameObject.Instantiate(ballGameObject) as GameObject;
            tmpBall.transform.position = tmpTransform.position;
            tmpBall.transform.rotation = tmpTransform.rotation;
            //tmpBall.transform.parent = CenterAxisGameObject.transform;
            TrailRenderer tmpBallTrailRenderer;
            tmpBallTrailRenderer = tmpBall.GetComponent<TrailRenderer>();
            tmpBallTrailRenderer.startColor = ballRendererStartColor;
            tmpBallTrailRenderer.endColor = ballRendererEndColor;

            tmpBall.GetComponent<Rigidbody>().velocity =
                transform.forward * (5 * triggerValue);
            //BallAdded();
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up,
                360 / mirrorCount);
            float tmpDestroyTime = 60.0f / mirrorCount;
            tmpDestroyTime = Mathf.Min(tmpDestroyTime, 15.0f);
            Destroy(tmpBall, tmpDestroyTime);
        }
    }

    void AddBlackHole()
    {
        for (int i = 0;
            i < mirrorCount;
            i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpBlackHole =
                GameObject.Instantiate(blackHoleGameObject) as GameObject;
            tmpBlackHole.transform.parent = CenterAxisGameObject.transform;
            tmpBlackHole.transform.position = transform.position;
            BlackHoleAdded(tmpBlackHole);
            tmpBlackHole.transform.position = transform.position;
            tmpBlackHole.transform.rotation = transform.rotation;
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up,
                360 / mirrorCount);
            Destroy(tmpBlackHole, 20.0f);
        }
    }

    void AddWhiteHole()
    {
        for (int i = 0;
            i < mirrorCount;
            i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpWhiteHole =
                GameObject.Instantiate(whiteHoleGameObject) as GameObject;
            tmpWhiteHole.transform.parent = CenterAxisGameObject.transform;
            tmpWhiteHole.transform.position = transform.position;
            WhiteHoleAdded(tmpWhiteHole);
            tmpWhiteHole.transform.position = transform.position;
            tmpWhiteHole.transform.rotation = transform.rotation;
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up,
                360 / mirrorCount);
            Destroy(tmpWhiteHole, 20.0f);
        }
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}