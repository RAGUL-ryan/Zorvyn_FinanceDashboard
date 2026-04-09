import { useNavigate } from 'react-router-dom'
import { Home, AlertTriangle } from 'lucide-react'

const NotFoundPage = () => {
  const navigate = useNavigate()
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
      <div className="text-center">
        <div className="w-20 h-20 bg-warning-50 rounded-full flex items-center justify-center mx-auto mb-6">
          <AlertTriangle size={40} className="text-warning-500" />
        </div>
        <h1 className="text-6xl font-bold text-gray-800 mb-2">404</h1>
        <p className="text-xl font-medium text-gray-600 mb-2">Page not found</p>
        <p className="text-gray-400 mb-8">The page you're looking for doesn't exist.</p>
        <button onClick={() => navigate('/dashboard')} className="btn-primary mx-auto">
          <Home size={16} /> Go to Dashboard
        </button>
      </div>
    </div>
  )
}

export default NotFoundPage
