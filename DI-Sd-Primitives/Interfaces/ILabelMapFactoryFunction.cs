namespace DI_Sd_Primitives.Interfaces
{
    public interface ILabelMapFactoryFunction
    {
        IDictionary<string, string> CreateLabelMap(IDictionary<string, string> canonicalIdMap);
    }
}
