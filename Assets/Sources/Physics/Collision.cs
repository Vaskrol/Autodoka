public class Collision {
    
    public float Distance { get; private set; }
    public Unit Collider { get; private set; }
    
    public Collision(float distance, Unit collider) {
        Distance = distance;
        Collider = collider;
    }
}
