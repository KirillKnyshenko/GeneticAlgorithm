using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    
    public World world;
    public Cell[,] cells;
    private List<Cell> _activeCells = new List<Cell>();
    private List<Cell> _passiveCells = new List<Cell>();
    [SerializeField] private GameObject _cell;
    [SerializeField] private Transform _parentOfCell;

    public void Awake()
    {
        Instance = this;
        cells = new Cell[world.size, world.size];
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
            PassiveCellTurn();
            yield return new WaitForSeconds(1f/4);
        }
    }

    public void ActiveCellTurn()
    {
        for (int i = 0; i < _activeCells.Count; i++)
        {
            _activeCells[i].Action();
        }
    }
    
    public void PassiveCellTurn()
    {
        for (int i = 0; i < _passiveCells.Count; i++)
        {
            
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
        var cell = new Cell();

        cell.Initialization(world.maxGens, RandomPointCell(), null, world.maxEnergy);
    }
    
    public void CreateCell(Vector2Int position, float energy)
    {
        var cell = new Cell();

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
        } while (CheckActiveCell(position) != null);

        if (cells[position.x, position.y] != null)
        {
            RemoveCell(position);
        }
        
        return position;
    }

    public Cell CheckCell(Vector2Int position)
    {
        position = CheckOfFrame(position);
        
        if (cells[position.x, position.y] == null)
        {
            return null;
        }
        else
        {
            return cells[position.x, position.y];
        }
    }

    public Cell CheckActiveCell(Vector2Int position)
    {
        position = CheckOfFrame(position);
        return _activeCells.Find(a => a.Position == new Vector2Int(position.x, position.y));
    }
    
    public GameObject DrawCell(Vector2Int position)
    {
        return Instantiate(_cell, (Vector2)position, Quaternion.identity, _parentOfCell);
    }
    
    public void AddActiveCell(Cell cell, Vector2Int position)
    {
        _activeCells.Add(cell);
        cells[position.x, position.y] = cell;
    }
    
    public void RemoveActiveCell(Cell cell)
    {
        _activeCells.Remove(cell);
        _passiveCells.Add(cell);
    }
    
    public void RemoveCell(Vector2Int position)
    {
        if (cells[position.x, position.y] != null)
        {
            _passiveCells.Remove(cells[position.x, position.y]);
            cells[position.x, position.y].RemoveVisual();
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
