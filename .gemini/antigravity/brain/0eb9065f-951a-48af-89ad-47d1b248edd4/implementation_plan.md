# Frontend UI Rewrite Plan (Google Classroom Style)

## Goal Description
The user wants to completely rewrite the frontend UI of the KhosuRoom application to closely resemble **Google Classroom**. The new UI must be highly aesthetic, modern, and optimized for mobile. Backend remains **untouched**.

## Proposed Changes

### 1. Design System (Material 3)
Update `index.css` to implement a premium, mobile-first Google Material Design 3 aesthetic.
- Edge-to-edge layouts (removing restrictive containers).
- Bottom navigation for mobile devices.
- Google Sans & Roboto typography.

### 2. Layouts & Navigation
- **MainLayout**: Top Header + Sidebar (for desktop) + Bottom Nav (for mobile).
- **GroupLayout**: Immersive banner + sticky tabs (Stream, Classwork, People, Grades).

### 3. Page Refactoring
- **Home (GroupsListPage)**: Premium class cards with banner images and teacher avatars.
- **Classroom Tabs**: Break down monolithic logic into `StreamTab`, `ClassworkTab`, `PeopleTab`, and `GradesTab`.
- **Admin Dashboard (Overhaul)**: Rewrite `AdminDashboardPage.jsx` to use `MainLayout`. Standardize management tables and forms to match the new aesthetic.

## Verification Plan
1. Test all classroom tabs on mobile and desktop.
2. Verify Admin features (User/Group management) follow the new design system.
3. Ensure SignalR chat works in the new `Stream` tab.
