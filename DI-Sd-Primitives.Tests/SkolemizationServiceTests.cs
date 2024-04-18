using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Tests
{
    public class SkolemizationServiceTests
    {
        [Theory]
        [InlineData("_:subject <urn:example:predicate> <urn:example:object> .", "<urn:example:subject> <urn:example:predicate> <urn:example:object> .\n")]
        [InlineData("_:subject <urn:example:predicate> _:object .", "<urn:example:subject> <urn:example:predicate> <urn:example:object> .\n")]
        [InlineData("_:subject <urn:example:predicate> \"literal object\" .", "<urn:example:subject> <urn:example:predicate> \"literal object\" .\n")]
        [InlineData("_:subject <urn:example:predicate> _:object1 .\n_:object1 <urn:example:predicate2> _:object2 .", "<urn:example:subject> <urn:example:predicate> <urn:example:object1> .\n<urn:example:object1> <urn:example:predicate2> <urn:example:object2> .\n")]
        [InlineData("_:subject <urn:example:predicate> _:object1 .\n_:object1 <urn:example:predicate2> \"literal object2\" .", "<urn:example:subject> <urn:example:predicate> <urn:example:object1> .\n<urn:example:object1> <urn:example:predicate2> \"literal object2\" .\n")]
        public void Skolemize_ReturnsSkolemizedNQuads(string nQuad, string expectedSkolemizedNQuad)
        {
            // Arrange
            var service = new SkolemizationService();
            var nQuads = nQuad.Split('\n').ToList();
            var expectedNQuads = expectedSkolemizedNQuad.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l + "\n").ToList();
            var urnScheme = "example";

            // Act
            var skolemizedNQuads = service.Skolemize(nQuads, urnScheme);

            // Assert
            var i = 0;
            foreach (var skolemizedNQuad in skolemizedNQuads)
            {
                Assert.Equal(expectedNQuads[i++], skolemizedNQuad);
            }
            
        }

        [Theory]
        [InlineData("<urn:example:subject> <urn:example:predicate> <urn:example:object>.", "_:subject <urn:example:predicate> _:object .\n")]
        [InlineData("<urn:example:subject> <urn:example:predicate> _:object.", "_:subject <urn:example:predicate> _:object .\n")]
        [InlineData("<urn:example:subject> <urn:example:predicate> \"literal object\".", "_:subject <urn:example:predicate> \"literal object\" .\n")]
        [InlineData("<urn:example:subject> <urn:example:predicate> <urn:example:object1>.\n<urn:example:object1> <urn:example:predicate2> _:object2 .", "_:subject <urn:example:predicate> _:object1 .\n_:object1 <urn:example:predicate2> _:object2 .\n")]
        [InlineData("<urn:example:subject> <urn:example:predicate> <urn:example:object1>.\n<urn:example:object1> <urn:example:predicate2> \"literal object2\" .", "_:subject <urn:example:predicate> _:object1 .\n_:object1 <urn:example:predicate2> \"literal object2\" .\n")]
        public void Deskolemize_ReturnsDeskolemizedNQuads(string nQuad, string expectedDeskolemizedNQuad)
        {
            // Arrange
            var service = new SkolemizationService();
            var nQuads = nQuad.Split('\n').ToList();
            var expectedNQuads = expectedDeskolemizedNQuad.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l + "\n").ToList();

            var urnScheme = "example";

            // Act
            var deskolemizedNQuads = service.Deskolemize(nQuads, urnScheme);

            // Assert
            var i = 0;
            foreach (var deskolemizedNQuad in deskolemizedNQuads)
            {
                Assert.Equal(expectedNQuads[i++], deskolemizedNQuad);
            }
        }
    }
}
