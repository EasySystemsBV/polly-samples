# polly-samples
A sample project on how to use [Polly](https://github.com/App-vNext/Polly).

The 'PolicyFactory' contains the policy definition. It uses the following policies:
- [Timeout](https://github.com/App-vNext/Polly#timeout)
- [Retry](https://github.com/App-vNext/Polly#retry)
- [Cache](https://github.com/App-vNext/Polly#cache)
- [Fallback](https://github.com/App-vNext/Polly#fallback)

Class 'AutocompleteTester' calls 'IGoogleAutocompleteApi' to get fictional autocomplete predictions. It calls the 'IGoogleAutocompleteApi' mock through a policy that is defined in the 'PolicyFactory'.

Class 'AutocompleteTesterTests' contains some basic tests that demo the behavior.