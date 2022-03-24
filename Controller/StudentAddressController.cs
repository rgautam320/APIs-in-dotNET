using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using SchoolAPI.Data;
using SchoolAPI.Model;

namespace SchoolAPI.Controller
{
    [Route("api/student-addresses")]
    [ApiController]
    public class StudentAddressController : ControllerBase
    {
        private readonly SchoolContext _context;
        public StudentAddressController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentAddress>>> GetStudentAddresses()
        {
            return await _context.StudentAddresses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAddress>> GetStudentAddress(int id)
        {
            var studentAddress = await _context.StudentAddresses.FindAsync(id);
            if (studentAddress == null)
            {
                return BadRequest();
            }
            return studentAddress;
        }

        [HttpPost]
        public async Task<ActionResult<StudentAddress>> PostStudentAddress(StudentAddress studentAddress)
        {
            if (studentAddress == null)
            {
                return BadRequest();
            }
            _context.StudentAddresses.Add(studentAddress);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudentAddress), new { id = studentAddress.StudentAddressId }, studentAddress);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutStudentAddress(int id, StudentAddress studentAddress)
        {
            if (studentAddress == null)
            {
                return BadRequest();
            }
            _context.Entry(studentAddress).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StudentAddresses.Any(e => e.StudentAddressId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAddress(int id)
        {
            var studentAddress = await _context.StudentAddresses.FindAsync(id);
            if (studentAddress == null)
            {
                return NotFound();
            }

            _context.StudentAddresses.Remove(studentAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}