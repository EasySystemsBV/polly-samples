using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PollyDemo
{
    [TestClass]
    public class AutocompleteTesterTests
    {
        IGoogleAutocompleteApi googleMock = Substitute.For<IGoogleAutocompleteApi>();
        private PolicyFactory policyFactory = new PolicyFactory();

        [TestInitialize]
        public void TestInitialize()
        {
            googleMock.GetPredictionsAsync("Foo").Returns(
                new AutocompleteResult
                {
                    Status = GoogleMapsConstants.STATUS_OK,
                    Predictions = new List<string> { "Foo", "Fool", "Foot bal" }
                });
            googleMock.GetPredictionsAsync("Ben").Returns(
                new AutocompleteResult
                {
                    Status = GoogleMapsConstants.STATUS_OK,
                    Predictions = new List<string> { "Bennekom", "Big Ben", "Ben fietsen" }
                });
        }

        [TestMethod]
        public async Task GetAutocompletePredictionsAsync_SingleCall_PredictionsOk()
        {
            //arrange
            var tester = new AutocompleteTester( policyFactory, googleMock );

            //act
            var result = await tester.GetAutocompletePredictionsAsync("Foo");

            //assert
            result.LastOrDefault().Should().Be( "Foot bal" );
        }

        [TestMethod]
        public async Task GetAutocompletePredictionsAsync_Caching_SecondCallReturnsFromCache()
        {
            //arrange
            var tester = new AutocompleteTester(policyFactory, googleMock);

            //act
            var result1 = await tester.GetAutocompletePredictionsAsync("Foo");
            var result2 = await tester.GetAutocompletePredictionsAsync("Ben");
            var result3 = await tester.GetAutocompletePredictionsAsync("Foo");

            //assert
            result3.LastOrDefault().Should().Be("Foot bal");
            await googleMock.Received( 1 ).GetPredictionsAsync( "Foo" );
        }
    }
}
