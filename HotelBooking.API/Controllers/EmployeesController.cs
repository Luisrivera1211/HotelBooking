using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost("RegisterEmployee")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeDto dto)
        {
            var result = await _employeeService.RegisterAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error registering employee: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("LoginEmployee")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _employeeService.LoginAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error employee login: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
