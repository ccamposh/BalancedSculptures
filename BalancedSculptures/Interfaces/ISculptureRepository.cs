using ccamposh.BalancedSculptures.Dto;

namespace ccamposh.BalancedSculptures.Interfaces
{
    public interface ISculptureRepository
    {
        bool TryInsert(Sculpture sculpture);
        long Count {get; }
        void SaveToFile(string filename);
    }
}