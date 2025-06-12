using DataAccessBlock;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data;
using SkyLineShoppers.Models; // Update this if Product model is in another namespace

namespace SkyLineShoppers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataAccess objdab;

        public ProductsController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            objdab = new DataAccess(connectionString);
        }
        #region Product
        [HttpGet("GetProductList")]
        public ActionResult<IEnumerable<Product>> GetProductList()
        {
            try
            {
                string strsql = "SELECT ProductId, Name, Description, Price, Image FROM Products";
                ArrayList arrList = new();
                var ProductList = objdab.GetList<Product>(strsql, CommandType.Text, arrList);
                return Ok(ProductList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("GetProducts/{ProductId}")]
        public ActionResult GetProducts(int ProductId)
        {
            ArrayList arrList = new ArrayList();
            List<Product> ProductList = new List<Product>();
            try
            {
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", ProductId));
                string strsql = "SELECT ProductId, Name, Description, Price, Image FROM Products where ProductId=?";
                ProductList = objdab.GetList<Product>(strsql, CommandType.Text, arrList).ToList();
                return Ok(ProductList);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        #endregion

        #region Cart

        [HttpPost("AddtoCart")]
        public ActionResult AddtoCart([FromBody] Cart objCart)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                if (objCart.Quantity == 1 && isExistCart(objCart.UserId, objCart.ProductId))
                {
                    arrList.Clear();
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.UserId));
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.ProductId));
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Quantity", DbType.Int32, 0, ParameterDirection.Input, "", objCart.Quantity));
                    string strsql = "insert into [Cart] ([UserId],[ProductId],[Quantity]) values(?,?,?)";
                    int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                    return Ok("Product Added to Cart Successfully");
                }
                else
                {
                    arrList.Clear();
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Quantity", DbType.Int32, 0, ParameterDirection.Input, "", objCart.Quantity));
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.UserId));
                    arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.ProductId));
                    string strsql = "update [Cart] set [Quantity]=? where [UserID]=? and [ProductID]=?";
                    int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                    return Ok("Product Quantity Updated Successfully");
                }

            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        private bool isExistCart(int UserId, int ProductId)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", UserId));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", ProductId));
                string strsql = "select * from [Cart] where [UserID]=? and [ProductID]=?";
                List<Cart> objListCart = objdab.GetList<Cart>(strsql, CommandType.Text, arrList).ToList();
                if (objListCart.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        [HttpPost("GetCartItems/{UserId}")]
        public ActionResult GetCartItems(int UserId)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", UserId));
                string strsql = "SELECT p.[ProductId], p.[Name], p.[Description], p.[Price], p.[Image], c.[Quantity] FROM [Products] p INNER JOIN [Cart] c ON p.[ProductId] = c.[ProductID] WHERE c.[UserID] = ? AND c.[Quantity] != 0";
                var cartList = objdab.GetList<cartList>(strsql, CommandType.Text, arrList).ToList();
                return Ok(cartList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("RemoveItem")]
        public ActionResult RemoveItem([FromBody] Cart objCart)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.UserId));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objCart.ProductId));
                string strsql = "delete from [Cart] where [UserId]=? and [ProductId]=?";
                int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                return Ok("Product Removed from Cart Successfully");
            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        #endregion

        #region Wishlist
        [HttpPost("AddToWishlist")]
        public ActionResult AddToWishlist([FromBody] Wishlist objWishlist)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.UserId));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.ProductId));
                string strsql = "insert into [Wishlist] ([UserId],[ProductId]) values(?,?)";
                int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                return Ok("Product Added to Wishlist Successfully");
            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("RemoveFromWishlist")]
        public ActionResult RemoveFromWishlist([FromBody] Wishlist objWishlist)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.UserId));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.ProductId));
                string strsql = "delete from [Wishlist] where [UserId]=? and [ProductId]=?";
                int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                return Ok("Product Removed from Wishlist Successfully");
            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("IsInWishlist")]
        public ActionResult IsInWishlist([FromBody] Wishlist objWishlist)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.UserId));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProductId", DbType.Int32, 0, ParameterDirection.Input, "", objWishlist.ProductId));
                string strsql = "select * from [Wishlist] where [UserId]=? and [ProductId]=?";
                List<Wishlist> objListWishlist = objdab.GetList<Wishlist>(strsql, CommandType.Text, arrList).ToList();
                if (objListWishlist.Count > 0)
                {
                    return Ok("true");
                }
                else
                {
                    return Ok("false");
                }
            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        #endregion
    }
}
