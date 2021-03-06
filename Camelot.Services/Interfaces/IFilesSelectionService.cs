using System.Collections.Generic;

namespace Camelot.Services.Interfaces
{
    public interface IFilesSelectionService
    {
        IReadOnlyCollection<string> SelectedFiles { get; }

        void SelectFiles(IEnumerable<string> files);

        void UnselectFiles(IEnumerable<string> files);
    }
}