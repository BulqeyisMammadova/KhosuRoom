using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class NotificationRepository(AppDBContext _context) : Repository<Notification>(_context), INotificationRepository
{
}
