using APBD_3.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.Services
{
    public interface IStudentServiceDb
    {
        public  void EnrollStudent(EnrollStudentRequest request)
        {
            
        }
         public  void PromoteStudents(String Studies , int Semester)
        {

        }
    }
}
