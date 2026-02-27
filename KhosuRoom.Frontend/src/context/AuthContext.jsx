import { createContext, useEffect, useMemo, useState } from 'react'
import { getMeApi, loginApi } from '../api/authApi'

export const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('accessToken')
    if (!token) {
      setLoading(false)
      return
    }

    getMeApi()
      .then((res) => {
        setUser(res?.data || null)
      })
      .catch(() => {
        logout()
      })
      .finally(() => setLoading(false))
  }, [])

  async function login(email, password) {
    const res = await loginApi({ email, password })
    const tokenData = res?.data

    if (!tokenData?.token) {
      throw new Error('Token tapılmadı')
    }

    localStorage.setItem('accessToken', tokenData.token)
    localStorage.setItem('refreshToken', tokenData.refreshToken || '')

    const me = await getMeApi()
    setUser(me?.data || null)
  }

  function logout() {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    setUser(null)
  }

  const value = useMemo(
    () => ({ user, loading, login, logout, isAuthenticated: !!user }),
    [user, loading]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
