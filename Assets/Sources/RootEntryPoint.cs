using System.IO;
using UnityEngine;

public class RootEntryPoint : MonoBehaviour {

    [SerializeField] private BattleFieldController _battleField;
    
    private void Start() {
        var config = LoadConfig();
        
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
        return JsonUtility.FromJson<GameConfig>(json);
    }
}
