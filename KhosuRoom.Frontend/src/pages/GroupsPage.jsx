import { useEffect, useState } from 'react'
import { getGroupsApi, getGroupMembersApi } from '../api/groupsApi'

export default function GroupsPage() {
  const [groups, setGroups] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [selectedGroupId, setSelectedGroupId] = useState('')
  const [members, setMembers] = useState([])

  useEffect(() => {
    getGroupsApi()
      .then((res) => {
        setGroups(res?.data || [])
      })
      .catch((err) => {
        setError(err.message || 'Qruplar yüklənmədi')
      })
      .finally(() => setLoading(false))
  }, [])

  async function loadMembers(groupId) {
    setSelectedGroupId(groupId)
    setMembers([])
    try {
      const res = await getGroupMembersApi(groupId)
      setMembers(res?.data || [])
    } catch (err) {
      setError(err.message || 'Member-lər yüklənmədi')
    }
  }

  if (loading) return <div>Qruplar yüklənir...</div>

  return (
    <section>
      <h2>Qruplar</h2>
      {error && <div className="error">{error}</div>}

      <div className="grid two">
        <div className="card">
          <h3>Mənim qruplarım</h3>
          {groups.length === 0 ? (
            <p className="muted">Qrup yoxdur.</p>
          ) : (
            <ul className="list">
              {groups.map((group) => (
                <li key={group.id}>
                  <button
                    className={`link-btn ${selectedGroupId === group.id ? 'active' : ''}`}
                    onClick={() => loadMembers(group.id)}
                  >
                    {group.name} <span className="muted">({group.code})</span>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="card">
          <h3>Üzvlər</h3>
          {!selectedGroupId ? (
            <p className="muted">Üzvləri görmək üçün qrup seç.</p>
          ) : members.length === 0 ? (
            <p className="muted">Üzv yoxdur və ya yüklənir.</p>
          ) : (
            <ul className="list">
              {members.map((m) => (
                <li key={m.userId || m.id}>
                  {m.fullName || m.userName || m.email} - <b>{m.role}</b>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </section>
  )
}
