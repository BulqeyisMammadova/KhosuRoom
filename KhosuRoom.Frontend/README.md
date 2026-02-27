# KhosuRoom Frontend (React + Vanilla JS)

Bu qovluq backend üçün başlanğıc frontend-dir.

## Quraşdırma

```bash
cd KhosuRoom.Frontend
npm install
npm run dev
```

Frontend default olaraq bu backend URL-ə sorğu göndərir:

- `http://localhost:5079/api`

Dəyişmək üçün `src/config.js` faylını edit et.

## Hazır olan hissələr

- Login flow (`/api/Auth/Login`)
- Current user fetch (`/api/me`)
- Protected route
- Groups list (`/api/Groups`)
- Group members (`/api/GroupMembers/{groupId}/members`)

## Növbəti addımlar

- Assignment/Submissions səhifələri
- Attendance səhifələri
- Chat (SignalR)
- Notifications
