using System;
using ccamposh.BalancedSculptures.Dto;

namespace ccamposh.BalancedSculptures.Interfaces
{
    public interface ISculptureRepository
    {
        bool TryInsert(Guid key);
        long Count {get; }
        void SaveToFile(string filename);
    }
}