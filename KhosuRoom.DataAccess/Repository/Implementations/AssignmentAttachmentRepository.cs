using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class AssignmentAttachmentRepository(AppDBContext _context) : Repository<AssignmentAttachment>(_context), IAssignmentAttachmentRepository
{
}
