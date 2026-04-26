using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await SeedDoctorsAsync(db);
        await SeedPatientsAsync(db);
        await SeedAppointmentsAsync(db);
        await SeedBillsAsync(db);
        await SeedPrescriptionsAsync(db);
    }

    private static async Task SeedDoctorsAsync(ApplicationDbContext db)
    {
        if (await db.Doctors.AnyAsync())
        {
            return;
        }

        var specializationIds = await db.Specializations
            .Select(s => s.Id)
            .ToListAsync();

        if (specializationIds.Count == 0)
        {
            return;
        }

        var doctorUsers = new List<User>
        {
            new()
            {
                FullName = "Dr. Aarav Mehta",
                Email = "doctor1@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Neha Iyer",
                Email = "doctor2@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Rohan Kapoor",
                Email = "doctor3@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Priya Sharma",
                Email = "doctor4@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Dr. Vikram Singh",
                Email = "doctor5@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                Role = "Doctor",
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Users.AddRangeAsync(doctorUsers);
        await db.SaveChangesAsync();

        var doctors = new List<Doctor>
        {
            new()
            {
                UserId = doctorUsers[0].Id,
                DepartmentId = 1,
                LicenseNumber = "LIC-CARD-1001",
                YearsOfExperience = 9,
                ConsultationFee = 800,
                Phone = "9876543210",
                IsAvailable = true,
                AvailabilitySchedule = "Mon-Fri 10:00-15:00"
            },
            new()
            {
                UserId = doctorUsers[1].Id,
                DepartmentId = 2,
                LicenseNumber = "LIC-DERM-1002",
                YearsOfExperience = 6,
                ConsultationFee = 650,
                Phone = "9876543211",
                IsAvailable = true,
                AvailabilitySchedule = "Mon-Sat 09:00-13:00"
            },
            new()
            {
                UserId = doctorUsers[2].Id,
                DepartmentId = 4,
                LicenseNumber = "LIC-ORTH-1003",
                YearsOfExperience = 11,
                ConsultationFee = 900,
                Phone = "9876543212",
                IsAvailable = true,
                AvailabilitySchedule = "Tue-Sat 11:00-17:00"
            },
            new()
            {
                UserId = doctorUsers[3].Id,
                DepartmentId = 5,
                LicenseNumber = "LIC-PEDI-1004",
                YearsOfExperience = 7,
                ConsultationFee = 600,
                Phone = "9876543213",
                IsAvailable = true,
                AvailabilitySchedule = "Mon-Fri 09:30-14:30"
            },
            new()
            {
                UserId = doctorUsers[4].Id,
                DepartmentId = 3,
                LicenseNumber = "LIC-NEURO-1005",
                YearsOfExperience = 12,
                ConsultationFee = 950,
                Phone = "9876543214",
                IsAvailable = true,
                AvailabilitySchedule = "Wed-Sun 12:00-18:00"
            }
        };

        await db.Doctors.AddRangeAsync(doctors);
        await db.SaveChangesAsync();

        var doctorSpecializations = new List<DoctorSpecialization>
        {
            new() { DoctorId = doctors[0].Id, SpecializationId = 1 }, // Cardiology
            new() { DoctorId = doctors[0].Id, SpecializationId = 6 }, // General Medicine
            new() { DoctorId = doctors[1].Id, SpecializationId = 2 }, // Dermatology
            new() { DoctorId = doctors[2].Id, SpecializationId = 4 }, // Orthopedics
            new() { DoctorId = doctors[3].Id, SpecializationId = 5 }, // Pediatrics
            new() { DoctorId = doctors[4].Id, SpecializationId = 3 }  // Neurology
        };

        await db.DoctorSpecializations.AddRangeAsync(doctorSpecializations);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPatientsAsync(ApplicationDbContext db)
    {
        if (await db.Patients.AnyAsync())
        {
            return;
        }

        var patientUsers = new List<User>
        {
            new()
            {
                FullName = "Raj Patel",
                Email = "patient1@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Anjali Verma",
                Email = "patient2@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Amit Kumar",
                Email = "patient3@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Sneha Gupta",
                Email = "patient4@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FullName = "Arjun Desai",
                Email = "patient5@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Users.AddRangeAsync(patientUsers);
        await db.SaveChangesAsync();

        var patients = new List<Patient>
        {
            new()
            {
                UserId = patientUsers[0].Id,
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Male",
                BloodGroup = "O+",
                Address = "123 Main Street, Mumbai, MH 400001",
                Phone = "9111111111",
                MedicalHistory = "Mild hypertension"
            },
            new()
            {
                UserId = patientUsers[1].Id,
                DateOfBirth = new DateTime(1992, 8, 22),
                Gender = "Female",
                BloodGroup = "A+",
                Address = "456 Park Avenue, Delhi, DL 110001",
                Phone = "9122222222",
                MedicalHistory = "Seasonal allergies"
            },
            new()
            {
                UserId = patientUsers[2].Id,
                DateOfBirth = new DateTime(1988, 3, 10),
                Gender = "Male",
                BloodGroup = "B+",
                Address = "789 Garden Lane, Bangalore, KA 560001",
                Phone = "9133333333",
                MedicalHistory = "Type 2 diabetes"
            },
            new()
            {
                UserId = patientUsers[3].Id,
                DateOfBirth = new DateTime(1995, 11, 5),
                Gender = "Female",
                BloodGroup = "AB+",
                Address = "321 Elm Street, Hyderabad, TG 500001",
                Phone = "9144444444",
                MedicalHistory = "Asthma"
            },
            new()
            {
                UserId = patientUsers[4].Id,
                DateOfBirth = new DateTime(1985, 7, 18),
                Gender = "Male",
                BloodGroup = "O-",
                Address = "654 Oak Road, Chennai, TN 600001",
                Phone = "9155555555",
                MedicalHistory = "No major medical history"
            }
        };

        await db.Patients.AddRangeAsync(patients);
        await db.SaveChangesAsync();
    }

    private static async Task SeedAppointmentsAsync(ApplicationDbContext db)
    {
        if (await db.Appointments.AnyAsync())
        {
            return;
        }

        var patients = await db.Patients.Take(3).ToListAsync();
        var doctors = await db.Doctors.Take(3).ToListAsync();

        if (patients.Count < 3 || doctors.Count < 3)
        {
            return;
        }

        var appointments = new List<Appointment>
        {
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctors[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(5).AddHours(10),
                Status = "Booked",
                Notes = "Regular checkup for heart condition",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[1].Id,
                DoctorId = doctors[1].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(3).AddHours(14),
                Status = "Booked",
                Notes = "Skin condition consultation",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[2].Id,
                DoctorId = doctors[2].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(7).AddHours(11),
                Status = "Booked",
                Notes = "Orthopedic assessment for back pain",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                PatientId = patients[0].Id,
                DoctorId = doctors[1].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(-2).AddHours(15),
                Status = "Completed",
                Notes = "Follow-up consultation",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        await db.Appointments.AddRangeAsync(appointments);
        await db.SaveChangesAsync();
    }

    private static async Task SeedBillsAsync(ApplicationDbContext db)
    {
        if (await db.Bills.AnyAsync())
        {
            return;
        }

        var appointments = await db.Appointments.Include(a => a.Doctor).ToListAsync();
        if (appointments.Count == 0)
        {
            return;
        }

        var bills = appointments.Select(a => new Bill
        {
            AppointmentId = a.Id,
            ConsultationFee = a.Doctor.ConsultationFee,
            MedicineCharges = a.Status == "Completed" ? 150 : 0,
            PaymentStatus = "Unpaid"
        }).ToList();

        await db.Bills.AddRangeAsync(bills);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPrescriptionsAsync(ApplicationDbContext db)
    {
        if (await db.Prescriptions.AnyAsync())
        {
            return;
        }

        var completedAppointments = await db.Appointments
            .Where(a => a.Status == "Completed")
            .Take(1)
            .ToListAsync();

        if (completedAppointments.Count == 0)
        {
            return;
        }

        var appointment = completedAppointments[0];

        var medicines = new List<Medicine>
        {
            new() { Name = "Aspirin", Dosage = "100mg", Duration = "30 days", Instructions = "Once daily after meals" },
            new() { Name = "Lisinopril", Dosage = "5mg", Duration = "60 days", Instructions = "Once daily in the morning" }
        };

        var prescription = new Prescription
        {
            AppointmentId = appointment.Id,
            Diagnosis = "Hypertension with mild cardiac concerns",
            Notes = "Monitor blood pressure daily. Follow-up after 2 weeks.",
            CreatedAt = DateTime.UtcNow,
            Medicines = medicines
        };

        await db.Prescriptions.AddAsync(prescription);
        await db.SaveChangesAsync();
    }
}