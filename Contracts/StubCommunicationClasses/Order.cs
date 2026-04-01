namespace Contracts.StubCommunicationClasses
{
    public class Order
    {
        public Guid Id { get; set; }
        public IEnumerable<MenuItemQuantity> MenuItems { get; set; }
    }
}
