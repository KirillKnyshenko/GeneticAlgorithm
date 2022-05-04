using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCamera : MonoBehaviour
{
    private Camera _camera;
    private Cell _cell;
    
    [SerializeField] private int _regenerationCount;
    [SerializeField] private Transform _selected;
    [SerializeField] private SpriteRenderer _selectedSpriteRenderer;
    
    private void Start()
    {
        _camera = Camera.main;
        transform.position = (new Vector3(Manager.Instance.world.size, Manager.Instance.world.size) / 2) - Vector3.forward;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _camera.orthographicSize = Manager.Instance.world.size / 2;
    }

    private void LateUpdate()
    {
        ReselectCell();
        UpdateCellInfo();
    }

    private void ReselectCell()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2Int mousePosition = new Vector2Int(Mathf.RoundToInt(_camera.ScreenToWorldPoint(Input.mousePosition).x),
                Mathf.RoundToInt(_camera.ScreenToWorldPoint(Input.mousePosition).y));

            if (Manager.Instance.cells != null)
            {
                if (mousePosition.x >= 0 && mousePosition.x < Manager.Instance.world.size)
                {
                    if (mousePosition.y >= 0 && mousePosition.y < Manager.Instance.world.size)
                    {
                        if (Manager.Instance.cells[mousePosition.x, mousePosition.y] != null)
                        {
                            _cell = Manager.Instance.cells[mousePosition.x, mousePosition.y];
                            _regenerationCount = _cell.regenerationCount;
                            return;
                        }
                        
                        _cell = null;
                    }
                }
            }
        }
    }

    private void UpdateCellInfo()
    {
        _selectedSpriteRenderer.enabled = false;
        if (_cell == null) return; 
        
        if (_regenerationCount == _cell.regenerationCount && _cell.isInWorld)
        {
            _selectedSpriteRenderer.enabled = true;
            Manager.Instance.UIManager.CellInfo(_cell);
            _selected.position = new Vector3(_cell.Position.x, _cell.Position.y, -0.5f);
        }
        else
        {
            Manager.Instance.UIManager.CellInfo();
        }
    }
}
