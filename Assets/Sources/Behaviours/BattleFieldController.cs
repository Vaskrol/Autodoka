using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleFieldController : MonoBehaviour {

	[SerializeField] private SpriteRenderer _field;
	[SerializeField] private Camera _camera;
	
	[Header("Units")]
	[SerializeField] private Unit _unitPrefab;
	[SerializeField] private Transform _unitsHolder;
	[SerializeField] private Color[] _unitColors;

	[Header("Debug")] 
	[SerializeField] private bool _debugMode;
	
	public List<int> FractionCounts = new List<int>(); 
	public Color[] FractionColors { get { return _unitColors; } }
	public bool IsSimulating { get { return _isSimulating; }}

	private GameConfig _config;
	private Unit[] _units;
	private SimulationPhysics _physics;
	private bool _isSimulating;

	public void Init(GameConfig config, SimulationPhysics physics) {
		_config = config;
		_field.size = new Vector2(config.gameAreaWidth, config.gameAreaHeight);
		_camera.orthographicSize = _field.size.y / 2f;
		_physics = physics;
	}

	public void SpawnRandomUnits(Action onComplete) {
		_units = new Unit[_config.numUnitsToSpawn];
		StartCoroutine(ProcessSpawnRandomUnits(onComplete));
	}
	
	public void StartSimulation() {
		foreach(var unit in _units)
			unit.StartSimulation();
		
		_isSimulating = true;
	}

	private void FixedUpdate() {
		if (!_isSimulating)
			return;
		
		_physics.Update();
	}

	private IEnumerator ProcessSpawnRandomUnits(Action onComplete) {
		var colorIdx = 0; 
		for (int i = 0; i < _config.numUnitsToSpawn; i++) {
			_units[i] = SpawnRandomUnit(colorIdx);
			if (++colorIdx >= _unitColors.Length)
				colorIdx = 0;
			yield return new WaitForSeconds(_config.unitSpawnDelay / 1000f);
		}

		onComplete();
	}

	private Unit SpawnRandomUnit(int fraction) {
		var unit = Instantiate(_unitPrefab, _unitsHolder);
		var unitSize = Random.Range(_config.minUnitRadius, _config.maxUnitRadius);
		var unitSpeed = Random.Range(_config.minUnitSpeed, _config.maxUnitSpeed);
		var unitVelocity = Random.insideUnitCircle.normalized * unitSpeed;
		
		unit.Init(unitSize, unitVelocity, fraction, _unitColors[fraction]);
		unit.transform.position = new Vector2(
			(Random.value - 0.5f) * (_field.size.x - unit.Size), 
			(Random.value - 0.5f) * (_field.size.y - unit.Size));

		unit.OnDie += OnUnitDie;
		
		if (FractionCounts.Count <= fraction)
			FractionCounts.Add(0);
		FractionCounts[fraction]++;
		_physics.AddUnit(unit);
		
		return unit;
	}

	private void OnUnitDie(Unit unit) {
		FractionCounts[unit.Fraction]--;
		unit.OnDie -= OnUnitDie;
	}
	
	public void Save() {
		SaveManager.SaveSimulation(_units, _field.size);
	}

	public void Load() {
		var saveToken = SaveManager.LoadSave();
		if (saveToken == null)
			return;

		ResetField();
		SpawnLoadedUnits(saveToken);
		StartSimulation();
	}

	public void StartNewSimulation() {
		ResetField();
		SpawnRandomUnits(StartSimulation);
	}
	
	private void SpawnLoadedUnits(SaveToken save) {
		_units = new Unit[save.Units.Count];
		for (var i = 0; i < save.Units.Count; i++) {
			var unitToken = save.Units[i];
			var unit = Instantiate(_unitPrefab, _unitsHolder);

			unit.Init(
				unitToken.Size,
				unitToken.Velocity,
				unitToken.Fraction,
				_unitColors[unitToken.Fraction]);
			unit.transform.position = unitToken.Position;

			unit.OnDie += OnUnitDie;
			if (FractionCounts.Count <= unitToken.Fraction)
				FractionCounts.Add(0);
			FractionCounts[unitToken.Fraction]++;

			_physics.AddUnit(unit);
			_units[i] = unit;
		}
	}

	private void ResetField() {
		_isSimulating = false;
		
		FractionCounts.Clear();
		_physics.Reset();
		foreach (var unit in _units) {
			if (unit == null)
				continue;
			
			Destroy(unit.gameObject);
		}
	}

	private void OnDrawGizmos() {
		if (!_debugMode)
			return;
		
		_physics.DebugDrawChunks();
	}

}

