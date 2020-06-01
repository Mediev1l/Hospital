using System;

namespace Hospital
{
    enum Result
    {
        Pending,
        NoDoctorAvailable,
        Offer,
        Accepted,
        Succeed,
        Failed,
    }
    class Request
    {
        public String Source { get; set; }
        public Result Result { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public int Complexity { get; set; }
        public double Price { get; set; }
    }
}
