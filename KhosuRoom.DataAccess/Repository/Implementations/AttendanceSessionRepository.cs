using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class AttendanceSessionRepository(AppDBContext _context) : Repository<AttendanceSession>(_context), IAttendanceSessionRepository
{
}
