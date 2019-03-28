using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreBar : MonoBehaviour {

	[SerializeField] private RectTransform _barBack;
	[SerializeField] private LayoutElement _coloredBar;
	[SerializeField] private BattleFieldController _battleField;

	private List<LayoutElement> _bars = new List<LayoutElement>();
	
	private void Start () {
		foreach (var fractionColor in _battleField.FractionColors) {
			var bar = Instantiate(_coloredBar, _barBack);
			bar.GetComponent<Image>().color = fractionColor.WithAlpha(0.5f);
			_bars.Add(bar);
		}
	}
	
	private void Update () {
		if (!_battleField.IsSimulating)
			return;
		
		var totalBarWidth = Screen.width;
		var totalUnitsCount = _battleField.FractionCounts.Sum(f => f.Value);
		for (int i = 0; i < _bars.Count; i++) {
			var bar = _bars[i];
			bar.preferredWidth = totalBarWidth / totalUnitsCount * _battleField.FractionCounts[i];
		}
	}
}
