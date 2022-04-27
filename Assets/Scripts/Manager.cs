using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    
    public World world;
    public Cell[,] cells;
    [SerializeField] private List<Cell> _poolCell = new List<Cell>();
    private List<Cell> _activeCells = new List<Cell>();
    
    [SerializeField] private GameObject _cell;
    [SerializeField] private Transform _parentOfCell;

    public void Awake()
    {
        Instance = this;
        cells = new Cell[world.size, world.size];
        StartPool();
        StartCoroutine(Tick());
    }

    public void Update()
    {
        if (_activeCells.Count == 0)
        {
            NewLife();
        }
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            ActiveCellTurn();
            yield return new WaitForSeconds(1f/4);
        }
    }

    public void StartPool()
    {
        for (int i = 0; i < world.size * world.size; i++)
        {
            var inst = Instantiate(_cell, _parentOfCell);
            Cell cell = new Cell(inst.transform);
            _poolCell.Add(cell);
        }
    }
    
    public void BackToPool(Cell cell)
    {
        _poolCell.Add(cell);
    }
    
    public Cell GetFromPool()
    {
        if (_poolCell.Count > 0)
        {
            Cell cell = _poolCell[0];
            AddActiveCell(_poolCell[0], _poolCell[0].Position);
            _poolCell.RemoveAt(0);
            return cell;
        }
        return null;
    }
    
    public void ActiveCellTurn()
    {
        List<int> cellToDead = new List<int>();
        
        for (int i = 0; i < _activeCells.Count; i++)
        {
            if (!_activeCells[i].IsDead)
            {
                _activeCells[i].Action();
            }
            else
            {
                cellToDead.Add(i);
            }
        }
        
        for (int i = 0; i < cellToDead.Count; i++)
        {
            _activeCells.RemoveAt(cellToDead[i]-i);
        }
    }
    
    private void NewLife()
    {
        if (!world.isBirthAfterDeath)
        {
            ClearWorld();
        }
        
        for (int i = 0; i < world.countOfBorn; i++)
        {
            CreateCell();
        }
    }

    public void CreateCell()
    {
        var cell = GetFromPool();

        cell.Initialization(world.maxGens, RandomPointCell(), null, world.maxEnergy);
    }
    
    public void CreateCell(Vector2Int position, float energy)
    {
        var cell = GetFromPool();

        cell.Initialization(world.maxGens, position, null, energy);
    }

    public Vector2Int RandomPointCell()
    {
        Vector2Int position;
        
        do
        {
            var x = Random.Range(0, world.size);
            var y = Random.Range(0, world.size);
            position = new Vector2Int(x, y);
        } while (cells[position.x, position.y] != null || cells[position.x, position.y]?.IsDead == true);

        if (cells[position.x, position.y] != null)
        {
            RemoveCell(position);
        }
        
        return position;
    }

    public Cell CheckCell(Vector2Int position)
    {

        if (cells[position.x, position.y] == null)
        {
            return null;
        }
        else
        {
            return cells[position.x, position.y];
        }
    }

    public void AddActiveCell(Cell cell, Vector2Int position)
    {
        _activeCells.Add(cell);
        cells[position.x, position.y] = cell;
    }
    
    public void RemoveActiveCell(Cell cell)
    {
        _activeCells.Remove(cell);
    }
    
    public void RemoveCell(Vector2Int position)
    {
        if (cells[position.x, position.y] != null)
        {
            print(position);
            cells[position.x, position.y].RemoveVisual();
            BackToPool(cells[position.x, position.y]);
            cells[position.x, position.y] = null;
        }
    }

    public void ClearWorld()
    {
        for (int i = 0; i < world.size; i++)
        {
            for (int j = 0; j < world.size; j++)
            {
                if (cells[i, j] != null)
                {
                    RemoveActiveCell(cells[i, j]);
                    RemoveCell(cells[i, j].Position);
                }
            }
        }
    }

    public Vector2Int CheckOfFrame(Vector2Int position)
    {
        if (world.size <= position.x)
        {
            position.x = 0;
        }
        else if (position.x < 0)
        {
            position.x = world.size - 1;
        }

        if (world.size <= position.y)
        {
            position.y = 0;
        }
        else if (position.y < 0)
        {
            position.y = world.size - 1;
        }

        return position;
    }
}
