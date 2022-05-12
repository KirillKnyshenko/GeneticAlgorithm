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
    
    public SaveManager.WorldData CreateWorldData()
    {
        SaveManager.WorldData world = new SaveManager.WorldData();

        world.size = size;
        world.countOfBorn = countOfBorn;
        world.maxGens = maxGens;
        world.maxEnergy = maxEnergy;
        world.actionEnergy = actionEnergy;
        world.energyBoost = energyBoost;
        world.relatedness = relatedness;
        world.yearsOldMax = yearsOldMax;
        world.chanceMutate = chanceMutate;
        world.babyCost = babyCost;
        world.isBirthAfterDeath = isBirthAfterDeath;

        return world;
    }
    
    public void SetWorldData(SaveManager.WorldData world)
    {
        size = world.size;
        countOfBorn = world.countOfBorn;
        maxGens = world.maxGens;
        maxEnergy = world.maxEnergy;
        actionEnergy = world.actionEnergy;
        energyBoost = world.energyBoost;
        relatedness = world.relatedness;
        yearsOldMax = world.yearsOldMax;
        chanceMutate = world.chanceMutate;
        babyCost = world.babyCost;
        isBirthAfterDeath = world.isBirthAfterDeath;
    }
}