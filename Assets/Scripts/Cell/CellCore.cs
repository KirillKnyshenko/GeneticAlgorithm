using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCore
{
    public bool isDead;
    public byte[] gens;
    private float _energy;
    protected byte _sumOfGens;
    public byte SumOfGens => _sumOfGens;
    public float GetEnergy(){ return _energy; }
    public void SetEnergy(float energy)
    {
        _energy = Mathf.Clamp(energy, 0, Manager.Instance.world.maxEnergy);
        IsDeath();
    }
    public void EnergyConsumption(float consumption)
    {
        SetEnergy(GetEnergy() - consumption);
    }
    public void EnergyAdd(float add)
    {
        SetEnergy(GetEnergy() + add);
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
    
    protected Vector2Int position;
    public Vector2Int Position => position;

    protected Transform visual;
    protected SpriteRenderer spriteRenderer;

    protected World.PowerType type;
    public World.PowerType Type => type;
    
    public void Moving(Vector2Int newPosition, Cell cell)
    {
        if (Type == World.PowerType.Carnivores)
        {
            if (GetEnergy() < Manager.Instance.world.actionEnergy / 2) return;
        
            EnergyConsumption(Manager.Instance.world.actionEnergy / 2);
        }
        else 
        {
            if (GetEnergy() < Manager.Instance.world.actionEnergy) return;

            EnergyConsumption(Manager.Instance.world.actionEnergy);
        }

        newPosition = Manager.Instance.CheckOfFrame(newPosition);
        
        if (Manager.Instance.CheckCell(newPosition) == null)
        {
            Manager.Instance.cells[newPosition.x, newPosition.y] = cell;
            Manager.Instance.cells[position.x, position.y] = null;
            position = newPosition;
            visual.position = (Vector2)position;
        }
    }

    public void Distributing(Cell cell, float energy)
    {
        cell.EnergyConsumption(energy);
    }
    
    public bool IsSiblings(Cell cell)
    {
        if (Mathf.Abs(_sumOfGens - cell.SumOfGens) < Manager.Instance.world.relatedness && Type == cell.Type)
        {
            return true;
        }

        return false;
    }
    
    protected bool Killing(Cell cellVictim)
    {
        if (!IsSiblings(cellVictim) || GetEnergy() < (Manager.Instance.world.maxEnergy * 0.15f))
        {
            if (cellVictim.type != World.PowerType.Herbivorous)
            {
                if (GetEnergy() < cellVictim.GetEnergy())
                {
                    Death();
                    
                    return false;
                }
            }
            
            if (Manager.Instance.KillActiveCell(cellVictim))
            {
                Manager.Instance.RemoveCell(cellVictim.Position);
                return true;
            }
        }

        return false;
    }
    
    public void RemoveVisual()
    {
        if (visual != null)
        {
            visual.gameObject.SetActive(false);
        }
    }
    
    private void IsDeath()
    {
        if (GetEnergy() <= 0 || Manager.Instance.world.yearsOldMax <= YearsOld)
        {
            Death();
        }
    }
    
    private void Death()
    {
        isDead = true;

        visual.GetComponent<SpriteRenderer>().color = Color.black;
    }
    
    #region MyMono
    protected void print(object obj)
    {
        Debug.Log(obj);
    }
    #endregion
}
