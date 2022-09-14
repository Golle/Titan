namespace Titan.Physics;

// TODO:optimize this if possible, naive solution and might be slower than just doing the bounds check.
public record CollisionMatrixConfiguration(Dictionary<uint, uint> colliders);

public class CollisionMatrixBuilder<T> where T : struct, Enum
{
    private readonly Dictionary<T, T> _collisions = new();

    public CollisionMatrixBuilder()
    {
        // TODO: verify that the values only have a single bit set
        foreach (var value in Enum.GetValues<T>())
        {
            _collisions[value] = default;
        }
    }
    public CollisionMatrixBuilder<T> With(T layer, T collidesWith)
    {
        _collisions[layer] = collidesWith;
        return this;
    }

    public CollisionMatrixConfiguration Build()
    {
        var matrix = _collisions
            .Select(pair => (Convert.ToUInt32(pair.Key), Convert.ToUInt32(pair.Value)))
            .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

        return new(matrix);
    }
}
