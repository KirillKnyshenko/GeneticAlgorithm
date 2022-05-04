using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _textColorMod;
    [SerializeField] private TMP_Text _textCount;
    [SerializeField] private TMP_Text _textTimeScale;
    [SerializeField] private Slider _timeSlider;
    
    [SerializeField] private TMP_Text _textcellColor;
    [SerializeField] private Image _cellColor;
    [SerializeField] private TMP_Text _textCellType;
    [SerializeField] private TMP_Text _textCellPosition;
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
    
    public void ChangeTimeScale()
    {
        Time.timeScale = _timeSlider.value * 100;
        _textTimeScale.text = "Скорость тиков: " + Mathf.RoundToInt(_timeSlider.value * 100);
    }

    public void CellInfo()
    {
        _textCellPosition.text = "";
        _cellColor.color = Color.white;
        _textcellColor.text = "";
        _textCellType.text = "";
    }
    
    public void CellInfo(Cell cell)
    {
        _textcellColor.text = "Генетический цвет";
        _cellColor.color = cell.GenColor;
        _textCellType.text = "Тип:\n" + cell.Type;
        _textCellPosition.text = "Координаты:\n" + cell.Position;
    }
}
 