namespace service_api_csharp.Domain.ValueObjects;

public sealed class LocationCityHelp : IEquatable<LocationCityHelp>
{
    public double Latitude { get; }
    public double Longitude { get; }

    public LocationCityHelp(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitud inválida");

        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitud inválida");

        Latitude = latitude;
        Longitude = longitude;
    }

    private LocationCityHelp() { }

    public bool Equals(LocationCityHelp? other)
        => other is not null &&
           Latitude == other.Latitude &&
           Longitude == other.Longitude;

    public override int GetHashCode()
        => HashCode.Combine(Latitude, Longitude);
}

