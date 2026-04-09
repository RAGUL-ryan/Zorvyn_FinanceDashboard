import { NavLink } from 'react-router-dom'
import { LayoutDashboard, ArrowLeftRight, Users, LogOut, TrendingUp } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'

const NAV = [
  { to: '/dashboard',    label: 'Dashboard',     icon: LayoutDashboard, roles: ['Admin','Analyst','Viewer'] },
  { to: '/transactions', label: 'Transactions',  icon: ArrowLeftRight,  roles: ['Admin','Analyst','Viewer'] },
  { to: '/users',        label: 'Users',         icon: Users,           roles: ['Admin'] },
]

const Sidebar = () => {
  const { user, logout, isAdmin } = useAuth()

  return (
    <aside className="fixed left-0 top-0 h-full w-64 bg-gray-900 text-white flex flex-col z-30">
      {/* Logo */}
      <div className="p-6 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 bg-primary-600 rounded-lg flex items-center justify-center">
            <TrendingUp size={20} />
          </div>
          <div>
            <p className="font-bold text-sm leading-tight">Finance</p>
            <p className="text-gray-400 text-xs">Dashboard</p>
          </div>
        </div>
      </div>

      {/* Nav links */}
      <nav className="flex-1 p-4 space-y-1">
        {NAV.filter(n => n.roles.includes(user?.role)).map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-150
               ${isActive
                 ? 'bg-primary-600 text-white'
                 : 'text-gray-400 hover:bg-gray-800 hover:text-white'}`
            }
          >
            <Icon size={18} />
            {label}
          </NavLink>
        ))}
      </nav>

      {/* User info + logout */}
      <div className="p-4 border-t border-gray-700">
        <div className="flex items-center gap-3 mb-3">
          <div className="w-8 h-8 rounded-full bg-primary-600 flex items-center justify-center text-xs font-bold">
            {user?.fullName?.[0] ?? 'U'}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium truncate">{user?.fullName}</p>
            <p className="text-xs text-gray-400">{user?.role}</p>
          </div>
        </div>
        <button onClick={logout} className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-400 hover:text-white hover:bg-gray-800 rounded-lg transition-all">
          <LogOut size={16} />
          Sign out
        </button>
      </div>
    </aside>
  )
}

export default Sidebar
