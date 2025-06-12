using System.Data;      
namespace SkyLineShoppers.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public static Cart Create(IDataReader reader)
        {
            var i = new Cart
            {
                CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
            };
            return i;
        }
    }
    public class Wishlist
    {
        public int wishlistId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public static Wishlist Create(IDataReader reader)
        {
            var i = new Wishlist
            {
                wishlistId = reader.GetInt32(reader.GetOrdinal("wishlistId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
            };
            return i;
        }
    }
    public class cartList
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public static cartList Create(IDataReader reader)
        {
            var i = new cartList
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? "" : reader.GetString(reader.GetOrdinal("Image")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
            };
            return i;
        }
    }
}
