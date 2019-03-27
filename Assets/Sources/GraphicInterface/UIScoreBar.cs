using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreBar : MonoBehaviour {

	[SerializeField] private RectTransform _barBack;
	[SerializeField] private LayoutElement _coloredBar;
	[SerializeField] private BattleFieldController _battleField;

	private List<LayoutElement> _bars = new List<LayoutElement>();
	private float _totalBarWidth;

	private void Start () {
		_totalBarWidth = _barBack.rect.width;

		foreach (var fractionColor in _battleField.FractionColors) {
			var bar = Instantiate(_coloredBar, _barBack);
			bar.GetComponent<Image>().color = fractionColor;
			_bars.Add(bar);
		}
	}
	
	private void Update () {
		if (!_battleField.IsSimulating)
			return;
		
		var totalUnitsCount = _battleField.FractionCounts.Sum(f => f.Value);
		Debug.Log("Total units: " + totalUnitsCount);
		for (int i = 0; i < _bars.Count; i++) {
			var bar = _bars[i];
			bar.preferredHeight = _totalBarWidth / totalUnitsCount * _battleField.FractionCounts[i];
		}
	}
}
