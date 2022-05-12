using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public class CellData
    {
        public bool isDead;
        public byte[] gens;
        public float energy;
        public int yearsOld;
        public Vector2Int position;
        public byte lastGen;
        public bool isAte;
        public Vector2Int rotation;
    }
    
    public class WorldData
    {
        public int size;
        public int countOfBorn;
        public int maxGens;
        public float maxEnergy;
        public float actionEnergy;
        public float energyBoost;
        public float relatedness;
        public int yearsOldMax;
        public float chanceMutate;
        public float babyCost;
        public bool isBirthAfterDeath;
    }
    
    public class ManagerData
    {
        public int born;
        public int ticks;
    }

    public class Data
    {
        public WorldData worldData;
        public ManagerData managerData = new ManagerData();
        public List<CellData> cells = new List<CellData>();
    }
    
    public static void Load()
    {
        Data data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(Application.dataPath + "/data.json"));

        if (data != null)
        {
            Manager.Instance.StopAllCoroutines();
            Manager.Instance.DeletePool();
            Manager.Instance.ClearWorld();
            Manager.Instance.world.SetWorldData(data.worldData);
            Manager.Instance.WorldInit();
            
            Manager.Instance.born = data.managerData.born;
            Manager.Instance.ticks = data.managerData.ticks;

            Camera.main.GetComponent<BehaviourCamera>().SetCamera();
            
            for (int i = 0; i < data.cells.Count; i++)
            {
                Cell cell = Manager.Instance.GetFromPool(data.cells[i].position);
                cell.SetCellData(data.cells[i]);
            }
            
            Manager.Instance.StartCoroutine(Manager.Instance.Tick());
        }
    }

    public static void Save()
    {
        Data data = new Data();
        var cells = Manager.Instance.cells;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j] != null)
                {
                    data.cells.Add(cells[i, j].CreateCellData());
                }
            }
        }

        data.worldData = Manager.Instance.world.CreateWorldData();
        data.managerData.born = Manager.Instance.born;
        data.managerData.ticks = Manager.Instance.ticks;

        File.WriteAllText(
            Application.dataPath + "/data.json",
            JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
        );
    }
}
