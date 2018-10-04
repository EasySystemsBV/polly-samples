using System.Collections.Generic;
using System.Threading.Tasks;

namespace PollyDemo
{
    public interface IGoogleAutocompleteApi
    {
        Task<AutocompleteResult> GetPredictionsAsync( string input );
    }
}