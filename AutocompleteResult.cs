using System.Collections.Generic;

namespace PollyDemo
{
    public class AutocompleteResult
    {
        public string Status { get; set; }

        public List<string> Predictions { get; set; }
    }
}
