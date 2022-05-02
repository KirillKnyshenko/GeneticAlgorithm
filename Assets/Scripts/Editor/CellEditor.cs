using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellDisplay))]
public class CellEditor : Editor
{
    private CellDisplay _cellDisplay;
    private Cell _cell;

    private void OnEnable()
    {
        _cellDisplay = target as CellDisplay;
        _cell = Manager.Instance.cells[(int)_cellDisplay.transform.position.x, (int)_cellDisplay.transform.position.y];
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_cell != null)
        {
            GUILayout.Label("Умер");
            GUILayout.Label(_cell.isDead.ToString());
            
            GUILayout.Label("Сумма генов");
            GUILayout.Label(_cell.SumOfGens.ToString());
            
            GUILayout.Label("Энергия");
            GUILayout.Label(_cell.GetEnergy().ToString());
            
            GUILayout.Label("Возраст");
            GUILayout.Label(_cell.YearsOld.ToString());

            GUILayout.Label("Позиция");
            GUILayout.Label("(" + _cell.Position.x + ", " + _cell.Position.y + ")");
            
            GUILayout.Label("Гены");
            string gens = "[";
            for (int i = 0; i < Manager.Instance.world.maxGens; i++)
            {
                gens = gens + _cell.gens[i].ToString();
                if (i == Manager.Instance.world.maxGens - 1)
                {
                    gens += "]";
                }
                else
                {
                    gens += ", ";
                }
            }
            GUILayout.Label(gens);
            
            GUILayout.Label("Тип");
            GUILayout.Label(_cell.Type.ToString());
        }
    }
}
