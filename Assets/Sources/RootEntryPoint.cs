using System.IO;
using UnityEngine;

public class RootEntryPoint : MonoBehaviour {

    [SerializeField] private BattleFieldController _battleField;

    private SimulationPhysics _physics;
    
    private void Start() {
        var config = LoadConfig();
        
        _physics = new SimulationPhysics(config);
        _battleField.Init(config);
        _battleField.SpawnUnits(OnUnitsSpawned);
    }

    private void OnUnitsSpawned() {
        Debug.Log("All units have been spawned.");
        
        _battleField.StartSimulation();
    }

    private GameConfig LoadConfig() {
        var configPath = Application.dataPath + "/data.txt";
        Debug.Log("Loading config from " + configPath);
        
        string json = File.ReadAllText(configPath); 
        var config = JsonUtility.FromJson<Config>(json);
        return config.GameConfig;
    }
}
