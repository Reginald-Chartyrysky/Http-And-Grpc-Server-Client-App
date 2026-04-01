namespace ClientConsole.Model
{
    public class Barcode
    {
        public int Id { get; set; }
        public int MenuItemServerId { get; set; }
        public string Code { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
