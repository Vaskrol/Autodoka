using System.Collections.Generic;
using UnityEngine;

public static class SaveManager {

    private const string SAVE_KEY = "Autodoka_field";
    
    public static void SaveSimulation(Unit[] units) {
        var saveToken = new SaveToken {
            Units = new List<UnitToken>(units.Length)
        };
        
        foreach (var unit in units) {
            if (unit == null)
                continue;
            
            saveToken.Units.Add(new UnitToken {
                Fraction = unit.Fraction,
                Position = unit.Position,
                Size = unit.Size,
                Velocity = unit.Velocity
            });
        }

        var json = JsonUtility.ToJson(saveToken);
        
        Debug.Log("Simualtion saved");
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    public static SaveToken LoadSave() {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) {
            Debug.LogError("Cannot load save: save doesn't exist");
            return null;
        }

        var json = PlayerPrefs.GetString(SAVE_KEY);
        var saveToken = JsonUtility.FromJson<SaveToken>(json);
        
        Debug.Log("Simualtion loaded");
        return saveToken;
    }

}
