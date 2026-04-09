import { useLocation } from 'react-router-dom'
import { Bell } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'

const TITLES = {
  '/dashboard':    'Dashboard',
  '/transactions': 'Transactions',
  '/users':        'User Management',
}

const Navbar = () => {
  const { pathname } = useLocation()
  const { user }     = useAuth()
  const title        = TITLES[pathname] ?? 'Finance Dashboard'

  return (
    <header className="h-16 bg-white border-b border-gray-100 flex items-center justify-between px-6 sticky top-0 z-20">
      <h1 className="text-lg font-semibold text-gray-800">{title}</h1>
      <div className="flex items-center gap-3">
        <button className="w-9 h-9 rounded-lg hover:bg-gray-100 flex items-center justify-center text-gray-500 transition-colors relative">
          <Bell size={18} />
        </button>
        <div className="flex items-center gap-2.5">
          <div className="w-8 h-8 rounded-full bg-primary-600 flex items-center justify-center text-white text-xs font-bold">
            {user?.fullName?.[0] ?? 'U'}
          </div>
          <div className="hidden md:block">
            <p className="text-sm font-medium text-gray-800 leading-tight">{user?.fullName}</p>
            <p className="text-xs text-gray-400">{user?.email}</p>
          </div>
        </div>
      </div>
    </header>
  )
}

export default Navbar
