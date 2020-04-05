using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_3.DTOs.Requests;
using APBD_3.DTOs.Responses;
using APBD_3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD_3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        String CONNECTION_STRING = @"Data Source=LAPTOP-11FAC326\SQLEXPRESS;Initial Catalog=s19047;Integrated Security=True";

        private IStudentServiceDb _service;
        public EnrollmentController(IStudentServiceDb service)
        {
            _service = service;
        }

       [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            _service.EnrollStudent(request);
            
            var response = new EnrollStudentResponse()
            {
                request = request,
                semester = 2020
            };

            return CreatedAtAction(nameof(EnrollStudent),response);
        }

        

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(String Studies,int Semester )
        {
            _service.PromoteStudents(Studies, Semester);
            var response = new EnrollStudentResponse()
            {
                Studies = Studies,
                semester = ++Semester
            };

            return CreatedAtAction(nameof(EnrollStudent), response);
        }
    }
}
