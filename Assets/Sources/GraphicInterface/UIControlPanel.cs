
using UnityEngine;
using UnityEngine.UI;

public class UIControlPanel : MonoBehaviour {

	[SerializeField] private Button _newSimulationButton;
	[SerializeField] private Button _saveButton;
	[SerializeField] private Button _loadButton;

	[SerializeField] private BattleFieldController _battleField;
	
	// Use this for initialization
	private void Start () {
		_newSimulationButton.onClick.AddListener(OnNewSimulationClicked);
		_saveButton.onClick.AddListener(OnSaveClicked);
		_loadButton.onClick.AddListener(OnLoadClicked);
	}

	private void OnLoadClicked() {
		_battleField.Load();
	}

	private void OnSaveClicked() {
		_battleField.Save();
	}

	private void OnNewSimulationClicked() {
		_battleField.StartNewSimulation();
	}
}
