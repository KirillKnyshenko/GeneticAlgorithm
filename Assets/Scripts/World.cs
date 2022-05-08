using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Game/World Object", order = 1)]
public class World : ScriptableObject
{
    public int size;
    public int countOfBorn;
    public int maxGens;
    public float maxEnergy;
    public float actionEnergy;
    public float energyBoost;
    public float relatedness;
    public int yearsOldMax;
    public enum PowerType {Carnivores, Herbivorous, Omnivorous};
    [Range(0, 1)]public float chanceMutate;
    [Range(0, 1)] public float babyCost;
    public bool isBirthAfterDeath;
}