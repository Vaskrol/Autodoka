using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleFieldController : MonoBehaviour {

	[SerializeField] private SpriteRenderer _field;
	[SerializeField] private Camera _camera;
	
	[Header("Units")]
	[SerializeField] private UnitView _unitViewPrefab;
	[SerializeField] private Transform _unitsHolder;
	[SerializeField] private Color[] _unitColors;

	[Header("Debug")] 
	[SerializeField] private bool _debugMode;
	
	public List<int> FractionCounts = new List<int>();
	public bool IsSimulating { get { return _isSimulating; }}

	public Color[] FractionColors {
		get {
			if (_config.numUnitsToSpawn >= _unitColors.Length)
				return _unitColors;
			
			return _unitColors.Take(_config.numUnitsToSpawn).ToArray();
		}
	}

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
		
		foreach (var unit in _units) 
			unit.Update();
		
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
		var unitSize = Random.Range(_config.minUnitRadius, _config.maxUnitRadius);
		var unitSpeed = Random.Range(_config.minUnitSpeed, _config.maxUnitSpeed);
		var unitVelocity = Random.insideUnitCircle.normalized * unitSpeed;
		var unitPosition = new Vector2(
			(Random.value - 0.5f) * (_field.size.x - unitSize), 
			(Random.value - 0.5f) * (_field.size.y - unitSize)); 
		
		var unit = new Unit(unitPosition, unitSize, unitVelocity, fraction);
		
		CreateUnitView(unit);

		unit.OnDie += OnUnitDie;
		
		if (FractionCounts.Count <= fraction)
			FractionCounts.Add(0);
		FractionCounts[fraction]++;
		_physics.AddUnit(unit);
		
		return unit;
	}

	private void CreateUnitView(Unit unit) {
		var unitView = Instantiate(_unitViewPrefab, _unitsHolder);
		unitView.Init(unit, _unitColors[unit.Fraction]);
	}

	private void OnUnitDie(Unit unit) {
		FractionCounts[unit.Fraction]--;
		unit.OnDie -= OnUnitDie;
	}
	
	public void Save() {
		SaveManager.SaveSimulation(_units);
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
			var unit = new Unit(
				unitToken.Position,
				unitToken.Size,
				unitToken.Velocity,
				unitToken.Fraction);

			CreateUnitView(unit);
			
			unit.OnDie += OnUnitDie;
			
			if (FractionCounts.Count <= unitToken.Fraction)
				FractionCounts.Add(0);
			FractionCounts[unitToken.Fraction]++;

			_physics.AddUnit(unit);
			_units[i] = unit;
		}
	}

	public void SetSimulationStep(float stepDuration) {
		Time.fixedDeltaTime = stepDuration;
	}	
	
	public void SetSimulationSpeed(float timeScale) {
		Time.timeScale = timeScale;
	}

	private void ResetField() {
		_isSimulating = false;
		
		foreach (var unit in _units) {
			if (unit == null)
				continue;
			
			unit.Die();
		}
		
		FractionCounts.Clear();
		_physics.Reset();
	}

	private void OnDrawGizmos() {
		if (!_debugMode)
			return;
		
		_physics.DebugDrawChunks();

		foreach (var unit in _units) {
			if (unit == null) 
				continue;
			
			if (!unit.IsSimulated) {
				Gizmos.DrawCube(unit.Position, Vector3.one);
				continue;
			}

			Gizmos.DrawSphere(unit.Position, unit.Size / 2f);
		}
	}

}

