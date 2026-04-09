import { TrendingUp, TrendingDown } from 'lucide-react'

const SummaryCard = ({ title, value, icon: Icon, type = 'default', subtitle }) => {
  const styles = {
    income:  { wrapper: 'border-success-500/20 bg-success-50/50', icon: 'bg-success-100 text-success-600', value: 'text-success-700' },
    expense: { wrapper: 'border-danger-500/20 bg-danger-50/50',   icon: 'bg-danger-100 text-danger-600',   value: 'text-danger-700'  },
    balance: { wrapper: 'border-primary-500/20 bg-primary-50/50', icon: 'bg-primary-100 text-primary-600', value: 'text-primary-700' },
    default: { wrapper: 'border-gray-100 bg-white',               icon: 'bg-gray-100 text-gray-600',       value: 'text-gray-800'    },
  }

  const s = styles[type] ?? styles.default

  return (
    <div className={`card p-5 border ${s.wrapper} transition-all hover:shadow-md`}>
      <div className="flex items-start justify-between mb-4">
        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${s.icon}`}>
          <Icon size={20} />
        </div>
        {type === 'income'  && <TrendingUp  size={16} className="text-success-500 mt-1" />}
        {type === 'expense' && <TrendingDown size={16} className="text-danger-500 mt-1"  />}
      </div>
      <p className="text-xs font-medium text-gray-500 uppercase tracking-wide mb-1">{title}</p>
      <p className={`text-2xl font-bold ${s.value}`}>{value}</p>
      {subtitle && <p className="text-xs text-gray-400 mt-1">{subtitle}</p>}
    </div>
  )
}

export default SummaryCard
