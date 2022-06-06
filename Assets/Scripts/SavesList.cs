using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavesList : MonoBehaviour
{
    [SerializeField] private Transform holder, item;


    private void Start()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        var files = Directory.GetFiles(Application.dataPath, "*.json");

        foreach (Transform it in holder)
        {
            if (it.gameObject != item.gameObject)
            {
                Destroy(it.gameObject);
            }
        }


        for (int i = 0; i < files.Length; i++)
        {
            var it = Instantiate(item.gameObject, Vector3.zero, Quaternion.identity, holder);
            var  saveItem = it.GetComponent<SaveItem>();

            saveItem.Init(Path.GetFileNameWithoutExtension(files[i]));
            saveItem.gameObject.SetActive(true);
        }
    }
}
