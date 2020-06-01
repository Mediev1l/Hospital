using System;
using System.Threading;

namespace Hospital
{
    class Doctor
    {
        private static int doctorCount;
        public static int fixedPay = 50;
        public int Id { get; set; }
        public int SurgeryAvailable { get; set; }
        public int Experience { get; set; }
        public double PayRatio { get; set; }

        private Thread thread;
        private PrivateHospital hospital;

        public Doctor(ref PrivateHospital hospital)
        {
            Random rnd = new Random();

            Id = doctorCount;
            Experience = rnd.Next(1, 10);
            PayRatio = rnd.NextDouble();
            SurgeryAvailable = rnd.Next(1, 5);
            this.hospital = hospital;

            doctorCount++;

            thread = new Thread(duty);
            thread.Start();
        }

        private void duty()
        {
            while (SurgeryAvailable > 0)
                Thread.Sleep(3000);

            hospital.dischargeDoctor(Id);
        }

        public double getPrice(int complexity)
        {
            return fixedPay * PayRatio * complexity;
        }

        public int getExperience()
        {
            return Experience;
        }

        public bool getAvailability()
        {
            return SurgeryAvailable > 0 ? true : false;
        }

        public void surgery(ref Request injury)
        {
            injury.Source = "Doctor";

            Random rnd = new Random();
            double result = (10 - injury.Complexity) * 0.03 + 0.5;

            if (result <= rnd.NextDouble())
                injury.Result = Result.Failed;
            else
                injury.Result = Result.Succeed;

            SurgeryAvailable--;

            hospital.passRequest(injury);
        }

    }
}
