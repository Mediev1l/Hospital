using System;
using System.Threading;

namespace Hospital
{
    class Patient
    {
        public static int patientCount { get; set; }
        public int Id { get; set; }
        public double Budged { get; set; }
        public Request Injury { get; set; }

        private Thread thread;
        private PrivateHospital hospital;

        public Patient(ref PrivateHospital hospital)
        {
            Func<double, double,double, double> doubleFromRange = (min,  max, rand) => min + (rand * (max - min));
            Random rnd = new Random();

            this.hospital = hospital;
            Id = patientCount;
            Budged = doubleFromRange(50, 1000, rnd.NextDouble());
            Injury = new Request()
            {
                PatientID = Id,
                Result = Result.Pending,
                Complexity = rnd.Next(1, 10),
                Source = "Patient" 
            };

            goToHospital();

            thread = new Thread(await);

            patientCount++;

            thread.Start();
        }

        private void await()
        {
            while (Injury != null)
            {
                if (Injury.Source == "Patient")
                    Thread.Sleep(3000);
                else if (Injury.Source == "Hospital")
                {
                    if (Injury.Result == Result.Offer)
                    {
                        if (Injury.Price <= Budged)
                        {
                            Injury.Source = "Patient";
                            Injury.Result = Result.Accepted;
                            hospital.passRequest(Injury);
                        }
                        else
                        {
                            Injury.Source = "Patient";
                            Injury.Result = Result.Pending;
                            goToHospital();
                        }
                    }
                    else if (Injury.Result == Result.NoDoctorAvailable)
                    {
                        Injury.Source = "Patient";
                        Injury.Result = Result.Pending;
                        goToHospital();
                    }
                }
                else if (Injury.Source == "Doctor")
                {
                    Injury = null;
                }
            }
        }

        private void goToHospital()
        {
            hospital.passRequest(Injury);
        }


    }
}
