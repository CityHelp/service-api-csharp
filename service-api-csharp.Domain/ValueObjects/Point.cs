using System.Collections.Generic;

namespace service_api_csharp.Domain.ValueObjects;

/// <summary>
/// Represents a geographic point with X (Longitude) and Y (Latitude) coordinates.
/// This Value Object is designed to be mapped to NetTopologySuite's Point in the Infrastructure layer.
/// </summary>
public class Point : ValueObject
{
    public double X { get; }
    public double Y { get; }
    public int Srid { get; }

    private Point(double x, double y, int srid)
    {
        X = x;
        Y = y;
        Srid = srid;
    }

    public static Point Create(double x, double y, int srid = 4326)
    {
        // Add validation if necessary (e.g., valid lat/long ranges)
        return new Point(x, y, srid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Srid;
    }
}
