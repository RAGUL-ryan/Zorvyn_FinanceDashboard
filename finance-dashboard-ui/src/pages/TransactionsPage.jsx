import { useState, useEffect, useCallback } from 'react'
import { Plus, Search, Filter, Edit2, Trash2, X, ChevronDown } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { useTransactions } from '../hooks/useTransactions'
import Pagination from '../components/common/Pagination'
import LoadingSpinner from '../components/common/LoadingSpinner'
import ConfirmModal from '../components/common/ConfirmModal'
import TransactionModal from '../components/transactions/TransactionModal'
import { formatCurrency, formatDate, getTypeBadgeClass, CATEGORIES, TRANSACTION_TYPES } from '../utils/formatters'
import toast from 'react-hot-toast'

const TransactionsPage = () => {
  const { canWrite, canDelete } = useAuth()
  const {
    transactions, pagination, loading, filters, setFilters,
    fetchTransactions, createTransaction, updateTransaction, deleteTransaction,
  } = useTransactions()

  const [showModal,   setShowModal]   = useState(false)
  const [editItem,    setEditItem]    = useState(null)
  const [deleteId,    setDeleteId]    = useState(null)
  const [deleting,    setDeleting]    = useState(false)
  const [showFilters, setShowFilters] = useState(false)
  const [localSearch, setLocalSearch] = useState('')

  useEffect(() => { fetchTransactions({ page: 1 }) }, [])

  const handleSearch = (e) => {
    e.preventDefault()
    setFilters(f => ({ ...f, search: localSearch }))
    fetchTransactions({ page: 1, search: localSearch })
  }

  const handleFilterChange = useCallback((key, value) => {
    const updated = { ...filters, [key]: value }
    setFilters(updated)
    fetchTransactions({ page: 1, ...updated })
  }, [filters, fetchTransactions, setFilters])

  const clearFilters = () => {
    setFilters({ type: '', category: '', search: '', from: '', to: '' })
    setLocalSearch('')
    fetchTransactions({ page: 1, type: '', category: '', search: '', from: '', to: '' })
  }

  const handleSave = async (data) => {
    try {
      if (editItem) {
        await updateTransaction(editItem.id, data)
      } else {
        await createTransaction(data)
      }
      setShowModal(false)
      setEditItem(null)
      fetchTransactions({ page: 1 })
    } catch (err) {
      toast.error(err.response?.data?.message || 'Operation failed')
    }
  }

  const handleDelete = async () => {
    setDeleting(true)
    try {
      await deleteTransaction(deleteId)
      setDeleteId(null)
      fetchTransactions({ page: pagination.page })
    } catch (err) {
      toast.error(err.response?.data?.message || 'Delete failed')
    } finally {
      setDeleting(false)
    }
  }

  const hasActiveFilters = filters.type || filters.category || filters.from || filters.to || filters.search

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-gray-800">Transactions</h2>
          <p className="text-sm text-gray-400 mt-0.5">{pagination.totalCount} total records</p>
        </div>
        {canWrite && (
          <button onClick={() => { setEditItem(null); setShowModal(true) }} className="btn-primary">
            <Plus size={16} /> Add Transaction
          </button>
        )}
      </div>

      {/* Search + filter bar */}
      <div className="card p-4 space-y-3">
        <div className="flex gap-3">
          <form onSubmit={handleSearch} className="flex-1 flex gap-2">
            <div className="relative flex-1">
              <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
              <input
                value={localSearch}
                onChange={(e) => setLocalSearch(e.target.value)}
                placeholder="Search description, category, notes..."
                className="input-field pl-9 text-sm"
              />
            </div>
            <button type="submit" className="btn-secondary text-sm">Search</button>
          </form>
          <button
            onClick={() => setShowFilters(v => !v)}
            className={`btn-secondary text-sm ${hasActiveFilters ? 'border-primary-400 text-primary-600' : ''}`}
          >
            <Filter size={14} />
            Filters
            {hasActiveFilters && <span className="w-2 h-2 bg-primary-500 rounded-full" />}
            <ChevronDown size={14} className={`transition-transform ${showFilters ? 'rotate-180' : ''}`} />
          </button>
          {hasActiveFilters && (
            <button onClick={clearFilters} className="btn-secondary text-sm text-danger-600 border-danger-200 hover:bg-danger-50">
              <X size={14} /> Clear
            </button>
          )}
        </div>

        {/* Expandable filters */}
        {showFilters && (
          <div className="grid grid-cols-2 md:grid-cols-4 gap-3 pt-2 border-t border-gray-100">
            <div>
              <label className="block text-xs font-medium text-gray-500 mb-1">Type</label>
              <select value={filters.type} onChange={(e) => handleFilterChange('type', e.target.value)} className="input-field text-sm">
                <option value="">All types</option>
                {TRANSACTION_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-500 mb-1">Category</label>
              <select value={filters.category} onChange={(e) => handleFilterChange('category', e.target.value)} className="input-field text-sm">
                <option value="">All categories</option>
                {CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-500 mb-1">From date</label>
              <input type="date" value={filters.from} onChange={(e) => handleFilterChange('from', e.target.value)} className="input-field text-sm" />
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-500 mb-1">To date</label>
              <input type="date" value={filters.to} onChange={(e) => handleFilterChange('to', e.target.value)} className="input-field text-sm" />
            </div>
          </div>
        )}
      </div>

      {/* Table */}
      <div className="card overflow-hidden">
        {loading ? (
          <LoadingSpinner />
        ) : transactions.length === 0 ? (
          <div className="text-center py-16 text-gray-400">
            <p className="text-lg mb-1">No transactions found</p>
            <p className="text-sm">Try adjusting your filters or add a new transaction</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 border-b border-gray-100">
                <tr>
                  {['Description','Category','Type','Amount','Date','Created By','Actions'].map(h => (
                    <th key={h} className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide px-4 py-3">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {transactions.map((t) => (
                  <tr key={t.id} className="hover:bg-gray-50/50 transition-colors">
                    <td className="px-4 py-3">
                      <p className="font-medium text-gray-800 truncate max-w-[180px]">{t.description}</p>
                      {t.notes && <p className="text-xs text-gray-400 truncate max-w-[180px] mt-0.5">{t.notes}</p>}
                    </td>
                    <td className="px-4 py-3 text-gray-500">{t.category}</td>
                    <td className="px-4 py-3">
                      <span className={getTypeBadgeClass(t.type)}>{t.type}</span>
                    </td>
                    <td className={`px-4 py-3 font-semibold ${t.type?.toLowerCase() === 'income' ? 'text-success-600' : 'text-danger-600'}`}>
                      {t.type?.toLowerCase() === 'income' ? '+' : '-'}{formatCurrency(t.amount)}
                    </td>
                    <td className="px-4 py-3 text-gray-400">{formatDate(t.date)}</td>
                    <td className="px-4 py-3 text-gray-500">{t.createdBy}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-1">
                        {canWrite && (
                          <button
                            onClick={() => { setEditItem(t); setShowModal(true) }}
                            className="w-8 h-8 rounded-lg hover:bg-primary-50 text-gray-400 hover:text-primary-600 flex items-center justify-center transition-colors"
                          >
                            <Edit2 size={14} />
                          </button>
                        )}
                        {canDelete && (
                          <button
                            onClick={() => setDeleteId(t.id)}
                            className="w-8 h-8 rounded-lg hover:bg-danger-50 text-gray-400 hover:text-danger-600 flex items-center justify-center transition-colors"
                          >
                            <Trash2 size={14} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Pagination */}
        {!loading && transactions.length > 0 && (
          <div className="border-t border-gray-100 px-4">
            <Pagination
              page={pagination.page}
              totalPages={pagination.totalPages}
              totalCount={pagination.totalCount}
              pageSize={pagination.pageSize}
              onPageChange={(p) => fetchTransactions({ page: p })}
            />
          </div>
        )}
      </div>

      {/* Modals */}
      <TransactionModal
        isOpen={showModal}
        editItem={editItem}
        onSave={handleSave}
        onClose={() => { setShowModal(false); setEditItem(null) }}
      />
      <ConfirmModal
        isOpen={!!deleteId}
        title="Delete Transaction"
        message="This transaction will be permanently removed. This action cannot be undone."
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
        loading={deleting}
      />
    </div>
  )
}

export default TransactionsPage
