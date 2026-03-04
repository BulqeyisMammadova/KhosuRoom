 import { apiFetch, unwrapResult } from './http';

export async function loginRequest(email, password) {
  const res = await apiFetch('/Auth/Login', {
    method: 'POST',
    body: JSON.stringify({ email, password })
  });
  return unwrapResult(res);
}

export async function getMeRequest() {
  const res = await apiFetch('/me', { method: 'GET' });
  return unwrapResult(res);
}

export async function changePasswordRequest(currentPassword, newPassword) {
  const res = await apiFetch('/me/change-password', {
    method: 'POST',
    body: JSON.stringify({ currentPassword, newPassword })
  });
  return unwrapResult(res);
}



export async function changePassword(currentPassword, newPassword) {
  return changePasswordRequest(currentPassword, newPassword);
}


export async function updateProfileImageRequest(file) {
  const form = new FormData();
  form.append('ProfileImageUrl', file);

  const res = await apiFetch('/me/profile-image', {
    method: 'PATCH',
    body: form
  });

  return unwrapResult(res);
}
 "(51ef8254d65def5408cd0e035a74a409c8f05b042Nfile:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front/src/api/authApi.js:;file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front