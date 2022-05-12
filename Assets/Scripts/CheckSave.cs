using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CheckSave : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        ChangeLoadButton();
    }

    public void ChangeLoadButton()
    {
        if (File.Exists(Application.dataPath + "/data.json"))
        {
            _button.interactable = true;
        }
    }
}
