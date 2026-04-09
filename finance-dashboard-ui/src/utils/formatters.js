export const formatCurrency = (amount) =>
  new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 }).format(amount)

export const formatDate = (dateStr) =>
  new Date(dateStr).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' })

export const formatDateTime = (dateStr) =>
  new Date(dateStr).toLocaleString('en-IN', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' })

export const toISODate = (dateStr) =>
  dateStr ? new Date(dateStr).toISOString() : null

export const getRoleBadgeClass = (role) => {
  const map = { Admin: 'badge-admin', Analyst: 'badge-analyst', Viewer: 'badge-viewer' }
  return map[role] ?? 'badge-viewer'
}

export const getTypeBadgeClass = (type) =>
  type?.toLowerCase() === 'income' ? 'badge-income' : 'badge-expense'

export const CATEGORIES = [
  'Salary', 'Freelance', 'Investment', 'Business',
  'Rent', 'Food', 'Transport', 'Utilities', 'Healthcare',
  'Education', 'Entertainment', 'Shopping', 'Other',
]

export const TRANSACTION_TYPES = ['Income', 'Expense']
export const ROLES = [
  { id: 1, name: 'Admin' },
  { id: 2, name: 'Analyst' },
  { id: 3, name: 'Viewer' },
]
