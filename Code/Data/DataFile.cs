namespace Observation;

public interface IDataFile<out T> where T : new()
{
	static abstract T? Data { get; }
}
