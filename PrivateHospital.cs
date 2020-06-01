using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hospital
{
    class PrivateHospital
    {
        private int succeed;
        private int failed;
        private Thread thread;
        private Queue<Request> requests;
        private List<Patient> patients;
        private List<Doctor> doctors;
        private Mutex mutex = new Mutex();

        public PrivateHospital()
        {
            requests = new Queue<Request>();
            patients = new List<Patient>();
            doctors = new List<Doctor>();
            thread = new Thread(run);

            thread.Start();
        }

        private void run()
        {
            Request response;
            Doctor tempDoctor = null;
            while(true)
            {
                Console.Clear();

                Console.WriteLine("=================Hospital=================");
                Console.WriteLine($"Succeed {succeed}");
                Console.WriteLine($"Failed {failed}");
                Console.WriteLine("==========================================\n");

                Console.WriteLine("==================Doctors=================");
                foreach(var d in doctors.ToArray())
                    Console.WriteLine($"Doctor id {d.Id} === Experience {d.Experience} === Availability {d.SurgeryAvailable}");
                Console.WriteLine("==========================================\n");

                Console.WriteLine("==================Patients================");
                foreach (var p in patients.ToArray())
                    Console.WriteLine($"Patient id {p.Id} === Complexity {p.Injury.Complexity}");
                Console.WriteLine("==========================================\n");

                if(requests.Count > 0)
                {
                    if(requests.Peek().Source == "Patient")
                    {
                        if(doctors.Count > 0)
                        {
                            response = requests.Dequeue();

                            if (response.Result == Result.Accepted)
                            {
                                tempDoctor = doctors.Find(d => response.DoctorID == d.Id);
                            }
                            else if (response.Result == Result.Pending)
                            {
                                tempDoctor = doctors.Find(d => d.Experience >= response.Complexity);
                            }


                            if(tempDoctor == null)
                            {
                                response.Source = "Hospital";
                                response.Result = Result.NoDoctorAvailable;

                                patients.Find(i => i.Id == response.PatientID).Injury = response;
                            }
                            else if (tempDoctor.SurgeryAvailable > 0 && response.Result == Result.Accepted)
                            {
                                response.Source = "Doctor";
                                tempDoctor.surgery(ref response);
                            }
                            else if (tempDoctor.SurgeryAvailable > 0)
                            {
                                response.Source = "Hospital";
                                response.Result = Result.Offer;
                                response.Price = tempDoctor.getPrice(response.Complexity);
                                response.DoctorID = tempDoctor.Id; 

                                patients.Find(i => i.Id == response.PatientID).Injury = response;
                            }
                            else
                            {
                                response.Source = "Hospital";
                                response.Result = Result.NoDoctorAvailable;

                                patients.Find(i => i.Id == response.PatientID).Injury = response;
                            }

                        }
                        else
                        {
                            response = requests.Dequeue();
                            response.Source = "Hospital";
                            response.Result = Result.NoDoctorAvailable;

                            patients.Find(i => i.Id == response.PatientID).Injury = response;
                        }
                    }
                    else if(requests.Peek().Source == "Doctor")
                    {
                        response = requests.Dequeue();

                        if (response.Result == Result.Succeed)
                            succeed++;
                        else
                            failed++;

                        var tempPatient = patients.Find(i => i.Id == response.PatientID);

                        tempPatient.Injury = response;
                        dischargePatient(tempPatient.Id);
                    }
                }
                else
                {
                    Thread.Sleep(2000);
                }
                response = null;
            }
        }

        public void registerPatient(ref Patient patient)
        {
            mutex.WaitOne();

            patients.Add(patient);

            mutex.ReleaseMutex();
        }

        public void registerDoctor(ref Doctor doctor)
        {
            mutex.WaitOne();

            doctors.Add(doctor);

            mutex.ReleaseMutex();
        }

        public void dischargePatient(int id)
        {
            mutex.WaitOne();

            patients.RemoveAll(x => x.Id == id);

            mutex.ReleaseMutex();

        }

        public void dischargeDoctor(int id)
        {
            mutex.WaitOne();

            doctors.RemoveAll(x => x.Id == id);

            mutex.ReleaseMutex();
        }

        public void passRequest(Request request)
        {
            mutex.WaitOne();

            requests.Enqueue(request);

            mutex.ReleaseMutex();
        }

        public int doctorCount()
        {
            return doctors.Count;
        }
    }
}
