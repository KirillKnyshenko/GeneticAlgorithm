using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveItem : MonoBehaviour
{
    [SerializeField] private RawImage preview;
    [SerializeField] private TMP_Text text;


    public void Delete()
    {
        File.Delete(SaveManager.savesFolder + @$"/{text.text}.json");
        GetComponentInParent<SavesList>().UpdateList();
    }

    public void LoadSave()
    {
        PlayerPrefs.SetString("LoadedSave", text.text);
        SceneManager.LoadScene(1);
    }

    public void Init(string getFileNameWithoutExtension)
    {
        text.text = getFileNameWithoutExtension;
        SaveManager.Data data = JsonConvert.DeserializeObject<SaveManager.Data>(File.ReadAllText(SaveManager.savesFolder + $"/{getFileNameWithoutExtension}.json"));

        Debug.LogError(SaveManager.savesFolder + $"/{getFileNameWithoutExtension}.json");

        var texture = new Texture2D(data.worldData.size, data.worldData.size);
        texture.LoadRawTextureData(data.texture);
        texture.Apply();

        preview.texture = texture;
    }
}
