using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    
    public World world;
    public Cell[,] cells;
    private List<Cell> _poolCell = new List<Cell>();
    private List<Cell> _activeCells = new List<Cell>();
    
    public enum ViewColor {GenColor, TypeColor, EnergyColor}

    public ViewColor colorMod;
        
    [SerializeField] private GameObject _cell;
    [SerializeField] private Transform _parentOfCell;

    public void Awake()
    {
        colorMod = ViewColor.GenColor;
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
            yield return new WaitForSeconds(1f/2);
            
            List<int> cellToDead = new List<int>();
        
            for (int i = 0; i < _activeCells.Count; i++)
            {
                if (_activeCells[i] != null)
                {
                    if (!_activeCells[i].isDead)
                    {
                        _activeCells[i].Action();
                    }
                    else
                    {
                        cellToDead.Add(i);
                    }
                }
                else
                {
                    cellToDead.Add(i);
                }
                
                if (i % 5000 == 0)
                {
                    yield return null;
                }
            }

            for (int i = 0; i < cellToDead.Count; i++)
            {
                _activeCells.RemoveAt(cellToDead[i]-i);
            }
        }
    }

    public void StartPool()
    {
        _poolCell = new List<Cell>(world.size * world.size);
        for (int i = 0; i < world.size * world.size; i++)
        {
            var inst = Instantiate(_cell, _parentOfCell);
            Cell cell = new Cell(inst.transform);
            cell.RemoveVisual();
            _poolCell.Add(cell);
        }
    }
    
    public void BackToPool(Cell cell)
    {
        _poolCell.Add(cell);
    }
    
    public Cell GetFromPool(Vector2Int position)
    {
        if (_poolCell.Count > 0)
        {
            Cell cell = _poolCell[_poolCell.Count - 1];
            AddActiveCell(cell, position);
            _poolCell.RemoveAt(_poolCell.Count - 1);
            return cell;
        }
        return null;
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
        Vector2Int position = RandomPointCell();
        var cell = GetFromPool(position);

        cell.Initialization(world.maxGens, position, null, world.maxEnergy);
    }
    
    public void CreateCell(Vector2Int position, float energy, byte[] gens)
    {
        var cell = GetFromPool(position);

        cell.Initialization(world.maxGens, position, Mutation(gens), energy);
    }

    public byte[] Mutation(byte[] gens)
    {
        byte[] childrenGen = new byte[world.maxGens];

        for (int i = 0; i < gens.Length; i++)
        {
            childrenGen[i] = gens[i];
        }
        
        if (Random.Range(0, 1f) <= world.chanceMutate)
        {
            int mutateGen = Random.Range(0, world.maxGens);

            childrenGen[mutateGen] = (byte) Random.Range(0, 10);
        }

        return childrenGen;
    }
    
    public Vector2Int RandomPointCell()
    {
        Vector2Int position;
        
        do
        {
            var x = Random.Range(0, world.size);
            var y = Random.Range(0, world.size);
            position = new Vector2Int(x, y);
        } while (cells[position.x, position.y] != null || cells[position.x, position.y]?.isDead == true);

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
        
        return cells[position.x, position.y];
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
    
    public bool KillActiveCell(Cell cell)
    {
        int index = _activeCells.FindIndex(x => x == cell);
        if (index != -1)
        {
            _activeCells[_activeCells.FindIndex(x => x == cell)] = null;
            return true;
        }

        return false;
    }
    public void RemoveCell(Vector2Int position)
    {
        if (cells[position.x, position.y] != null)
        {
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

    public void ChangeColor()
    {
        switch (colorMod)
        {
            case ViewColor.GenColor:
                colorMod = ViewColor.TypeColor;
                break;
            case ViewColor.TypeColor:
                colorMod = ViewColor.EnergyColor;
                break;
            case ViewColor.EnergyColor:
                colorMod = ViewColor.GenColor;
                break;
        }

        for (int i = 0; i < _activeCells.Count; i++)
        {
            if (_activeCells[i] != null)
            {
                _activeCells[i].DrawColor();
            }
        }
    }
}
