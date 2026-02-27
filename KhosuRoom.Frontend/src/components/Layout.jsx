import { Link, NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'

export default function Layout() {
  const { user, logout } = useAuth()

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link className="brand" to="/">KhosuRoom</Link>

        <nav className="nav-links">
          <NavLink to="/">Ana səhifə</NavLink>
          <NavLink to="/groups">Qruplar</NavLink>
        </nav>

        <div className="user-box">
          <span>{user?.fullName || user?.userName || 'User'}</span>
          <button onClick={logout}>Çıxış</button>
        </div>
      </header>

      <main className="content">
        <Outlet />
      </main>
    </div>
  )
}
