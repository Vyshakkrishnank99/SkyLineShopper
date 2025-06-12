using DataAccessBlock;
using Microsoft.AspNetCore.Mvc;
using SkyLineShoppers.Models;
using System.Collections;
using System.Data;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkyLineShoppers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataAccess objdab;

        public UsersController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            objdab = new DataAccess(connectionString);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationDto userDto)
        {

            ArrayList arrList = new ArrayList();
            try
            {
                // Save file to a specific location
                string profilePicUrl = null;
                if (userDto.File != null)
                {
                    var fileName = Path.GetFileName(userDto.File.FileName);
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");
                    directoryPath = Path.Combine(directoryPath, userDto.Username);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    var filePath = Path.Combine(directoryPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        userDto.File.CopyTo(fileStream);
                    }
                    string? strFilePath = filePath;
                    profilePicUrl = $"/upload/{userDto.Username}/{userDto.File.FileName}";

                }

                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@FirstName", DbType.String, 50, ParameterDirection.Input, "", userDto.FirstName));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@LastName", DbType.String, 50, ParameterDirection.Input, "", userDto.LastName));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Username", DbType.String, 50, ParameterDirection.Input, "", userDto.Username));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Telephone", DbType.Int64, 0, ParameterDirection.Input, "", userDto.Telephone));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Email", DbType.String, 100, ParameterDirection.Input, "", userDto.Email));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Password", DbType.String, 256, ParameterDirection.Input, "", userDto.Password));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@ProfilePicUrl", DbType.String, 255, ParameterDirection.Input, "", profilePicUrl ?? (object)DBNull.Value));
                string strsql = "insert into [Users] ([FirstName],[LastName],[Username],[Telephone],[Email],[Password],[ProfilePicUrl]) values(@FirstName, @LastName, @Username, @Telephone, @Email, @Password, @ProfilePicUrl)";
                int status = objdab.ExecuteNonQuery(strsql, CommandType.Text, arrList);
                return Ok("User Registered Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login objLogin)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Username", DbType.String, 50, ParameterDirection.Input, "", objLogin.Username));
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Password", DbType.String, 256, ParameterDirection.Input, "", objLogin.Password));

                string strsql = "SELECT * FROM [Users] WHERE [Username]=@Username AND [Password]=@Password";
                List<Users> objListusers = objdab.GetList<Users>(strsql, CommandType.Text, arrList).ToList();

                if (objListusers.Count > 0)
                {
                    var user = objListusers[0];
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("SkyLineShoppers");

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new { Token = tokenString, UserId = user.UserId, Username = user.Username });
                }
                else
                {
                    return Unauthorized("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpGet("GetProfile/{UserId}")]
        public ActionResult GetProfile(int UserId)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Clear();
                arrList.Add(new DataAccessBlock.DataAccess.Parameter("@UserId", DbType.Int32, 0, ParameterDirection.Input, "", UserId));
                string strsql = "SELECT * FROM [Users] WHERE [UserId]=?";
                List<Users> objListusers = objdab.GetList<Users>(strsql, CommandType.Text, arrList).ToList();
                if (objListusers.Count > 0)
                {
                    return Ok(objListusers[0]);
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Use StatusCode to return a more appropriate error response
            }
        }
        //[HttpPost("ForgotPassword/{email}")]
        //public ActionResult ForgotPassword(string email)
        //{
        //    ArrayList arrList = new ArrayList();
        //    try
        //    {
        //        DataAccess objdab = new DataAccess();
        //        arrList.Clear();
        //        arrList.Add(new DataAccessBlock.DataAccess.Parameter("@Email", DbType.String, 100, ParameterDirection.Input, "", email));
        //        string strsql = "select * from [Users] where [Email]=?";
        //        List<Users> objListusers = objdab.GetList<Users>(strsql, CommandType.Text, arrList).ToList();
        //        if (objListusers.Count > 0)
        //        {

        //            var token = Guid.NewGuid().ToString();
        //            var user = objListusers[0];
        //            var password = user.Password;

        //            // Prepare the email content
        //            var subject = "Your Password";
        //            var body = $"Your password is: {password}";

        //            // Send email
        //            SendEmail(email, subject, body);

        //            return Ok("Password sent to your email.");
        //        }
        //        else
        //        {
        //            return Ok("Email not found");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Ok(ex.Message);
        //    }
        //}
        //private void SendEmail(string toEmail, string subject, string body)
        //{
        //    // Configure your email settings
        //    var fromAddress = new MailAddress("vyshak.krishnan@beo.in", "SkyLineShoppers");
        //    var toAddress = new MailAddress(toEmail);
        //    const string fromPassword = "AB#1969!4";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.office365.com", // SMTP server address
        //        Port = 587, // SMTP port
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };

        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = false // Plain text email
        //    })
        //    {
        //        smtp.Send(message);
        //    }
        //}
    }
}
