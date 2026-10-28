using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Zxcvbn;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace web_api.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //fix: The path never changes, so it can be readonly.
        //In C#, fields are written in camelCase (filePath)
        readonly string Filepath = "Data.txt";
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Chavi", "Giti" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        //When the user is not found, this returns the text "User not found" with status 200.
        //Return NotFound() (or NoContent()) instead, and Ok(user) when found: change string to ActionResult<User>
        public string Get(int id)
        {
            using (StreamReader reader = System.IO.File.OpenText(Filepath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(currentUserInFile))
                    {
                        continue;
                    }

                    User user = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.Id == id)
                        return JsonSerializer.Serialize(user);
                }
            }
            return "User not found";
        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
         

            if (user == null)
    {
                return BadRequest("Invalid user data");
            }
            int numberOfUsers = System.IO.File.ReadLines(Filepath).Count();
            user.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(user);
            
            System.IO.File.AppendAllText(Filepath, userJson +
              Environment.NewLine);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        //POST api/<Users>/login
        //The assignment asks a wrong login to return NoContent() (204)
        [HttpPost("login")]
        //A full class name is clearer than Userin (for example LoginUser), and in C# parameters are written in camelCase (loginUser)
        public IActionResult Login([FromBody] Userin Userin)
        {
            using (StreamReader reader = System.IO.File.OpenText(Filepath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    

                    User user = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.Email == Userin.Email && user.Password == Userin.Password)
                    {
                        return Ok(user);
                    }

                       
                }
            }
            return NotFound("User not found");


        }




        // POST api/<UsersController>
        [HttpPost("password")]
        public IActionResult Post([FromBody] string password)
        {
           

    
            var result = Zxcvbn.Core.EvaluatePassword(password);

            return Ok(result.Score);


        }



        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User user)
        {

           string textToReplace = string.Empty; 
            using (StreamReader reader = System.IO.File.OpenText(Filepath))

            { 
                string currentUserInFile; 
                while ((currentUserInFile = reader.ReadLine()) != null) 
                { 
 
                    User userInFile = JsonSerializer.Deserialize<User>(currentUserInFile);

                    if (userInFile.Id == id) 
                    {
                        textToReplace = currentUserInFile; 
                        //fix: Found- break
                        break;
                    }
                } 
            } 
 
            if (textToReplace != string.Empty) 
            { 
                //fix: Keep the id, otherwise the updated user is saved without it
                user.Id = id;
                string text = System.IO.File.ReadAllText(Filepath); 
                text = text.Replace(textToReplace, JsonSerializer.Serialize(user));

                System.IO.File.WriteAllText(Filepath, text);
             
                return Ok(user);
            }
            return NoContent();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
