﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIControlPanel : MonoBehaviour {

	[SerializeField] private Button _newSimulationButton;
	[SerializeField] private Button _saveButton;
	[SerializeField] private Button _loadButton;

	[SerializeField] private GameObject _gameOverPanel;	
	[SerializeField] private Button _gameOverRestartButton;
	[SerializeField] private Image _gameOverWinnerColor;	
	[SerializeField] private Text _gameOverText;	

	[SerializeField] private BattleFieldController _battleField;
	
	// Use this for initialization
	private void Start () {
		_newSimulationButton.onClick.AddListener(OnNewSimulationClicked);
		_saveButton.onClick.AddListener(OnSaveClicked);
		_loadButton.onClick.AddListener(OnLoadClicked);
		_gameOverRestartButton.onClick.AddListener(OnNewSimulationClicked);
		
		_gameOverPanel.SetActive(false);
	}

	private void OnLoadClicked() {
		_battleField.Load();
	}

	private void OnSaveClicked() {
		_battleField.Save();
	}

	private void OnNewSimulationClicked() {
		_battleField.StartNewSimulation();
		_gameOverPanel.SetActive(false);
	}

	private void Update() {
		if (!_battleField.IsSimulating)
			return;
		
		var unitCounts = _battleField.FractionCounts;
		var aliveColors = unitCounts.Where(c => c > 0).ToArray();
		if (aliveColors.Length > 1)
			return;

		_gameOverPanel.SetActive(true);
		
		if (aliveColors.Length == 1) {
			_gameOverWinnerColor.color = _battleField.FractionColors[unitCounts.IndexOf(aliveColors[0])];
			_gameOverText.text = "We have a winner!";
			return;
		}
		
		if (aliveColors.Length == 0) {
			_gameOverWinnerColor.color = Color.white;
			_gameOverText.text = "Draw!";
		}
	}
}
