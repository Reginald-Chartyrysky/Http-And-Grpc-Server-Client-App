namespace ClientConsole.Model
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsWeighted { get; set; }
        public string FullPath { get; set; }
        public ICollection<Barcode> Barcodes { get; set; }
    }
}
