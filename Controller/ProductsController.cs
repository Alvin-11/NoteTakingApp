using BlazorApp2.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using Azure.Core;
// There remains the delete notes and the save data api to configure. Afterwards, call all apis and test the to ensure functionality
namespace BlazorApp2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase,Interface
    {
        private readonly AppDbContext _context;

        public ProductsController()
        {
          //  _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessage()
        {
            var jsondata = new Product { Id = 4, Name = "", Price = 8 };
            // Simulate some asynchronous work
            await Task.Delay(1);
            return Ok("Hello from the API!");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff() {
            var name = HttpContext.Session.GetString("isLoggedIn");
            if (name == null) { return Ok(false); }
            if (name.Equals("true"))
            {
                HttpContext.Session.SetString("username", "");
                HttpContext.Session.SetString("isLoggedIn", "false");
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("login")]
        public async Task<IActionResult> UserLogin() {
            var name = HttpContext.Session.GetString("isLoggedIn");
            if (name == null) { return Ok(false); }
            if (name.Equals("true"))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("username")]
        public async Task<IActionResult> UserName()
        {
            var name = HttpContext.Session.GetString("isLoggedIn");
            if (name == null) { return Ok("false"); }
            if (name.Equals("true"))
            {
                var username1 = HttpContext.Session.GetString("username");
                return Ok(username1);
            }
            return Ok("false");
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] LoginRequest request)
        {
           // return Ok(true);
            string username = request.Username;
            string password = request.Password;
            string name = request.Name;

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "users.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();
                SqliteCommand command;

                command = connection.CreateCommand();
                command.CommandText =
                @"
                   CREATE TABLE IF NOT EXISTS Users (
                   Id INTEGER PRIMARY KEY AUTOINCREMENT,
                   Username TEXT NOT NULL,
                   Password TEXT NOT NULL,
                   Name TEXT NOT NULL
                );
                 ";
                command.ExecuteNonQuery();
                command.CommandText =
                @"
                  INSERT INTO Users (Username, Password,Name)
                  VALUES ($username, $password,$name);
               ";
                command.Parameters.AddWithValue("$username", username);
                command.Parameters.AddWithValue("$password", password); // Hash in real apps
                command.Parameters.AddWithValue("$name", name);

                command.ExecuteNonQuery();
                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception error: {ex.ToString()}");
            }
            finally
            {
                connection.Close();
               
            }

            return NotFound(false);
        }


        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest1 request)
        {
            string username = request.Username;
            string password = request.Password;

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "users.db");
            string connectionString = $"Data Source={dbPath};";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT 1
            FROM Users
            WHERE Username = $username AND Password = $password
            LIMIT 1;
        ";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", password); // For production, store hashed passwords!

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                HttpContext.Session.SetString("username", request.Username);
                HttpContext.Session.SetString("isLoggedIn", "true");
                return Ok(true);
            }
            connection.Close();
            return NotFound(false);
        }


        [HttpPost("createNote")]
        public async Task<IActionResult> CreateNote([FromBody] CreateNote request)
        {
            string title = request.Title;
            if (DoesTitleExist(title))
            {
                return Ok("Title already exists");
            }
            string note = request.Note;
            var name = HttpContext.Session.GetString("username");
            if (name == null) { return Ok(false); }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();
                SqliteCommand command;

                command = connection.CreateCommand();
                command.CommandText =
                $@"
                   CREATE TABLE IF NOT EXISTS [{name}] (
                   Id INTEGER PRIMARY KEY AUTOINCREMENT,
                   Title TEXT NOT NULL,
                   Note TEXT NOT NULL
                );
                 ";
                command.ExecuteNonQuery();
                command.CommandText =
                $@"
                  INSERT INTO [{name}] (Title,Note)
                  VALUES ($title, $note);
               ";
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$note", note); // Hash in real apps

                command.ExecuteNonQuery();
                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception error: {ex.ToString()}");
            }
            finally
            {
                connection.Close();

            }

            return Ok(false);
        }

        [HttpPost("GetNote")]
        public async Task<IActionResult> GetNote([FromBody] UpdateNote request)
        {
            string title = request.Title;
            var name = HttpContext.Session.GetString("username");
            if (name == null) { return Ok(false); }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT Note
                    FROM {name}
                    WHERE Title = $title
                    LIMIT 1;
                ";

                command.Parameters.AddWithValue("$title", title);

                var result = command.ExecuteScalar(); 

                if (result != null)
                {
                    string note = result.ToString();
                   
                    return Ok(note);
                }
                else
                {
                    connection.Close();
                    return NotFound("No matching note found.");
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetAllTitles")]
        public async Task<IActionResult> GetAllTitles()
        {
            var name = HttpContext.Session.GetString("username");
            if (name == null) { return Ok(false); }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = $@"
                    SELECT Title FROM {name};
                ";

                using var reader = command.ExecuteReader();

                List<string> titles = new List<string>();

                while (reader.Read())
                {
                    titles.Add(reader.GetString(0));  
                }
                connection.Close();
                return Ok(titles);
            }
            catch (Exception ex)
            {
                connection.Close();
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("SaveNote")]
        public async Task<IActionResult> SaveNote([FromBody] SaveNote request)
        {
            string title = request.Title;
            string oldtitle = request.OldTitle;
            string note = request.Note;
            if (!title.Equals(oldtitle))
            {
                if (DoesTitleExist(title))
                {
                    return Ok("Title already exists");
                }
            }
            var name = HttpContext.Session.GetString("username");
            if (name == null) { return Ok(false); }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT Note
                    FROM {name}
                    WHERE Title = $title
                    LIMIT 1;
                ";

                command.Parameters.AddWithValue("$title", oldtitle);

                var result = command.ExecuteScalar();

                if (result != null)
                {
                    using (var updateCommand = connection.CreateCommand())
                    {

                        if (oldtitle.Equals(title))
                        {
                            updateCommand.CommandText = @"
                            UPDATE " + name + @"
                            SET Note = $newNote
                            WHERE Title = $oldTitle;
                        ";
                            updateCommand.Parameters.AddWithValue("$newNote", note);
                            updateCommand.Parameters.AddWithValue("$oldTitle", oldtitle);
                        }
                        else
                        {
                            updateCommand.CommandText = @"
                            UPDATE " + name + @"
                            SET Title = $newTitle,
                                Note = $newNote
                            WHERE Title = $oldTitle;
                        ";
                            updateCommand.Parameters.AddWithValue("$newTitle", title);
                            updateCommand.Parameters.AddWithValue("$newNote", note);
                            updateCommand.Parameters.AddWithValue("$oldTitle", oldtitle);
                        }
                       

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        connection.Close();
                        return Ok(true);
                    }
                }
                else
                {
                    connection.Close();
                    return NotFound("No matching note found.");
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DeleteNote")]
        public async Task<IActionResult> DeleteNote([FromBody] UpdateNote request)
        {
            string title = request.Title;
            var name = HttpContext.Session.GetString("username");
            if (name == null) { return NotFound(false); }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";
            SqliteConnection connection = new SqliteConnection(connectionString);
            try
            {
                connection.Open();
                SqliteCommand command;

                command = connection.CreateCommand();
                command.CommandText =
                $@"
                   DELETE FROM {name}
                WHERE Title = $title;
    
                );
                 ";
                command.Parameters.AddWithValue("$title", title);

                int rowsAffected  = command.ExecuteNonQuery();
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception error: {ex.ToString()}");
            }
            finally
            {
                connection.Close();
                
            }
            if (DoesTitleExist(title)) {
                return NotFound(false);
            } 
            else 
            {
                return Ok(true);
            }
                
        }

        public bool DoesTitleExist(string title)
        {
            var name = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "notes.db");
            string connectionString = $"Data Source={dbPath};";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = @"
                    SELECT COUNT(*) 
                    FROM sqlite_master 
                    WHERE type='table' AND name=$tableName;
                ";
                checkCmd.Parameters.AddWithValue("$tableName", name);

                var tableExists = (long)checkCmd.ExecuteScalar() > 0;
                if (!tableExists)
                {
                    return false;
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT COUNT(1) 
                    FROM [{name}]
                    WHERE Title = $title;
                ";
                command.Parameters.AddWithValue("$title", title);

                var count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

    }
    public class LoginRequest1
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public class CreateNote
    {
        public string Title { get; set; }
        public string Note { get; set; }

    }

    public class UpdateNote
    {
        public string Title { get; set; }

    }

    public class SaveNote
    {
        public string OldTitle { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }

    }

}
