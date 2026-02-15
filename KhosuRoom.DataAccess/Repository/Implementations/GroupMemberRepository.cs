using KhosuRoom.DataAccess.Data;
using KhosuRoom.DataAccess.Repository.Abstarctions;
using KhosuRoom.DataAccess.Repository.Implementations.Generic;

namespace KhosuRoom.DataAccess.Repository.Implementations;

internal class GroupMemberRepository(AppDBContext _context) : Repository<GroupMember>(_context), IGroupMemberRepository
{
}
