using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UserManager : MonoBehaviour
{

    public InputField nickNameInput;
    private string nickName;

    private Coroutine webrequestCoroutine;

    public string NickName
    {
        get
        {
            return nickName;
        }
        set
        {
            nickName = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (nickNameInput != null)
        {
            //load nick
            if (PlayerPrefs.HasKey(CornUtility.USERNAME_PREFS))
            {
                nickName = PlayerPrefs.GetString(CornUtility.USERNAME_PREFS);
                nickNameInput.text = nickName;
            }
            //nick input
            nickNameInput.onEndEdit.AddListener(delegate
            {
                if (nickNameInput.text.Length > 0)
                {
                    LoginOrRegister(SystemInfo.deviceUniqueIdentifier, nickNameInput.text);
                }
            });
            //nick text update
            nickNameInput.onValueChanged.AddListener(delegate
            {
                UpdateNickInputText();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (nickNameInput != null)
        {
            nickNameInput.interactable = webrequestCoroutine == null;
            if (nickNameInput.isFocused == false)
            {
                UpdateNickInputText();
            }
        }
    }

    void UpdateNickInputText()
    {
        nickNameInput.transform.GetChild(nickNameInput.transform.childCount - 1).GetComponent<Text>().text = nickNameInput.text;
    }

    public void LoginOrRegister(string _deviceId, string _nickname)
    {
        webrequestCoroutine = StartCoroutine(LoginOrRegisterRequest(_deviceId, _nickname));
    }

    IEnumerator LoginOrRegisterRequest(string _deviceId, string _nickname)
    {
        WWWForm loginForm = new();
        loginForm.AddField("loginorsignup", "ok");
        loginForm.AddField("deviceid", _deviceId);
        loginForm.AddField("nickname", _nickname);

        UnityWebRequest loginReq = UnityWebRequest.Post(CornUtility.DATABASE_ADDRESS, loginForm);
        yield return loginReq.SendWebRequest();
        if (loginReq.error == null)
        {
            Debug.Log(loginReq.downloadHandler.text);
            if (loginReq.downloadHandler.text.Contains(":"))
            {
                nickName = loginReq.downloadHandler.text.Split(':')[1];
                if (nickNameInput != null)
                {
                    nickNameInput.text = nickName;
                }
                PlayerPrefs.SetString(CornUtility.USERNAME_PREFS, nickName);
            }
            loginReq.Dispose();
            webrequestCoroutine = null;
        }
        else
        {
            //revert back
            if (nickNameInput != null)
            {
                nickNameInput.interactable = false;
                nickNameInput.text = nickName;
                UpdateNickInputText();
            }
            Debug.Log(loginReq.error);
            loginReq.Dispose();
        }
    }
}
