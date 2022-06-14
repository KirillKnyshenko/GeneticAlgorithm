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
        public byte[] texture;
    }
    
    public static readonly string savesFolder = Directory.GetParent(Application.dataPath) + "/Saves/";
    
    public static void Load(string fileName)
    {
        if (Directory.Exists(savesFolder))
        {
            Data data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(savesFolder + $"/{fileName}.json"));

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
    }

    public static bool Save(string fileName)
    {
        if (!Directory.Exists(savesFolder));
        {
            Directory.CreateDirectory(savesFolder);
        }
        
        if (File.Exists(savesFolder + @"\" + fileName + ".json"))
        {
            return false;
        }

        Data data = new Data();
        var cells = Manager.Instance.cells;

        var tex = new Texture2D(cells.GetLength(0), cells.GetLength(0));
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j] != null)
                {
                    var dt = cells[i, j].CreateCellData();
                    data.cells.Add(dt);
                    tex.SetPixel(i, j,cells[i, j].GenColor);
                }
                else
                {
                    tex.SetPixel(i, j, Color.gray);
                }
            }
        }

        tex.Apply();
        
        data.worldData = Manager.Instance.world.CreateWorldData();
        data.managerData.born = Manager.Instance.born;
        data.managerData.ticks = Manager.Instance.ticks;

        data.texture = tex.GetRawTextureData();
        
        
        File.WriteAllText(
            savesFolder + $"/{fileName}.json",
            JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
        );


        return true;
    }
}
