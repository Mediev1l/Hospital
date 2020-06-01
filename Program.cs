using System;
using System.Threading;

namespace Hospital
{
    class Program
    {
        static void Main(string[] args)
        {

            PrivateHospital h1 = new PrivateHospital();

            while(true)
            {
                if(h1.doctorCount() < 10)
                {
                    Doctor d1 = new Doctor(ref h1);
                    h1.registerDoctor(ref d1);
                }

                Patient p1 = new Patient(ref h1);
                h1.registerPatient(ref p1);

                Thread.Sleep(2000);   
            }
        }
    }
}
