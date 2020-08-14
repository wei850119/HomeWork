using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Homework.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text;

namespace Homework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            #region remark

            //if (id != department.DepartmentId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(department).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DepartmentExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();

            #endregion

            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            try
            {
                StringBuilder hex = new StringBuilder(department.RowVersion.Length*2);
                foreach (byte b in department.RowVersion)
                    hex.AppendFormat("{0:x2}",b);
                var result = await _context.Database.ExecuteSqlRawAsync($"EXEC dbo.Department_Update {department.DepartmentId},N'{department.Name}',{department.Budget},'{department.StartDate.ToString("yyyy-MM-dd HH:mm:ss")}',{department.InstructorId},0x{hex.ToString()}");

                #region solution 2
                // var result = await _context.Department.FindAsync(id);
                // string TDATE = Convert.ToDateTime(department.StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                // int DepartmentID = department.DepartmentId;
                // string Name = department.Name;
                // decimal Budget = department.Budget;
                // int? InstructorId = department.InstructorId;
                // byte[] RowVersion = result.RowVersion;
                // await _context.Database.ExecuteSqlRawAsync("exec dbo.Department_Update @DepartmentID = {0}, @Name = {1}, @Budget = {2}, @StartDate = {3}, @InstructorID = {4}, @RowVersion_Original = {5}", id, Name, Budget, TDATE, InstructorId, RowVersion);
                #endregion

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            #region remark
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
            #endregion

            var data = await _context.Department.FromSqlRaw("EXECUTE dbo.Department_Insert {0},{1},{2},{3}", department.Name, department.Budget, department.StartDate.ToString("yyyy-MM-dd HH:mm:ss"), department.InstructorId).ToListAsync();
            
            department = data.FirstOrDefault();
            // department.DepartmentId = data[0].DepartmentId;
            // department.RowVersion = data[0].RowVersion;

            // var result = await _context.Database.ExecuteSqlRawAsync
            // (
            //     "EXECUTE dbo.Department_Insert {0},{1},{2},{3}",
            //     department.Name,
            //     department.Budget,
            //     department.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
            //     department.InstructorId
            // );

            // if(result!=0){
            //     return BadRequest();
            // }
            // else
            // {
            //     var result = _context.Department.
            // }

            return CreatedAtAction("GetDepartment", new { id = data[0].DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            #region remark
            //var department = await _context.Department.FindAsync(id);
            //if (department == null)
            //{
            //    return NotFound();
            //}

            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();

            //return department;
            #endregion

            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var param = new[] {
                new SqlParameter("@DepartmentId", department.DepartmentId),
                new SqlParameter("@RowVersion", department.RowVersion)
            };

            await _context.Database.ExecuteSqlRawAsync("EXECUTE dbo.Department_Delete @DepartmentId,@RowVersion", param);

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }

        // GET: api/Departments
        [HttpGet("~/api/VwDepartmentCourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetDepartmentCourseCount()
        {
            return await _context.VwDepartmentCourseCount.FromSqlRaw("SELECT * FROM dbo.VwDepartmentCourseCount").ToListAsync();
        }
    }
}
