using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Cell
{
    public bool IsDead;
    public byte[] Gens { get; private set; }
    private byte _lastGen = 0;
    private byte _sumOfGens;
    
    private World.PowerType _type;
    public World.PowerType Type => _type;
    
    private float _energy;
    public float Energy
    {
        get => _energy;
        set
        {
            _energy = Mathf.Clamp(value, 0, Manager.Instance.world.maxEnergy);
            IsDeath();
        }
    }

    private int _yearsOld;
    public int YearsOld 
    {
        get => _yearsOld;
        set
        {
            _yearsOld = value;
            IsDeath();
        }
    }

    private Vector2Int _position;

    public Vector2Int Position => _position;

    private Vector2Int _rotation;
    public Vector2Int Rotation => _rotation;
    
    private Transform _visual;
    private SpriteRenderer _visualSpriteRenderer;

    public Cell(Transform visual)
    {
        _visual = visual;
        _visualSpriteRenderer = visual.GetComponent<SpriteRenderer>();
    }    
    
    public void Initialization(int maxGens, Vector2Int position, byte[] gens, float energy)
    {
        //Энергия
        Energy = energy;
        
        //Клетка жива
        IsDead = false;
        
        //Возраст клетки 0
        YearsOld = 0;

        //Начинать действия с 0 гена
        _lastGen = 0;
        
        //Гены
        if (gens == null)
        {
            Gens = new byte[maxGens];
            
            for (int i = 0; i < maxGens; i++)
            {
                Gens[i] = (byte) Random.Range(0, 64);
            }
        }
        else
        {
            Gens = gens;
        }

        //Сумма генов
        byte sum = 0;
        for (int i = 0; i < Gens.Length; i++)
        {
            _sumOfGens += Gens[i];
        }

        //Позиция
        _position = position;

        //Поворот
        _rotation = Rotate();

        //Отображение
        _visual.position = (Vector2)_position;
        _visual.gameObject.SetActive(true);

        //Тип питания
        if (Gens[0] < 10)
        {
            _type = World.PowerType.Carnivores;
            _visualSpriteRenderer.color = Color.red;
        }
        else if (Gens[0] < 60)
        {
            _type = World.PowerType.Herbivorous;
            _visualSpriteRenderer.color = Color.green;
        }
        else
        {
            _type = World.PowerType.Omnivorous;
            _visualSpriteRenderer.color = Color.blue;
        }
    }
    
    public void Action()
    {
        if (Gens.Length <= _lastGen)
        {
            _lastGen = 0;
        }

        ActionVariant();
        
        _lastGen++;
        _yearsOld++;
    }

    private void ActionVariant()
    {
        if (Gens[_lastGen] <= 5)
        {
            Move();
        }
        else if (Gens[_lastGen] <= 10)
        {
            _rotation = Rotate();
        }
        else if (Gens[_lastGen] <= 20)
        {
            Division();
        }
        else if (Gens[_lastGen] <= 64)
        {
            Eating();
        }
        Energy = EnergyConsumption(Manager.Instance.world.actionEnergy);
    }

    private void Move()
    {
        var position = _position + _rotation;

        position = Manager.Instance.CheckOfFrame(position);
        
        if (Manager.Instance.CheckCell(position) == null)
        {
            Manager.Instance.cells[position.x, position.y] = this;
            Manager.Instance.cells[_position.x, _position.y] = null;
            _position = position;
            _visual.position = (Vector2)position;
        }
        else if (_type != World.PowerType.Herbivorous)
        {
            Eating();
        }
        else
        {
            Division();
        }
    }

    private Vector2Int Rotate()
    {
        Vector2Int rotation;
        do
        {
            var x = Random.Range(-1,2);
            var y = Random.Range(-1,2);
            rotation = new Vector2Int(x, y);
        } while (rotation == new Vector2Int(0, 0));

        return rotation;
    }

    private void Division()
    {
        if (Energy > Manager.Instance.world.needForDivision)
        {
            var position = _position + _rotation;
        
            position = Manager.Instance.CheckOfFrame(position);
        
            if (Manager.Instance.cells[position.x, position.y] == null || Manager.Instance.cells[position.x, position.y]?.IsDead == true)
            {
                var babyCost = Manager.Instance.world.maxEnergy * Manager.Instance.world.babyCost;
                Manager.Instance.RemoveCell(position);
                Manager.Instance.CreateCell(position, babyCost, Gens);
                
                Energy -= babyCost;
            }
        }
    }

    private void Eating()
    {
        if (_type == World.PowerType.Herbivorous)
        {
            Energy += Manager.Instance.world.energyBoost;
        }
        else
        {
            bool isAte = FoundMeatAround();

            if (!isAte && (_type == World.PowerType.Omnivorous))
            {
                Energy += Manager.Instance.world.energyBoost * 0.4f;
            }
        }
    }

    private bool FoundMeatAround()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((i != 0) && (j != 0))
                {
                    var position = Manager.Instance.CheckOfFrame(_position + new Vector2Int(i, j));
                    Cell cellVictim = Manager.Instance.CheckCell(position);
                    
                    if (cellVictim != null)
                    {
                        if (cellVictim.IsDead)
                        {
                            EnergyConsumption(cellVictim.Energy);
                            Manager.Instance.RemoveCell(cellVictim.Position);
                            return true;
                        }
                        else if(Gens[1] > 32)
                        {
                            if (Energy > cellVictim.Energy)
                            {
                                EnergyConsumption(cellVictim.Energy * 0.6f);
                                cellVictim.IsDead = true;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private float EnergyConsumption(float consumption)
    {
        return Energy - consumption;
    }

    private bool IsDeath()
    {
        if (Energy <= 0 || Manager.Instance.world.yearsOldMax <= YearsOld)
        {
            Death();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Death()
    {
        IsDead = true;

        _visual.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 1, 0.5f);
    }

    public void RemoveVisual()
    {
        if (_visual != null)
        {
            _visual.gameObject.SetActive(false);
        }
    }
    
    #region Color

    public void CreateColor()
    {
        SpriteRenderer spriteRenderer = _visual.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
        
        byte r, g, b;
        
        if ((2 % _sumOfGens) == 0)
        {
            r = (byte)(_sumOfGens * 0.5f);
        }
        else
        {
            r = (byte)(_sumOfGens * 0.1f);
        }
        
        if ((2 % _sumOfGens) == 0)
        {
            g = (byte)(_sumOfGens * 0.5f);
        }
        else
        {
            g = (byte)(_sumOfGens * 0.1f);
        }
        
        if ((2 % _sumOfGens) == 0)
        {
            b = (byte)(_sumOfGens * 0.5f);
        }
        else
        {
            b = (byte)(_sumOfGens * 0.1f);
        }

        spriteRenderer.color = new Color32(r, g, b, 255);
        print((_sumOfGens * 0.5f));
    }

    #endregion

    #region MyMono
    private void print(object obj)
    {
        Debug.Log(obj);
    }
    #endregion
}
