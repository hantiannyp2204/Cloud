using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupSettings : MonoBehaviour
{
    [SerializeField] private GameObject deleteBtn;
    [SerializeField] private GameObject deleteConfirmBtn;
    [SerializeField] private TMP_Text statusTxt;
    bool isDeleting = false;
    // Start is called before the first frame update
    void Start()
    {
        statusTxt.text = "";
        ResetButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetButton()
    {
        statusTxt.text = "";
        isDeleting = false;
        deleteConfirmBtn.SetActive(false);
        deleteBtn.SetActive(true);
    }
    public void SwapButtons()
    {
        if(isDeleting == false)                                 
        {
            statusTxt.text = "Are you sure you want to delete the group\n(This cannot be reverted)";
            deleteConfirmBtn.SetActive(true); 
            deleteBtn.SetActive(false);
        }
        else
        {
            deleteConfirmBtn.SetActive(false);
            deleteBtn.SetActive(true);
        }
        isDeleting = !isDeleting;
    }
}
