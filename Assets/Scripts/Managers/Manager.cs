using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public UIManager UIManager;
    
    public World world;
    public int carnivores, herbivorous, omnivorous;
    public int born, ticks;
    public Cell[,] cells;

    private List<Cell> _poolCell = new List<Cell>();
    private List<Cell> _activeCells = new List<Cell>();

    public enum ViewColor {GenColor, TypeColor, EnergyColor}

    public ViewColor colorMod;
        
    [SerializeField] private GameObject _cell;
    [SerializeField] private Transform _worldPlace;
    [SerializeField] private Transform _parentOfCell;

    public void Awake()
    {
        Instance = this;
        colorMod = ViewColor.GenColor;
        if (PlayerPrefs.HasKey("LoadedSave"))
        {
            SaveManager.Load(PlayerPrefs.GetString("LoadedSave"));
            PlayerPrefs.DeleteAll();
        }
        else
        {
            WorldInit();
        }
    }

    public void Update()
    {
        if (_activeCells.Count == 0)
        {
            StopCoroutine(Tick());
            NewLife();
            StartCoroutine(Tick());
        }
    }

    public void WorldInit()
    {
        cells = new Cell[world.size, world.size];
        _worldPlace.localScale = new Vector3(world.size, world.size, 0);
        _worldPlace.position = new Vector3(world.size/2, world.size/2, 0);
        StartPool();
    }
    
    public IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f/2);
            
            List<int> cellToDead = new List<int>();

            ticks++;
            
            carnivores = 0;
            herbivorous = 0;
            omnivorous = 0;

            UIManager.TickUpdate(ticks, born);
            
            for (int i = 0; i < _activeCells.Count; i++)
            {
                if (_activeCells[i] != null)
                {
                    if (!_activeCells[i].isDead)
                    {
                        // Расчёт количества
                        switch (_activeCells[i].Type)
                        {
                            case World.PowerType.Carnivores:
                                carnivores++;
                                break;
                            case World.PowerType.Herbivorous:
                                herbivorous++;
                                break;
                            case World.PowerType.Omnivorous:
                                omnivorous++;
                                break;
                        }
                        
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
                
                // if (i % 5000 == 0)
                // {
                //     yield return null;
                // }
            }
            
            for (int i = 0; i < cellToDead.Count; i++)
            {
                _activeCells.RemoveAt(cellToDead[i] - i);
            }

            UIManager.ChangeCount(carnivores, herbivorous, omnivorous);
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
    
    public void DeletePool()
    {
        if (_poolCell != null)
        {
            for (int i = 0; i < _poolCell.Count; i++)
            {
                if (_poolCell[i].visual != null)
                {
                    Destroy(_poolCell[i].visual.gameObject);
                }
            }
        }
    }
    
    private void NewLife()
    {
        born++;
        
        if (!world.isBirthAfterDeath)
        {
            ticks = 0;
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
            cells[position.x, position.y].isInWorld = false;
            BackToPool(cells[position.x, position.y]);
            cells[position.x, position.y] = null;
        }
    }

    public void ClearWorld()
    {
        if (cells != null)
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
