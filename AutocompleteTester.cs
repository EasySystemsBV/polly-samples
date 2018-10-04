using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;

namespace PollyDemo
{
    public class AutocompleteTester
    {
        private readonly IAsyncPolicy<AutocompleteResult> policy;
        private readonly IGoogleAutocompleteApi googleAutocomplete;

        public AutocompleteTester(PolicyFactory policyFactory,
            IGoogleAutocompleteApi googleAutocomplete)
        {
            this.policy = policyFactory.ApiCallPolicyAutocomplete;
            this.googleAutocomplete = googleAutocomplete;
        }

        public async Task<IEnumerable<string>> GetAutocompletePredictionsOldSchoolAsync(string input)
        {
            var apiCallResults = await googleAutocomplete.GetPredictionsAsync(input);
            return apiCallResults.Predictions;
        }

        public async Task<IEnumerable<string>> GetAutocompletePredictionsAsync(string input)
        {
            var apiCallResults = await policy.ExecuteAsync(
                async (ctx) => await googleAutocomplete.GetPredictionsAsync(input), 
                new Context(input));
            return apiCallResults.Predictions;
        }
    }
}
