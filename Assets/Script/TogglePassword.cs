using Microsoft.Unity.VisualStudio.Editor;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TogglePassword : MonoBehaviour
{
    [SerializeField]
    TMP_InputField userPassword;

    UnityEngine.UI.Image image;
    bool passwordShow = true;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        OnPasswordToggle();
    }

    public void OnPasswordToggle()
    {
        //hide pw
        if (passwordShow == true)
        {
            image.color = new Vector4(1,1,1, 0.5f);
            userPassword.contentType = TMP_InputField.ContentType.Password;
            userPassword.ForceLabelUpdate();
            passwordShow = false;
        }
        //show pw
        else
        {
            image.color = new Vector4(1, 1, 1, 1);
            userPassword.contentType = TMP_InputField.ContentType.Standard;
            userPassword.ForceLabelUpdate();
            passwordShow = true;
        }
    }
}
