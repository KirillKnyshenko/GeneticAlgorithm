using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _textColorMod;
    [SerializeField] private TMP_Text _textCount;
    public void ChangeColor()
    {
        Manager.Instance.ChangeColor();
        
        switch (Manager.Instance.colorMod)
        {
            case Manager.ViewColor.GenColor:
                _textColorMod.text = "Режим просмотра:\nГенетический";
                break;
            case Manager.ViewColor.TypeColor:
                _textColorMod.text = "Режим просмотра:\nТипу пищи";
                break;
            case Manager.ViewColor.EnergyColor:
                _textColorMod.text = "Режим просмотра:\nЭнергетический";
                break;
        }
    }
    
    public void ChangeCount(int c, int n, int o)
    {
        _textCount.text = "Травоядных: " + n + "\nПлотоядных: " + c + "\nВсеядных: " + o + "\nВсего: " + (c + n + o);
    }
}
