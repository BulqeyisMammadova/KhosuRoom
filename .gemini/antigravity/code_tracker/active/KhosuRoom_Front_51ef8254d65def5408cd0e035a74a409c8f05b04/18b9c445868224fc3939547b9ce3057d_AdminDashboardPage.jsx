ˆZimport React, { useEffect, useState, useMemo } from 'react';
import MainLayout from '../components/layout/MainLayout';
import { getAdminUsers, createTeacher, createStudent, updateAdminUser } from '../api/adminApi';
import { getGroups, createGroup, deleteGroup } from '../api/groupApi';
import { getGroupMembers, addGroupMember, removeGroupMember } from '../api/groupMemberApi';

export default function AdminDashboardPage() {
  const [activeTab, setActiveTab] = useState('users');
  const [users, setUsers] = useState([]);
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  const refreshData = async () => {
    setLoading(true);
    try {
      const [u, g] = await Promise.all([getAdminUsers(), getGroups()]);
      setUsers(Array.isArray(u) ? u : []);
      setGroups(Array.isArray(g) ? g : []);
    } catch (err) {
      setError('System refresh failed. Please check network.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { refreshData(); }, []);

  const showToast = (msg, isError = false) => {
    if (isError) setError(msg); else setSuccess(msg);
    setTimeout(() => { setError(''); setSuccess(''); }, 3000);
  };

  return (
    <MainLayout title="Management Console">
      <div className="page-container fade-in">
        <header style={{ marginBottom: '32px' }}>
          <h1 style={{ fontSize: '28px', fontWeight: 500, color: 'var(--color-primary)' }}>Administrative Controls</h1>
          <p className="text-secondary">Orchestrate your school's users, classrooms, and enrollments.</p>
        </header>

        {success && <div className="card" style={{ backgroundColor: 'var(--color-success)', color: 'white', padding: '12px 24px', marginBottom: '16px' }}>{success}</div>}
        {error && <div className="card" style={{ backgroundColor: 'var(--color-error)', color: 'white', padding: '12px 24px', marginBottom: '16px' }}>{error}</div>}

        <nav className="tabs">
          <button className={`tab ${activeTab === 'users' ? 'active' : ''}`} onClick={() => setActiveTab('users')}>User Management</button>
          <button className={`tab ${activeTab === 'groups' ? 'active' : ''}`} onClick={() => setActiveTab('groups')}>Classrooms</button>
          <button className={`tab ${activeTab === 'members' ? 'active' : ''}`} onClick={() => setActiveTab('members')}>Memberships</button>
        </nav>

        <section>
          {activeTab === 'users' && <UserSection users={users} onRefresh={refreshData} onToast={showToast} />}
          {activeTab === 'groups' && <GroupSection groups={groups} onRefresh={refreshData} onToast={showToast} />}
          {activeTab === 'members' && <MemberSection groups={groups} users={users} onToast={showToast} />}
        </section>
      </div>
    </MainLayout>
  );
}

/* Sub-Sections for Maximum Clarity */

function UserSection({ users, onRefresh, onToast }) {
  const [form, setForm] = useState({ firstName: '', lastName: '', role: 'Student' });
  const [search, setSearch] = useState('');

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      if (form.role === 'Teacher') await createTeacher(form);
      else await createStudent(form);
      onToast(`${form.role} registered successfully`);
      setForm({ firstName: '', lastName: '', role: 'Student' });
      onRefresh();
    } catch (err) { onToast('Registration failed', true); }
  };

  const filteredUsers = users.filter(u => `${u.firstName} ${u.lastName}`.toLowerCase().includes(search.toLowerCase()));

  return (
    <div className="flex flex-col gap-4">
      <div className="card">
        <h3 className="mb-4">Quick Register</h3>
        <form onSubmit={handleCreate} className="flex gap-4 items-center flex-wrap">
          <input className="input" style={{ flex: 1, minWidth: '200px' }} placeholder="First Name" value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })} required />
          <input className="input" style={{ flex: 1, minWidth: '200px' }} placeholder="Last Name" value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })} required />
          <select className="input" style={{ width: '140px' }} value={form.role} onChange={e => setForm({ ...form, role: e.target.value })}>
            <option value="Student">Student</option>
            <option value="Teacher">Teacher</option>
          </select>
          <button className="btn btn-primary" type="submit">Add User</button>
        </form>
      </div>

      <div className="card p-0" style={{ overflow: 'hidden' }}>
        <div style={{ padding: '24px' }}>
          <h3>Global Directory</h3>
          <input className="input mt-4" placeholder="Filter by name..." value={search} onChange={e => setSearch(e.target.value)} />
        </div>
        <div className="data-table-wrapper">
          <table className="data-table">
            <thead>
              <tr>
                <th>Full Name</th>
                <th>Username</th>
                <th>Roles</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map(u => (
                <tr key={u.id || u.Id}>
                  <td>{u.firstName} {u.lastName}</td>
                  <td><code>{u.userName}</code></td>
                  <td>{(u.roles || []).join(', ')}</td>
                  <td>
                    <span style={{ color: u.isActive ? 'var(--color-success)' : 'var(--color-error)' }}>
                      {u.isActive ? 'Active' : 'Deactivated'}
                    </span>
                  </td>
                  <td>
                    <button className="btn-text" onClick={() => updateAdminUser(u.id || u.Id, { ...u, isActive: !u.isActive }).then(onRefresh)}>
                      {u.isActive ? 'Block' : 'Unblock'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function GroupSection({ groups, onRefresh, onToast }) {
  const [name, setName] = useState('');

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      await createGroup({ name, code: 'C-' + Math.floor(Math.random() * 999) });
      onToast('Classroom created');
      setName('');
      onRefresh();
    } catch (err) { onToast('Creation failed', true); }
  };

  return (
    <div className="flex flex-col gap-4">
      <div className="card">
        <h3 className="mb-4">New Classroom</h3>
        <form onSubmit={handleCreate} className="flex gap-4 items-center">
          <input className="input" placeholder="Class Name (e.g. Physics 101)" value={name} onChange={e => setName(e.target.value)} required />
          <button className="btn btn-primary" type="submit">Create</button>
        </form>
      </div>

      <div className="card p-0" style={{ overflow: 'hidden' }}>
        <div style={{ padding: '24px' }}><h3>Classroom List</h3></div>
        <div className="data-table-wrapper">
          <table className="data-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Class Code</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {groups.map(g => (
                <tr key={g.id || g.Id}>
                  <td>{g.name}</td>
                  <td><code>{g.code}</code></td>
                  <td>
                    <button className="btn-text" style={{ color: 'var(--color-error)' }} onClick={() => deleteGroup(g.id || g.Id).then(onRefresh)}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function MemberSection({ groups, users, onToast }) {
  const [selectedGroup, setSelectedGroup] = useState('');
  const [members, setMembers] = useState([]);
  const [userId, setUserId] = useState('');
  const [role, setRole] = useState('Student');

  useEffect(() => {
    if (groups.length > 0 && !selectedGroup) setSelectedGroup(groups[0].id || groups[0].Id);
  }, [groups]);

  const loadMembers = async () => {
    if (!selectedGroup) return;
    try {
      const resp = await getGroupMembers(selectedGroup);
      setMembers(resp);
    } catch (err) { onToast('Failed to load members', true); }
  };

  useEffect(() => { loadMembers(); }, [selectedGroup]);

  return (
    <div className="flex flex-col gap-4">
      <div className="card">
        <h3 className="mb-4">Enroll User to Class</h3>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '16px' }}>
          <div>
            <label className="label">Classroom</label>
            <select className="input" value={selectedGroup} onChange={e => setSelectedGroup(e.target.value)}>
              {groups.map(g => <option key={g.id || g.Id} value={g.id || g.Id}>{g.name}</option>)}
            </select>
          </div>
          <div>
            <label className="label">User</label>
            <select className="input" value={userId} onChange={e => setUserId(e.target.value)}>
              <option value="">Choose User...</option>
              {users.map(u => <option key={u.id || u.Id} value={u.id || u.Id}>{u.firstName} {u.lastName}</option>)}
            </select>
          </div>
          <div>
            <label className="label">Enroll As</label>
            <select className="input" value={role} onChange={e => setRole(e.target.value)}>
              <option value="Student">Student</option>
              <option value="Teacher">Teacher</option>
            </select>
          </div>
        </div>
        <button className="btn btn-primary" onClick={async () => {
          if (!userId) return onToast('Select a user', true);
          try {
            await addGroupMember(selectedGroup, { userId, role });
            onToast('User enrolled');
            loadMembers();
          } catch (err) { onToast('Enrollment failed', true); }
        }}>Assign to Class</button>
      </div>

      <div className="card p-0" style={{ overflow: 'hidden' }}>
        <div style={{ padding: '24px' }}><h3>Class Roster</h3></div>
        <div className="data-table-wrapper">
          <table className="data-table">
            <thead>
              <tr>
                <th>Full Name</th>
                <th>Role in Class</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {members.map(m => (
                <tr key={m.userId || m.UserId}>
                  <td>{m.fullName}</td>
                  <td>{m.role}</td>
                  <td>
                    <button className="btn-text" style={{ color: 'var(--color-error)' }} onClick={() => removeGroupMember(selectedGroup, m.userId || m.UserId).then(loadMembers)}>Unenroll</button>
                  </td>
                </tr>
              ))}
              {members.length === 0 && <tr><td colSpan="3" style={{ textAlign: 'center', padding: '40px' }} className="text-secondary">No members in this classroom yet.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
 *cascade08*cascade08 *cascade08"*cascade08"( *cascade08()*cascade08)* *cascade08*,*cascade08,Ç *cascade08ÇÉ*cascade08ÉÖ *cascade08ÖÜ*cascade08Üá *cascade08áå*cascade08åç *cascade08çè*cascade08è† *cascade08†¢*cascade08¢£ *cascade08£§*cascade08§• *cascade08•©*cascade08©´ *cascade08´≠*cascade08≠æ *cascade08æø*cascade08ø„ *cascade08„‰*cascade08‰Î *cascade08ÎÏ*cascade08ÏÓ *cascade08Ó*cascade08Ò *cascade08ÒÚ*cascade08Ú˚ *cascade08˚˛*cascade08˛Ä *cascade08ÄÅ*cascade08Å´ *cascade08´Æ*cascade08Æπ *cascade08π∫*cascade08∫º *cascade08ºø*cascade08ø¿ *cascade08¿¡*cascade08¡¬ *cascade08¬√*cascade08√Œ *cascade08Œœ*cascade08œ– *cascade08–—*cascade08—Í *cascade08ÍÓ*cascade08Ó¸ *cascade08¸–/*cascade08–/›0 *cascade08›0‡0*cascade08‡0ı0 *cascade08ı0ﬂ=*cascade08ﬂ=Ï> *cascade08Ï>Ô>*cascade08Ô>Ñ? *cascade08Ñ?ÍZ*cascade08ÍZˆZ *cascade08"(51ef8254d65def5408cd0e035a74a409c8f05b042\file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front/src/pages/AdminDashboardPage.jsx:;file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front