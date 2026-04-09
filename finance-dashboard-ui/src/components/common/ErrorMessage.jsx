import { AlertCircle, RefreshCw } from 'lucide-react'

const ErrorMessage = ({ message = 'Something went wrong', onRetry }) => (
  <div className="flex flex-col items-center justify-center p-10 gap-3 text-center">
    <div className="w-12 h-12 rounded-full bg-danger-50 flex items-center justify-center">
      <AlertCircle size={24} className="text-danger-500" />
    </div>
    <p className="text-gray-600 text-sm max-w-xs">{message}</p>
    {onRetry && (
      <button onClick={onRetry} className="btn-secondary text-sm mt-1">
        <RefreshCw size={14} /> Try again
      </button>
    )}
  </div>
)

export default ErrorMessage
