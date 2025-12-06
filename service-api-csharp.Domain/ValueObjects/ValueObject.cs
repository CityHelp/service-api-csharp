namespace service_api_csharp.Domain.ValueObjects;

// Capa: [TuProyecto].Domain
public abstract class ValueObject
{
    // Método abstracto para que las clases derivadas especifiquen qué campos se deben comparar
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        
        // Compara todos los componentes de ambas instancias
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        // Genera un hash code basado en todos los componentes
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    // Sobrecarga de operadores (opcional, pero buena práctica)
    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }
}