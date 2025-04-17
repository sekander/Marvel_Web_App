using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarvelWebApp.Models;
using MarvelWebApp.Interface;
using System.Threading.Tasks;
using System.Runtime.ConstrainedExecution;

namespace MarvelWebApp.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminAPIController : BaseEntityController<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // private static readonly userID
        private ApplicationUser _user;

        public AdminAPIController(
            IEntityService<ApplicationUser> entityService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        ) : base(entityService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await GetAllEntities();
        }
        // GET: api/admin/users/email/{email}
        [HttpGet("users/email/{email}")]
        public async Task<IActionResult> GetUserIdByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
            return BadRequest(new { message = "Email is required" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
            return NotFound(new { message = "User not found" });
            }
            else 
                _user = user;

            return Ok(new { userId = user.Id });
        }

        // GET: api/admin/users/5
        [HttpGet("users/{id}")]
        // public async Task<IActionResult> GetUserById(int id)
        public async Task<IActionResult> GetUserById(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            //return await GetEntity(existingUser);
            return Ok (new { user = existingUser});
        }

        // POST: api/admin/users
        [HttpPost("users")]
        // public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user, [FromQuery] string password)
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
        
        {
            if (ModelState.IsValid)
            {


                if (string.IsNullOrEmpty(user.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email is required"
                    });
                }

                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }

                var addUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    // Password = user.Password
                };

                //Debug
                Console.WriteLine("User: " + addUser.UserName);
                Console.WriteLine("Email: " + addUser.Email);
                Console.WriteLine("First Name: " + addUser.FirstName);
                Console.WriteLine("Last Name: " + addUser.LastName);
                Console.WriteLine("Password: " + user.Password);  

                var result = await _userManager.CreateAsync(addUser, user.Password); // Default password for demo purposes
                Console.WriteLine("Result: " + result); 
                if (result.Succeeded)
                {
                    Console.WriteLine("User created successfully ", result);
                    return Ok(new
                    {
                        result
                        // success = true,
                        // message = "User created successfully",
                        // data = user
                    });
                }
                else
                {
                    Console.WriteLine("Error creating user ", result);
                    Console.WriteLine("Error creating user");

                    var errorMessages = result.Errors.Select(e => new
                    {
                        code = e.Code,
                        description = e.Description
                    });

                    return BadRequest(new
                    {
                        success = false,
                        message = "User creation failed",
                        errors = errorMessages
                    });
                }
            }
            return BadRequest(new { message = "Invalid model state", errors = ModelState });
        }

        // PUT: api/admin/users/5
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
        // public async Task<IActionResult> UpdateUser([FromBody] ApplicationUser user)
        {
            // if (id != int.Parse(user.Id))
            if (id != user.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var existingUser = await _userManager.FindByIdAsync(id);
            // var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser == null )
            // if (_user == null)
            {
                return NotFound(new { message = "User not found" });
            }


            // Update user properties
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            // _user.UserName = user.UserName;
            // _user.Email = user.Email;

            // var updateResult = await _userManager.UpdateAsync(existingUser);
            var updateResult = await _userManager.UpdateAsync(_user);
            if (updateResult.Succeeded)
            {
                return Ok(new
                {
                    success = true,
                    message = "User updated successfully",
                    data = user
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Error updating user",
                errors = updateResult.Errors
            });
        }

        // DELETE: api/admin/users/5
        [HttpDelete("users/{id}")]
        // public async Task<IActionResult> DeleteUser(int id)
        public async Task<IActionResult> DeleteUser(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var deleteResult = await _userManager.DeleteAsync(existingUser);
            if (deleteResult.Succeeded)
            {
                return Ok(new
                {
                    success = true,
                    message = "User deleted successfully",
                    data = id
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Error deleting user",
                errors = deleteResult.Errors
            });
        }

        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user = user.UserName, roles });
        }

        // POST: api/admin/assign-role/5
        [HttpPost("assign-role/{userId}")]
        // public async Task<IActionResult> AssignRoleToUser(int userId, [FromBody] string role)
        public async Task<IActionResult> AssignRoleToUser(string userId, [FromBody] string role)
        {
            // var user = await _userManager.FindByIdAsync(userId.ToString());
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest(new { message = $"Role '{role}' does not exist" });
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            // _userManager.RemoveFromRoleAsync(user, "User");
            if (result.Succeeded)
            {
                return Ok(new
                {
                    success = true,
                    message = $"Role '{role}' assigned to user {user.UserName} successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Error assigning role",
                errors = result.Errors
            });
        }


        // POST: api/admin/remove-role/{userId}
        [HttpPost("remove-role/{userId}")]
        public async Task<IActionResult> RemoveRole(string userId, [FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest(new { message = "Role cannot be empty." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            if (!await _userManager.IsInRoleAsync(user, role))
                return BadRequest(new { message = $"User is not in the role '{role}'." });

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to remove role.",
                    errors = result.Errors
                });
            }

            return Ok(new { message = $"Role '{role}' removed from user." });
        }



    }
}
