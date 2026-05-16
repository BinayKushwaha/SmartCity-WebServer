namespace SmartCity.Domain
{
    public class RetailProperty
    {
        public int Id { get; set; }
        public PropertyType Type { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Features { get; set; }
    }
}
