import { useAuth } from '../hooks/useAuth'

export default function HomePage() {
  const { user } = useAuth()

  return (
    <section>
      <h2>Salam, {user?.firstName || user?.userName || 'istifadəçi'} 👋</h2>
      <p className="muted">Frontend başlanğıc versiyası hazırdır.</p>
      <ul>
        <li>Auth + protected routes</li>
        <li>Groups list API bağlantısı</li>
        <li>Sadə layout və navigation</li>
      </ul>
    </section>
  )
}
