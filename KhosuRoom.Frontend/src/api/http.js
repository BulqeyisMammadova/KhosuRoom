import { API_BASE_URL } from '../config'

export async function http(path, options = {}) {
  const token = localStorage.getItem('accessToken')

  const headers = {
    ...(options.body instanceof FormData ? {} : { 'Content-Type': 'application/json' }),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...(options.headers || {})
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  })

  const data = await response.json().catch(() => null)

  if (!response.ok) {
    const message = data?.message || `HTTP ${response.status}`
    throw new Error(message)
  }

  return data
}
