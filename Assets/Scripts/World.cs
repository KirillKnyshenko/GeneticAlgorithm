using UnityEngine;

[System.Serializable]
public class World
{
    public int size;
    public int countOfBorn;
    public int maxGens;
    public int maxEnergy;
    public int actionEnergy;
    public int energyBoost;
    public int yearsOldMax;
    public enum PowerType {Carnivores, Herbivorous, Omnivorous};
    [Range(0, 1)] public float needForDivision;
    [Range(0, 1)] public float babyCost;
    public bool isBirthAfterDeath;
}
