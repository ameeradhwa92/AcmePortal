using AcmePortal.Common;
using AcmePortal.Model;

namespace AcmePortal.Tests.Workflow
{
    public class CustomerRulesTests
    {
        [Fact]
        public void ValidateDeactivation_NoOpenOrders_IsValid()
        {
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (isValid, error) = CustomerRules.ValidateDeactivation(customer, 0);
            Assert.True(isValid);
            Assert.Null(error);
        }

        [Fact]
        public void ValidateDeactivation_HasOpenOrders_IsInvalid()
        {
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (isValid, error) = CustomerRules.ValidateDeactivation(customer, 3);
            Assert.False(isValid);
            Assert.NotNull(error);
        }
    }
}
