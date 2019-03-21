using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class BattleFieldController : MonoBehaviour {

	[SerializeField] private SpriteRenderer _field;
	[SerializeField] private Camera _camera;
	
	[Header("Units")]
	[SerializeField] private Unit _unitPrefab;
	[SerializeField] private Transform _unitsHolder;
	[SerializeField] private Color[] _unitColors;
	
	private GameConfig _config;
	private Unit[,] _units;
	private Rect _fieldSize;
	private int _perFractionCount;

	public void Init(GameConfig config) {
		_config = config;
		_field.size = new Vector2(config.GameAreaWidth, config.GameAreaHeight);
		_fieldSize = new Rect(Vector2.zero, _field.size);
		_camera.orthographicSize = _field.size.y / 2f;
		_perFractionCount  = _config.NumUnitsToSpawn / _unitColors.Length;
	}

	public void SpawnUnits(Action onComplete) {
		
		_units = new Unit[_unitColors.Length,_config.NumUnitsToSpawn];
		StartCoroutine(ProcessSpawnUnits(onComplete));
	}

	public void StartSimulation() {
		for (int colorIdx = 0; colorIdx < _unitColors.Length; colorIdx++)
		for (int i = 0; i < _perFractionCount; i++) {
			_units[colorIdx, i].Fight();
		}
	}

	private IEnumerator ProcessSpawnUnits(Action onComplete) {
		for (int colorIdx = 0; colorIdx < _unitColors.Length; colorIdx++) 
		for (int i = 0; i < _perFractionCount; i++) {
			_units[colorIdx, i] = SpawnUnit(colorIdx);
			yield return new WaitForSeconds(_config.UnitSpawnDelay / 1000f);
		}

		onComplete();
	}

	private Unit SpawnUnit(int unitColorIndex) {
		var unit = Instantiate(_unitPrefab, _unitsHolder);
		var unitSize = Random.Range(_config.MinUnitRadius, _config.MaxUnitRadius);
		var unitSpeed = Random.Range(_config.MinUnitSpeed, _config.MaxUnitSpeed);
		unit.Init(unitSize, unitSpeed, _fieldSize, unitColorIndex, _unitColors[unitColorIndex]);
		
		return unit;
	}
	
	

}

