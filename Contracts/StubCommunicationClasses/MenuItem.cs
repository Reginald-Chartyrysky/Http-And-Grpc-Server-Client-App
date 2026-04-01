namespace Contracts.StubCommunicationClasses
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsWeighted { get; set; }
        public string FullPath { get; set; }
        public HashSet<string> BarCodes { get; set; }
    }
}
