using System.Collections.Generic;

public class Chunk {
    
    public List<Unit> Units { get; private set; }

    public Chunk() {
        Units = new List<Unit>();
    }

    public void AddUnit(Unit unit) {
        Units.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        Units.Remove(unit);
    }

    public void Clear() {
        Units.Clear();
    }
}
