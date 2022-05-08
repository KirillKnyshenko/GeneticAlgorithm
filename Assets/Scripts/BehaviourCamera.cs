using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourCamera : MonoBehaviour
{
    private Camera _camera;
    private Cell _cell;
    private float _cameraMaxSize;


    [SerializeField] private int scrollSpeed, moveSpeed;
    [SerializeField] private int _regenerationCount;
    [SerializeField] private Transform _selected;
    [SerializeField] private SpriteRenderer _selectedSpriteRenderer;
    [SerializeField] private GameObject _panelMenu;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        _camera = Camera.main;
        transform.position = (new Vector3(Manager.Instance.world.size, Manager.Instance.world.size) / 2) - Vector3.forward;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _cameraMaxSize = Manager.Instance.world.size / 2;
        _camera.orthographicSize = _cameraMaxSize;
    }

    private void LateUpdate()
    {
        ReselectCell();
        UpdateCellInfo();
        CameraSize();
        CameraPosition();
        MiniMenu();
    }
    
    private void MiniMenu() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _panelMenu.SetActive(!_panelMenu.active);
        }
    }
    
    private void CameraSize() 
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            float sizeChange = Input.mouseScrollDelta.y * Time.unscaledDeltaTime * scrollSpeed;
            _camera.orthographicSize -= sizeChange;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 3, _cameraMaxSize);
        }
    }
    
    private void CameraPosition() 
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 direction = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * Time.unscaledDeltaTime * moveSpeed;
            _camera.transform.position -= direction;

            float x = Mathf.Clamp(_camera.transform.position.x, 0, Manager.Instance.world.size);
            float y = Mathf.Clamp(_camera.transform.position.y, 0, Manager.Instance.world.size);
            _camera.transform.position = new Vector3(x, y,  _camera.transform.position.z);
        }
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
