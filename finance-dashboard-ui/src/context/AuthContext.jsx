import { createContext, useContext, useState, useEffect, useCallback } from 'react'
import { authApi } from '../api/authApi'
import toast from 'react-hot-toast'

const AuthContext = createContext(null)

export const AuthProvider = ({ children }) => {
  const [user, setUser]       = useState(null)
  const [loading, setLoading] = useState(true)

  // Rehydrate from localStorage on mount
  useEffect(() => {
    const stored = localStorage.getItem('user')
    const token  = localStorage.getItem('accessToken')
    if (stored && token) {
      try { setUser(JSON.parse(stored)) } catch { localStorage.clear() }
    }
    setLoading(false)
  }, [])

  const login = useCallback(async (email, password) => {
    const res  = await authApi.login(email, password)
    const data = res.data.data
    localStorage.setItem('accessToken',  data.accessToken)
    localStorage.setItem('refreshToken', data.refreshToken)
    localStorage.setItem('user',         JSON.stringify(data.user))
    setUser(data.user)
    return data.user
  }, [])

  const logout = useCallback(async () => {
    try { await authApi.logout() } catch {}
    localStorage.clear()
    setUser(null)
    toast.success('Logged out successfully')
  }, [])

  const isAdmin   = user?.role === 'Admin'
  const isAnalyst = user?.role === 'Analyst'
  const isViewer  = user?.role === 'Viewer'
  const canWrite  = isAdmin || isAnalyst
  const canDelete = isAdmin

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, isAdmin, isAnalyst, isViewer, canWrite, canDelete }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
