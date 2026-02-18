using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class AttendanceRecordRepository(AppDBContext _context) : Repository<AttendanceRecord>(_context), IAttendanceRecordRepository
{
}
