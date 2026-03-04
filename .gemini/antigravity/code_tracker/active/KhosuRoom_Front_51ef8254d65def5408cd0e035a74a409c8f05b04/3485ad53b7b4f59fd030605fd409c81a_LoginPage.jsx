¿import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Input } from '../components/common/Input';
import Button from '../components/common/Button';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const result = await login(email, password);

      if (result.mustChangePassword) {
        navigate('/change-password', { replace: true });
        return;
      }

      // Precise Role-Based Redirection
      const userRole = result.role || result.Role;

      switch (userRole) {
        case 'Admin':
          navigate('/admin', { replace: true });
          break;
        case 'Teacher':
          navigate('/groups', { replace: true });
          break;
        case 'Student':
          navigate('/groups', { replace: true });
          break;
        default:
          navigate('/groups', { replace: true });
      }

    } catch (err) {
      setError(err.message || 'Verification failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '100vh',
      backgroundColor: 'var(--color-surface)',
      padding: '24px'
    }}>
      <div className="card" style={{
        width: '100%',
        maxWidth: '450px',
        padding: '48px 40px 36px',
        border: '1px solid var(--color-border)',
        borderRadius: '8px',
        textAlign: 'center'
      }}>
        <div style={{ display: 'flex', justifyContent: 'center', marginBottom: '16px' }}>
          <span className="material-icons-outlined" style={{ fontSize: '48px', color: 'var(--color-primary)' }}>school</span>
        </div>

        <h1 style={{ fontSize: '24px', marginBottom: '8px', fontWeight: 500 }}>Sign in</h1>
        <p className="text-muted" style={{ marginBottom: '32px' }}>Access your KhosuRoom Workspace</p>

        <form onSubmit={handleSubmit} style={{ textAlign: 'left' }}>
          <Input
            label="Email or Username"
            type="text"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="username"
          />

          <Input
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="current-password"
          />

          {error && <div className="text-danger mb-4" style={{ fontSize: '14px', background: 'rgba(217,48,37,0.1)', padding: '8px', borderRadius: '4px' }}>{error}</div>}

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '40px' }}>
            <button className="btn-text" type="button">
              Help?
            </button>
            <Button variant="primary" type="submit" disabled={loading}>
              {loading ? 'Processing...' : 'Sign in'}
            </Button>
          </div>
        </form>
      </div>

      <div style={{
        width: '100%',
        maxWidth: '450px',
        display: 'flex',
        justifyContent: 'space-between',
        marginTop: '24px',
        fontSize: '12px',
        color: 'var(--color-text-secondary)'
      }}>
        <div>System v2.5.0</div>
        <div style={{ display: 'flex', gap: '24px' }}>
          <span>Privacy</span>
          <span>Terms</span>
        </div>
      </div>
    </div>
  );
}
ô *cascade08ô– *cascade08–ÿ*cascade08ÿ› *cascade08›ﬁ*cascade08ﬁ„ *cascade08„‰*cascade08‰ˆ *cascade08ˆï*cascade08ïù *cascade08ù∑*cascade08∑∏ *cascade08∏π*cascade08πº *cascade08ºΩ*cascade08Ωæ *cascade08æÕ*cascade08Õ’ *cascade08’÷*cascade08÷ÿ *cascade08ÿ⁄*cascade08⁄ê *cascade08ê„*cascade08„‰ *cascade08‰*cascade08Ò *cascade08ÒÙ*cascade08Ùı *cascade08ıÑ	*cascade08Ñ	á	 *cascade08á	±	*cascade08±	≤	 *cascade08≤	ƒ	*cascade08ƒ	∆	 *cascade08∆	Ï	*cascade08Ï	√
 *cascade08√
‹
 *cascade08‹
Â
*cascade08Â
Ê
 *cascade08Ê
Á
*cascade08Á
Ë *cascade08ËÏ*cascade08Ï© *cascade08©¡*cascade08¡˝ *cascade08˝õ*cascade08õı *cascade08ıÖ*cascade08ÖÜ *cascade08Üâ*cascade08â˜ *cascade08˜¯*cascade08¯ó *cascade08óÆ*cascade08Æœ *cascade08œ‘*cascade08‘Â *cascade08ÂÊ*cascade08ÊÁ *cascade08ÁÓ*cascade08ÓÍ *cascade08ÍÓ*cascade08ÓÔ *cascade08ÔÒ*cascade08Òá *cascade08áà*cascade08àâ *cascade08âã*cascade08ã— *cascade08—“*cascade08“Ù *cascade08Ùº*cascade08ºê *cascade08êë*cascade08ëì *cascade08ìî*cascade08îë *cascade08ëò*cascade08ò£ *cascade08£™*cascade08™Ã *cascade08Ã„*cascade08„‰ *cascade08‰Á*cascade08ÁÇ *cascade08ÇÑ*cascade08Ñá *cascade08áà*cascade08àâ *cascade08âè*cascade08è¿ *cascade08"(51ef8254d65def5408cd0e035a74a409c8f05b042Sfile:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front/src/pages/LoginPage.jsx:;file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front