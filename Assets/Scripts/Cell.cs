using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Cell : CellCore
{
    private Color _genColor, _typeColor, _energyColor;
    
    private byte _lastGen = 0;

    private bool _isAte;

    private Vector2Int _rotation;
    public Vector2Int Rotation => _rotation;

    public Cell(Transform newVisual)
    {
        visual = newVisual;
        spriteRenderer = visual.GetComponent<SpriteRenderer>();
    }    
    
    public void Initialization(int maxGens, Vector2Int newPosition, byte[] newGens, float energy)
    {
        //Энергия
        SetEnergy(energy);
        
        //Клетка жива
        isDead = false;

        //Возраст клетки 0
        YearsOld = 0;

        //Начинать действия с 0 гена
        _lastGen = 0;
        
        //Гены
        if (newGens == null)
        {
            gens = new byte[maxGens];
            
            for (int i = 0; i < maxGens; i++)
            {
                gens[i] = (byte) Random.Range(0, 10);
            }
        }
        else
        {
            gens = new byte[maxGens];
            
            for (int i = 0; i < maxGens; i++)
            {
                gens[i] = newGens[i];
            }
        }

        //Сумма генов
        for (int i = 0; i < gens.Length; i++)
        {
            _sumOfGens += gens[i];
        }

        //Позиция
        position = newPosition;

        //Поворот
        _rotation = Rotate();

        //Отображение
        visual.position = (Vector2)position;
        visual.gameObject.SetActive(true);

        //Тип питания
        if (gens[0] == 0)
        {
            type = World.PowerType.Omnivorous;
            _typeColor = Color.blue;
        }
        else if (gens[0] == 1 || gens[0] == 2)
        {
            type = World.PowerType.Carnivores;
            _typeColor = Color.red;
        }
        else
        {
            type = World.PowerType.Herbivorous;
            _typeColor = Color.green;
        }
        
        CreateGenColor();
        DrawColor();
    }
    
    public void Action()
    {
        // Обнуление текущего гена, если он вышел за рамки
        if (gens.Length <= _lastGen)
        {
            _lastGen = 0;
        }

        // Проявление активности
        ActionVariant();
        
        // Переход к следующему гену
        _lastGen++;
        // Взросление клетки
        YearsOld++;
    }

    private void ActionVariant()
    {
        if (Manager.Instance.colorMod ==Manager.ViewColor.EnergyColor)
        {
            CreateEnergyColor();
            spriteRenderer.color = _energyColor;
        }
        
        Eating();
        
        // Плотоядные
        #region Carnivores

        if (type == World.PowerType.Carnivores)
        {
            Division();
            
            if ((gens[_lastGen] == 3 || gens[_lastGen] == 4) && !_isAte)
            {
                Move();
            }
            else if (gens[_lastGen] == 5 || gens[_lastGen] == 6)
            {
                _rotation = Rotate();
            }
            
        }

        #endregion
        
        // Фотосинтезирующие
        #region Herbivorous

        if (type == World.PowerType.Herbivorous)
        {
            if (gens[_lastGen] == 2)
            {
                Division();
            }
            else if (gens[_lastGen] == 9)
            {
                Move();
            }
            else if (gens[_lastGen] == 8)
            {
                _rotation = Rotate();
            }
            
        }

        #endregion
        
        // Всеядные
        #region Omnivorous
        
        if (type == World.PowerType.Omnivorous)
        {
            Division();
            
            if (gens[_lastGen] == 1)
            {
                Move();
            }
            else if (gens[_lastGen] == 2 && gens[_lastGen] == 5)
            {
                _rotation = Rotate();
            }
        }
        
        #endregion
        
        EnergyDistribution();
    }

    private void Move()
    {
        Vector2Int newPosition = Position + Rotation;

        Moving(newPosition, this);
    }

    private Vector2Int Rotate()
    {
        if (GetEnergy() < Manager.Instance.world.actionEnergy / 2) return Rotation;
        
        EnergyConsumption(Manager.Instance.world.actionEnergy / 2);
        
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
        float babyCost;
        if (type == World.PowerType.Carnivores)
        {
            babyCost = (Manager.Instance.world.babyCost / 10) * Manager.Instance.world.maxEnergy;
        }
        else
        {
            babyCost = Manager.Instance.world.babyCost * Manager.Instance.world.maxEnergy;
        }

        babyCost -= gens[2];
        
        if (GetEnergy() >= (babyCost + Manager.Instance.world.actionEnergy))
        {
            var newPosition = Position + Rotation;
        
            newPosition = Manager.Instance.CheckOfFrame(newPosition);
        
            if (Manager.Instance.cells[newPosition.x, newPosition.y] == null || Manager.Instance.cells[newPosition.x, newPosition.y]?.isDead == true || (type != World.PowerType.Herbivorous && Killing(Manager.Instance.cells[newPosition.x, newPosition.y])))
            {
                Manager.Instance.RemoveCell(newPosition);
                Manager.Instance.CreateCell(newPosition, babyCost, gens);
                
                EnergyConsumption(babyCost + Manager.Instance.world.actionEnergy);
            }
        }
    }

    public void EnergyDistribution()
    {
        if (GetEnergy() > (Manager.Instance.world.maxEnergy * 0.4f))
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i != 0) && (j != 0))
                    {
                        Vector2Int positionDistribution = Manager.Instance.CheckOfFrame(new Vector2Int(i, j));
                        Cell cellDistribution = Manager.Instance.CheckCell(positionDistribution);

                        if (cellDistribution != null)
                        {
                            if (cellDistribution.isDead)
                            {
                                if (IsSiblings(cellDistribution))
                                {
                                    if (cellDistribution.GetEnergy() < GetEnergy())
                                    {
                                        float energyForDistributing = (GetEnergy() - cellDistribution.GetEnergy()) * 0.5f;
                                        Distributing(cellDistribution, energyForDistributing);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void Eating()
    {
        if (type == World.PowerType.Herbivorous)
        {
            EnergyAdd(Manager.Instance.world.energyBoost + (gens[5] / 10f));
        }
        else
        {
            _isAte = FoundMeat();

            if (!_isAte && (type == World.PowerType.Omnivorous))
            {
                EnergyAdd(Manager.Instance.world.energyBoost * 0.3f);
            }
        }
    }

    private bool FoundMeat()
    {
        Vector2Int positionVictim = Manager.Instance.CheckOfFrame(Position + Rotation);
        Cell cellVictim = Manager.Instance.CheckCell(positionVictim);

        if (cellVictim != null)
        {
            //Съесть клетку если она мертва
            if (cellVictim.isDead)
            {
                if (cellVictim.GetEnergy() > 0)
                {
                    EnergyAdd(cellVictim.GetEnergy());
                    Manager.Instance.RemoveCell(cellVictim.Position);

                    Moving(positionVictim, this);
                    return true;
                }

                Rotate();
            }

            //Условие для атаки живой клетки
            if (gens[1] > 2)
            {
                if (Killing(cellVictim))
                {
                    EnergyAdd(cellVictim.GetEnergy());
                    Moving(positionVictim, this);
                    return true;
                }
            }
        }
        
        return false;
    }

    #region Color

    public void DrawColor()
    {
        if (!isDead)
        {
            if (Manager.Instance.colorMod == Manager.ViewColor.EnergyColor)
            {
                CreateEnergyColor();
                spriteRenderer.color = _energyColor;
            }
            else if (Manager.Instance.colorMod == Manager.ViewColor.TypeColor)
            {
                spriteRenderer.color = _typeColor;
            }
            else if (Manager.Instance.colorMod == Manager.ViewColor.GenColor)
            {
                spriteRenderer.color = _genColor;
            }
        }
        else
        {
            spriteRenderer.color = Color.black;
        }
    }
    
    public void CreateGenColor()
    {
        float r = 0, g = 0, b = 0;
        var thoughtCount = Manager.Instance.world.maxGens;
        for (int i = 0; i < thoughtCount; i++)
        {
            if (i < thoughtCount * 0.3f)
            {
                r += gens[i];
            }
            else if (i >= thoughtCount * 0.3f && i <= thoughtCount * 0.7f)
            {
                g += gens[i];
            }
            else
            {
                b += gens[i];
            }
        }
        float max = Mathf.Max(Mathf.Max(r, g), b);
        Color genColor = new Color32((byte)(((r / max) * (255))), (byte)((g / max) * (255)), (byte)((b / max) * (255)), 255);

        Color.RGBToHSV(genColor, out float h, out float s, out float v);
        _genColor = Color.HSVToRGB(h, s * 2f, v);
    }

    public void CreateEnergyColor()
    {
        _energyColor = Color.Lerp(Color.black, Color.green, GetEnergy() / Manager.Instance.world.maxEnergy);
    }
    
    #endregion
}
