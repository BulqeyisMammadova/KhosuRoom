import { http } from './http'

export function loginApi(payload) {
  return http('/Auth/Login', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function getMeApi() {
  return http('/me')
}
