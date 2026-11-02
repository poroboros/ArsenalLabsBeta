

using Microsoft.Xna.Framework;

namespace ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;

public class TestChamberSpawnData
{
    public string name;
    public Point offset;
    public bool hasChest;
    public byte difficulty;
    public float rarity;

    public TestChamberSpawnData(string name, Point? offset = null)
    {
        this.name = name;

        this.offset = offset ?? Point.Zero;
    }
}
