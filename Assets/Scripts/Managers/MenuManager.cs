using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    public World world;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Slider _worldSize;
    [SerializeField] private Slider _countOfBorn;
    [SerializeField] private Slider _maxEnergy;
    [SerializeField] private Slider _actionEnergy;
    [SerializeField] private Slider _relatedness;
    [SerializeField] private Slider _yearsOldMax;
    [SerializeField] private Slider _chanceMutate;
    [SerializeField] private Slider _babyCost;

    private void Start()
    {
        Instance = this;
    }

    public void SetActivePanel()
    {
        _panel.SetActive(!_panel.active);
        _worldSize.value = world.size;
        _countOfBorn.value = world.countOfBorn;
        _maxEnergy.value = world.maxEnergy;
        _actionEnergy.value = world.actionEnergy;
        _relatedness.value = world.relatedness;
        _yearsOldMax.value = world.yearsOldMax;
        _chanceMutate.value = world.chanceMutate;
        _babyCost.value = world.babyCost;
    }
    
    public void StartSimulation()
    {
        world.size = (int)_worldSize.value;
        world.countOfBorn = (int)_countOfBorn.value;
        world.maxEnergy = (int)_maxEnergy.value;
        world.actionEnergy = (int)_actionEnergy.value;
        world.relatedness = (int)_relatedness.value;
        world.yearsOldMax = (int)_yearsOldMax.value;
        world.chanceMutate = _chanceMutate.value;
        world.babyCost = _babyCost.value;
        LoadScene(1);
    }

    public void LoadScene(int number)
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(number);
    }
    
    public void LoadSave()
    {
        PlayerPrefs.SetInt("Key", 1);
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    public void Save()
    {
        PlayerPrefs.SetInt("Key", 1);
        SaveManager.Save();
    }
}
