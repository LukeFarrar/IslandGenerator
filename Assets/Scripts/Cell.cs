   public enum Terrain
    {
        Water,
        Shallow,
        Land,
        Mountain,
        Ice,
        Sand,
        Lava
    }

public class Cell
{    public Terrain terrain;

    public Cell(Terrain terrain) {
        this.terrain = terrain;
    }
}