using AcmePortal.Model;

namespace AcmePortal.Common
{
    public static class CustomerRules
    {
        public static (bool IsValid, string? ErrorMessage) ValidateDeactivation(Customer customer, int openOrderCount)
        {
            if (openOrderCount > 0)
                return (false, $"Cannot deactivate customer with {openOrderCount} open order(s).");

            return (true, null);
        }
    }
}
