namespace SmartHealthcare.Core.Enums;

public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}

public enum AppointmentType
{
    InPerson = 0,
    Online = 1,
    Phone = 2
}

public enum UserRole
{
    Admin = 0,
    Doctor = 1,
    Patient = 2
}
