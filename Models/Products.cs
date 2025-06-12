using System.Data;

namespace SkyLineShoppers.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public static Product Create(IDataReader reader)
        {
            var i = new Product
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? "" : reader.GetString(reader.GetOrdinal("Image"))
            };
            return i;
        }
    }
}
