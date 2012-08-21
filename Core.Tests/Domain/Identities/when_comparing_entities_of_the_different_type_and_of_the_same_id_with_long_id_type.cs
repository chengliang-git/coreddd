using NUnit.Framework;

namespace Core.Tests.Domain.Identities
{
    [TestFixture]
    public class when_comparing_entities_of_the_different_type_and_of_the_same_id_with_long_id_type 
        : base_when_comparing_entities_of_the_different_type_and_of_the_same_id<long>
    {
        protected override long GetId()
        {
            return long.MaxValue;
        }
    }
}