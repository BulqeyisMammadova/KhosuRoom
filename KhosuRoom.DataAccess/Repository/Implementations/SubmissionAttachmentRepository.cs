using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class SubmissionAttachmentRepository(AppDBContext _context) : Repository<SubmissionAttachment>(_context), ISubmissionAttachmentRepository
{
}


