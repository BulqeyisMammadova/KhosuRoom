ûimport { Navigate, Route, Routes } from 'react-router-dom';
import NotFoundPage from './pages/NotFoundPage';
import LoginPage from './pages/LoginPage';
import ChangePasswordPage from './pages/ChangePasswordPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import GroupsListPage from './pages/GroupsListPage';
import GroupLayout from './pages/GroupLayout';
import StreamTab from './pages/teacher/StreamTab';
import ClassworkTab from './pages/teacher/ClassworkTab';
import PeopleTab from './pages/teacher/PeopleTab';
import GradesTab from './pages/teacher/GradesTab';
import CalendarPage from './pages/CalendarPage';
import SettingsPage from './pages/SettingsPage';
import ToReviewPage from './pages/ToReviewPage';
import ToDoPage from './pages/ToDoPage';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './components/layout/MainLayout';
import { useAuth } from './context/AuthContext';

export default function App() {
  const { role } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        path="/change-password"
        element={
          <ProtectedRoute allowWhenMustChange>
            <ChangePasswordPage />
          </ProtectedRoute>
        }
      />

      {/* Main App Layout */}
      <Route element={<ProtectedRoute><MainLayout /></ProtectedRoute>}>
        <Route path="/" element={
          role === 'Admin' ? <Navigate to="/admin" replace /> : <Navigate to="/groups" replace />
        } />

        <Route path="/groups" element={<GroupsListPage />} />

        {/* Single Class Routes */}
        <Route path="/class/:id" element={<GroupLayout />}>
          <Route index element={<Navigate to="stream" replace />} />
          <Route path="stream" element={<StreamTab />} />
          <Route path="classwork" element={<ClassworkTab />} />
          <Route path="people" element={<PeopleTab />} />

          {/* Grades only for Teacher & Admin (Maybe Student can see their own, but typically a summary) */}
          <Route
            path="grades"
            element={
              role === 'Teacher' || role === 'Admin' ? <GradesTab /> : <Navigate to="stream" replace />
            }
          />
        </Route>

        <Route path="/calendar" element={<CalendarPage />} />
        <Route path="/settings" element={<SettingsPage />} />

        {/* Role-Specific Secondary Pages */}
        <Route
          path="/to-review"
          element={role === 'Teacher' ? <ToReviewPage /> : <Navigate to="/groups" replace />}
        />
        <Route
          path="/to-do"
          element={role === 'Student' ? <ToDoPage /> : <Navigate to="/groups" replace />}
        />
      </Route>

      <Route
        path="/admin"
        element={
          <ProtectedRoute roles={["Admin"]}>
            <AdminDashboardPage />
          </ProtectedRoute>
        }
      />

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
Ô *cascade08ÔÚ*cascade08Úš*cascade08šˆ *cascade08ˆ‹*cascade08‹Œ *cascade08Œ*cascade08‘ *cascade08‘“*cascade08“” *cascade08”˜*cascade08˜› *cascade08›*cascade08  *cascade08 £*cascade08£¤ *cascade08¤¥*cascade08¥¦ *cascade08¦§*cascade08§¨ *cascade08¨ª*cascade08ª« *cascade08«®*cascade08®² *cascade08²·*cascade08·Û *cascade08Ûü*cascade08ü¤ *cascade08¤æ*cascade08æ‡ *cascade08‡‘*cascade08‘• *cascade08•—*cascade08—Ÿ *cascade08Ÿ¹*cascade08¹± *cascade08±´*cascade08´ò *cascade08òõ*cascade08õ­ *cascade08­°*cascade08°¹ *cascade08¹©*cascade08©¹ *cascade08¹Æ*cascade08ÆÔ *cascade08Ôá*cascade08áë *cascade08ë¤*cascade08¤« *cascade08«®*cascade08®± *cascade08±â*cascade08âã *cascade08ãî*cascade08î¹ *cascade08¹½*cascade08½ğ *cascade08ğø*cascade08øú *cascade08úû*cascade08ûü *cascade08ü… *cascade08…¶*cascade08¶À *cascade08ÀÁ *cascade08ÁÃ*cascade08ÃÄ *cascade08ÄÏ*cascade08ÏĞ *cascade08ĞÑ*cascade08ÑÒ *cascade08Òá *cascade08áì*cascade08ìğ *cascade08ğò *cascade08òô*cascade08ôõ *cascade08õö *cascade08ö‹*cascade08‹’ *cascade08’“ *cascade08“› *cascade08›¿*cascade08¿À *cascade08ÀÉ*cascade08ÉÚ *cascade08ÚÛ *cascade08ÛÜ *cascade08Üç*cascade08çê *cascade08êë *cascade08ëõ *cascade08õ€*cascade08€† *cascade08†‡ *cascade08‡Š *cascade08ŠŸ*cascade08Ÿ¦ *cascade08¦§ *cascade08§¨*cascade08¨« *cascade08«Ï*cascade08ÏĞ *cascade08ĞÙ*cascade08Ùù *cascade08ùû*cascade08"(51ef8254d65def5408cd0e035a74a409c8f05b042Gfile:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front/src/App.jsx:;file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front