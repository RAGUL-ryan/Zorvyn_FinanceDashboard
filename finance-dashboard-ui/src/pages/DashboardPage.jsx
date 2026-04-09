import { useState, useEffect, useCallback } from 'react'
import { DollarSign, TrendingUp, TrendingDown, Activity, RefreshCw } from 'lucide-react'
import { dashboardApi } from '../api/dashboardApi'
import SummaryCard from '../components/dashboard/SummaryCard'
import MonthlyTrendChart from '../components/dashboard/MonthlyTrendChart'
import CategoryPieChart from '../components/dashboard/CategoryPieChart'
import LoadingSpinner from '../components/common/LoadingSpinner'
import ErrorMessage from '../components/common/ErrorMessage'
import { formatCurrency, formatDate, getTypeBadgeClass } from '../utils/formatters'

const DashboardPage = () => {
  const [summary,    setSummary]    = useState(null)
  const [trends,     setTrends]     = useState([])
  const [categories, setCategories] = useState([])
  const [loading,    setLoading]    = useState(true)
  const [error,      setError]      = useState(null)
  const [months,     setMonths]     = useState(6)

  const fetchAll = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const [summaryRes, trendsRes, catRes] = await Promise.all([
        dashboardApi.getSummary(),
        dashboardApi.getTrends(months),
        dashboardApi.getCategories(),
      ])
      setSummary(summaryRes.data.data)
      setTrends(trendsRes.data.data)
      setCategories(catRes.data.data)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load dashboard data')
    } finally {
      setLoading(false)
    }
  }, [months])

  useEffect(() => { fetchAll() }, [fetchAll])

  if (loading) return <LoadingSpinner />
  if (error)   return <ErrorMessage message={error} onRetry={fetchAll} />

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-gray-800">Financial Overview</h2>
          <p className="text-sm text-gray-400 mt-0.5">All-time summary of your finances</p>
        </div>
        <button onClick={fetchAll} className="btn-secondary text-sm">
          <RefreshCw size={14} /> Refresh
        </button>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        <SummaryCard
          title="Total Income"
          value={formatCurrency(summary?.totalIncome ?? 0)}
          icon={TrendingUp}
          type="income"
          subtitle="All-time earnings"
        />
        <SummaryCard
          title="Total Expenses"
          value={formatCurrency(summary?.totalExpenses ?? 0)}
          icon={TrendingDown}
          type="expense"
          subtitle="All-time spending"
        />
        <SummaryCard
          title="Net Balance"
          value={formatCurrency(summary?.netBalance ?? 0)}
          icon={DollarSign}
          type="balance"
          subtitle={summary?.netBalance >= 0 ? 'Positive balance' : 'Negative balance'}
        />
        <SummaryCard
          title="Transactions"
          value={summary?.totalTransactions ?? 0}
          icon={Activity}
          type="default"
          subtitle="Total records"
        />
      </div>

      {/* Charts row */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
        {/* Monthly trends */}
        <div className="xl:col-span-2 card p-6">
          <div className="flex items-center justify-between mb-5">
            <div>
              <h3 className="font-semibold text-gray-800">Monthly Trends</h3>
              <p className="text-xs text-gray-400 mt-0.5">Income vs Expenses</p>
            </div>
            <select
              value={months}
              onChange={(e) => setMonths(Number(e.target.value))}
              className="input-field w-auto text-xs py-1.5 px-2"
            >
              <option value={3}>Last 3 months</option>
              <option value={6}>Last 6 months</option>
              <option value={12}>Last 12 months</option>
            </select>
          </div>
          <MonthlyTrendChart data={trends} />
        </div>

        {/* Category breakdown */}
        <div className="card p-6">
          <div className="mb-5">
            <h3 className="font-semibold text-gray-800">By Category</h3>
            <p className="text-xs text-gray-400 mt-0.5">Spending distribution</p>
          </div>
          <CategoryPieChart data={categories} />
        </div>
      </div>

      {/* Recent activity */}
      <div className="card p-6">
        <div className="flex items-center justify-between mb-5">
          <div>
            <h3 className="font-semibold text-gray-800">Recent Activity</h3>
            <p className="text-xs text-gray-400 mt-0.5">Latest 10 transactions</p>
          </div>
        </div>

        {summary?.recentActivity?.length ? (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-100">
                  <th className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide py-2 pr-4">Description</th>
                  <th className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide py-2 pr-4">Category</th>
                  <th className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide py-2 pr-4">Date</th>
                  <th className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide py-2 pr-4">Type</th>
                  <th className="text-right text-xs font-medium text-gray-400 uppercase tracking-wide py-2">Amount</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {summary.recentActivity.map((t) => (
                  <tr key={t.id} className="hover:bg-gray-50/50 transition-colors">
                    <td className="py-3 pr-4">
                      <p className="font-medium text-gray-700 truncate max-w-[180px]">{t.description}</p>
                    </td>
                    <td className="py-3 pr-4 text-gray-500">{t.category}</td>
                    <td className="py-3 pr-4 text-gray-400">{formatDate(t.date)}</td>
                    <td className="py-3 pr-4">
                      <span className={getTypeBadgeClass(t.type)}>{t.type}</span>
                    </td>
                    <td className={`py-3 text-right font-semibold ${t.type?.toLowerCase() === 'income' ? 'text-success-600' : 'text-danger-600'}`}>
                      {t.type?.toLowerCase() === 'income' ? '+' : '-'}{formatCurrency(t.amount)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p className="text-center text-gray-400 text-sm py-8">No recent activity</p>
        )}
      </div>
    </div>
  )
}

export default DashboardPage
