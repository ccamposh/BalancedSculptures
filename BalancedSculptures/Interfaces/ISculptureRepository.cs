using ccamposh.BalancedSculptures.Dto;

namespace ccamposh.BalancedSculptures.Interfaces
{
    public interface ISculptureRepository
    {
        bool TryInsert(byte[] key);
        long Count {get; }
        void SaveToFile(string filename);
    }
}