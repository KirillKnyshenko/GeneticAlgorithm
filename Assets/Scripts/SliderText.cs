using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private Slider _slider;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        ChangeValue();
    }

    public void ChangeValue()
    {
        _text.text = _slider.value.ToString("F2");
    }
    
    public void ChangeValue(bool isInt)
    {
        if (isInt)
        {
            _text.text = Mathf.RoundToInt(_slider.value).ToString();
        }
        else
        {
            _text.text = _slider.value.ToString("F2");
        }
    }
}
