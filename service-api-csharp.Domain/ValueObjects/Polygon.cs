using System;
using System.Collections.Generic;
using System.Linq;

namespace service_api_csharp.Domain.ValueObjects;

/// <summary>
/// Represents a geographic polygon defined by a list of points (exterior ring).
/// This Value Object is designed to be mapped to NetTopologySuite's Polygon in the Infrastructure layer.
/// </summary>
public class Polygon : ValueObject
{
    public IReadOnlyList<Point> Coordinates { get; }
    public int Srid { get; }

    private Polygon(List<Point> coordinates, int srid)
    {
        Coordinates = coordinates.AsReadOnly();
        Srid = srid;
    }

    public static Polygon Create(IEnumerable<Point> points, int srid = 4326)
    {
        var pointList = points?.ToList() ?? new List<Point>();

        if (pointList.Count < 3)
        {
            throw new ArgumentException("A polygon must have at least 3 points.");
        }

        // Ensure the polygon is closed (first point equals last point)
        if (!pointList.First().Equals(pointList.Last()))
        {
            pointList.Add(pointList.First());
        }

        return new Polygon(pointList, srid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Srid;
        foreach (var point in Coordinates)
        {
            yield return point;
        }
    }
}
