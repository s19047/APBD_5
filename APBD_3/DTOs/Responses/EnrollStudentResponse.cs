using APBD_3.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public EnrollStudentRequest request {get; set;}

        public int semester { get; set; }
    }
}
