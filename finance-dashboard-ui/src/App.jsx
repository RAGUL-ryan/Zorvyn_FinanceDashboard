import { Routes, Route, Navigate, Outlet } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import LoginPage       from './pages/LoginPage'
import DashboardPage   from './pages/DashboardPage'
import TransactionsPage from './pages/TransactionsPage'
import UsersPage       from './pages/UsersPage'
import NotFoundPage    from './pages/NotFoundPage'
import ProtectedRoute  from './components/layout/ProtectedRoute'
import Sidebar         from './components/layout/Sidebar'
import Navbar          from './components/layout/Navbar'
import LoadingSpinner  from './components/common/LoadingSpinner'

// Shell layout wrapping all protected pages
const AppShell = () => (
  <div className="flex h-screen overflow-hidden bg-gray-50">
    <Sidebar />
    <div className="flex-1 flex flex-col overflow-hidden ml-64">
      <Navbar />
      <main className="flex-1 overflow-y-auto p-6">
        <Outlet />
      </main>
    </div>
  </div>
)

const App = () => {
  const { loading } = useAuth()
  if (loading) return <LoadingSpinner fullScreen />

  return (
    <Routes>
      {/* Public */}
      <Route path="/login" element={<LoginPage />} />

      {/* Protected shell */}
      <Route element={<ProtectedRoute><AppShell /></ProtectedRoute>}>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard"    element={<DashboardPage />} />
        <Route path="/transactions" element={<TransactionsPage />} />
        <Route
          path="/users"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <UsersPage />
            </ProtectedRoute>
          }
        />
      </Route>

      {/* Catch-all */}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}

export default App
