namespace service_api_csharp.Domain.ValueObjects;

public sealed class Location : IEquatable<Location>
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Location(double latitude, double longitude)
    {
        // Validaciones de invariantes (recomendado)
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitud inválida");

        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitud inválida");

        Latitude = latitude;
        Longitude = longitude;
    }

    public bool Equals(Location? other)
    {
        if (other is null) return false;
        return Latitude == other.Latitude &&
               Longitude == other.Longitude;
    }

    public override int GetHashCode() => 
        HashCode.Combine(Latitude, Longitude);
}
