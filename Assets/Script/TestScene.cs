
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TestScene : MonoBehaviour
{
    //Start is called before the first frame update
    void Start()
    {

        Debug.Log("TestScene===Start");

        var btn1 = GameObject.Find("btn1");
        var btn2 = GameObject.Find("btn2");
        var image = GameObject.Find("Image");
        var Text = GameObject.Find("Text");

        //按钮点击事件
        btn1.GetComponent<Button>().onClick.AddListener(delegate() {

            Debug.Log("btn1");
            StartCoroutine(GetRequest());
            StartCoroutine(DownloaAssetBundle());

            if (Application.platform == RuntimePlatform.Android)
            {
                var playerCls = new AndroidJavaClass("com.example.mylibrary.TestModule");
                string SayHello = playerCls.CallStatic<string>("SayHello");
                Debug.Log("SayHello" + SayHello);
                Text.GetComponent<Text>().text = SayHello;
            }



        });

        //图片切换
        btn2.GetComponent<Button>().onClick.AddListener(delegate () {

            Debug.Log("btn2");
            Sprite sprite = Resources.Load("ic_launcher", typeof(Sprite)) as Sprite;
            image.GetComponent<Image>().sprite = sprite;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DownloaAssetBundle()
    {
        string uri = @"http://192.168.65.151/AssetBundles/Win32/test";//ab包路径
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);//获取UnityWebReuest
        yield return request.SendWebRequest();//等待获取完成
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);//获取ab包
        GameObject cube = bundle.LoadAsset<GameObject>("C_IM1");
        
        Instantiate(cube);
    
    }

    public IEnumerator GetRequest()
    {
        // 请求的 url
        string url = "http://lee.free.vipnps.vip/hotupversion/configrelease";//你要请求的url地址
        //是什么请求就调用什么方法，如果是 post/put 方法，还需要传递一个 string 类型的数据
        // ///可以用 JSON 工具类将对象封装成 JSON
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {

            yield return webRequest.SendWebRequest();

            // 出现网络错误
            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
            }
            else// 正常处理
            {
                // 获取到的数据
                string jsonText = webRequest.downloadHandler.text;
                //Debug.Log(jsonText);


                //json 解析
                JObject obj = JObject.Parse(jsonText);
                Debug.Log(obj["scriptVersion"]);

            }
        }
    }
}
