using System.Collections.Generic;

public class Chunk {
    
    public HashSet<Unit> Units { get; private set; }

    public Chunk() {
        Units = new HashSet<Unit>();
    }

    public void AddUnit(Unit unit) {
        Units.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        Units.Remove(unit);
    }
}
