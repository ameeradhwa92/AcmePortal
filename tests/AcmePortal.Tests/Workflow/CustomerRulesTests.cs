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
        public void ValidateDeactivation_NoOpenOrders_ErrorMessageIsNull()
        {
            // Explicitly documents the null-message contract on success
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (_, error) = CustomerRules.ValidateDeactivation(customer, 0);
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

        [Fact]
        public void ValidateDeactivation_HasOpenOrders_ErrorMessage_ContainsOrderCount()
        {
            // The interpolated message should include the exact count provided
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (_, error) = CustomerRules.ValidateDeactivation(customer, 5);
            Assert.Contains("5", error);
        }

        [Fact]
        public void ValidateDeactivation_ExactlyOneOpenOrder_IsInvalid()
        {
            // Boundary: the minimum count that should trigger the guard
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (isValid, error) = CustomerRules.ValidateDeactivation(customer, 1);
            Assert.False(isValid);
            Assert.NotNull(error);
        }

        [Fact]
        public void ValidateDeactivation_NegativeOpenOrderCount_IsValid()
        {
            // A bad caller passing -1: openOrderCount > 0 is false, so it passes.
            // This test documents (and pins) that current behaviour.
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (isValid, _) = CustomerRules.ValidateDeactivation(customer, -1);
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateDeactivation_NullCustomer_DoesNotThrow()
        {
            // The method never reads from Customer, so null should not throw.
            // Documents the contract boundary.
            var exception = Record.Exception(() =>
                CustomerRules.ValidateDeactivation(null!, 0));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateDeactivation_LargeOpenOrderCount_IsInvalid()
        {
            // Confirm behaviour doesn't break at extreme values
            var customer = new Customer { Id = 1, Name = "Acme" };
            var (isValid, error) = CustomerRules.ValidateDeactivation(customer, int.MaxValue);
            Assert.False(isValid);
            Assert.NotNull(error);
        }
    }
}
