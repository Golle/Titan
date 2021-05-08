using NUnit.Framework;
using Titan.ECS.Components;

namespace Titan.ECS.Tests.Components
{

    internal class ComponentIdTests
    {
        [TestCase(0b100ul)]
        [TestCase(0b1100ul)]
        [TestCase(0b11100ul)]
        [TestCase(0b111100ul)]
        public void Contains_NoMatchLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b11ul, 0);

            var result = component.Contains(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }

        [TestCase(0b110ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b11110ul)]
        public void Contains_PartialMatchOnLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b11ul, 0);

            var result = component.Contains(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }

        [TestCase(0b0001ul)]
        [TestCase(0b0011ul)]
        [TestCase(0b0110ul)]
        [TestCase(0b1100ul)]
        public void Contains_FullMatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(low, 0);

            var result = component.Contains(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }

        [TestCase(0b0001ul)]
        [TestCase(0b0011ul)]
        [TestCase(0b0110ul)]
        [TestCase(0b1100ul)]
        public void Contains_PartialMatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(0b1111ul, 0);

            var result = component.Contains(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }


        [TestCase(0b1100ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b1111ul)]
        [TestCase(0b0111ul)]
        public void Equals_NoMatchOnLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b0011ul, 0);

            var result = component.Equals(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }

        [TestCase(0b1100ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b1111ul)]
        [TestCase(0b0111ul)]
        public void Equals_MatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(low, 0);

            var result = component.Equals(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }


        [TestCase(0b0010ul)]
        [TestCase(0b0110ul)]
        [TestCase(0b1110ul)]
   
        public void IsSubet_NoMatchOnLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b0001ul, 0);

            var result = component.IsSubsetOf(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }

        [TestCase(0b0001ul)]
        [TestCase(0b0010ul)]
        [TestCase(0b0100ul)]
        [TestCase(0b1100ul)]

        public void IsSubet_PartialMatchOnLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b1111ul, 0);

            var result = component.IsSubsetOf(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }

        [TestCase(0b1011ul)]
        [TestCase(0b0111ul)]
        [TestCase(0b1111ul)]

        public void IsSubset_MatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(0b0011ul, 0);

            var result = component.IsSubsetOf(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }

        [TestCase(0b1100ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b1111ul)]
        [TestCase(0b0111ul)]

        public void IsSubset_FullMatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(low, 0);

            var result = component.IsSubsetOf(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }



        [TestCase(0b1000ul)]
        [TestCase(0b1100ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b0110ul)]
        public void MatchesAny_NoMatchOnLow_ReturnFalse(ulong low)
        {
            var component = new ComponentId(0b10001ul, 0);

            var result = component.MatchesAny(new ComponentId(low, 0));

            Assert.That(result, Is.False);
        }
        
        [TestCase(0b1000ul)]
        [TestCase(0b1100ul)]
        [TestCase(0b1110ul)]
        [TestCase(0b0111ul)]
        [TestCase(0b0011ul)]
        [TestCase(0b0001ul)]
        public void MatchesAny_MatchOnLow_ReturnTrue(ulong low)
        {
            var component = new ComponentId(0b1111ul, 0);

            var result = component.MatchesAny(new ComponentId(low, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void MatchesAny_ULongMaxMatchOnLow_ReturnTrue()
        {
            var component = new ComponentId(0b1ul, 0);

            var result = component.MatchesAny(new ComponentId(ulong.MaxValue, 0));

            Assert.That(result, Is.True);
        }
    }
}
