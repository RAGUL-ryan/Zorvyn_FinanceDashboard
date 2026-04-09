import { useState, useEffect } from 'react'
import { X } from 'lucide-react'
import { CATEGORIES, TRANSACTION_TYPES } from '../../utils/formatters'

const EMPTY = { amount: '', type: 'Income', category: '', date: '', description: '', notes: '' }

const Field = ({ label, error, children }) => (
  <div>
    <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>
    {children}
    {error && <p className="text-xs text-danger-500 mt-1">{error}</p>}
  </div>
)

const TransactionModal = ({ isOpen, editItem, onSave, onClose }) => {
  const [form,    setForm]    = useState(EMPTY)
  const [errors,  setErrors]  = useState({})
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (editItem) {
      setForm({
        amount:      editItem.amount ?? '',
        type:        editItem.type ?? 'Income',
        category:    editItem.category ?? '',
        date:        editItem.date ? editItem.date.substring(0, 10) : '',
        description: editItem.description ?? '',
        notes:       editItem.notes ?? '',
      })
    } else {
      setForm({ ...EMPTY, date: new Date().toISOString().substring(0, 10) })
    }
    setErrors({})
  }, [editItem, isOpen])

  const validate = () => {
    const e = {}
    if (!form.amount || Number(form.amount) <= 0) e.amount = 'Amount must be greater than 0'
    if (!form.type)        e.type        = 'Type is required'
    if (!form.category)    e.category    = 'Category is required'
    if (!form.date)        e.date        = 'Date is required'
    if (!form.description) e.description = 'Description is required'
    setErrors(e)
    return !Object.keys(e).length
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      await onSave({
        amount:      Number(form.amount),
        type:        form.type,
        category:    form.category,
        date:        new Date(form.date).toISOString(),
        description: form.description,
        notes:       form.notes || null,
      })
    } finally {
      setLoading(false)
    }
  }

  const set = (k) => (e) => {
    setForm(f => ({ ...f, [k]: e.target.value }))
    setErrors(er => ({ ...er, [k]: '' }))
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={onClose} />
      <div className="relative bg-white rounded-2xl shadow-xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-800">
            {editItem ? 'Edit Transaction' : 'New Transaction'}
          </h2>
          <button onClick={onClose} className="w-8 h-8 rounded-lg hover:bg-gray-100 flex items-center justify-center text-gray-400 transition-colors">
            <X size={18} />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Field label="Amount (₹)" error={errors.amount}>
              <input
                type="number" step="0.01" min="0.01"
                value={form.amount} onChange={set('amount')}
                placeholder="0.00"
                className={`input-field ${errors.amount ? 'border-danger-500' : ''}`}
              />
            </Field>
            <Field label="Type" error={errors.type}>
              <select value={form.type} onChange={set('type')} className={`input-field ${errors.type ? 'border-danger-500' : ''}`}>
                {TRANSACTION_TYPES.map(t => <option key={t}>{t}</option>)}
              </select>
            </Field>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <Field label="Category" error={errors.category}>
              <select value={form.category} onChange={set('category')} className={`input-field ${errors.category ? 'border-danger-500' : ''}`}>
                <option value="">Select category</option>
                {CATEGORIES.map(c => <option key={c}>{c}</option>)}
              </select>
            </Field>
            <Field label="Date" error={errors.date}>
              <input
                type="date" value={form.date} onChange={set('date')}
                max={new Date().toISOString().substring(0, 10)}
                className={`input-field ${errors.date ? 'border-danger-500' : ''}`}
              />
            </Field>
          </div>

          <Field label="Description" error={errors.description}>
            <input
              type="text" value={form.description} onChange={set('description')}
              placeholder="Brief description of this transaction"
              className={`input-field ${errors.description ? 'border-danger-500' : ''}`}
              maxLength={500}
            />
          </Field>

          <Field label="Notes (optional)">
            <textarea
              value={form.notes} onChange={set('notes')}
              placeholder="Additional notes..."
              rows={3}
              className="input-field resize-none"
              maxLength={1000}
            />
          </Field>

          {/* Actions */}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="btn-secondary flex-1">Cancel</button>
            <button type="submit" disabled={loading} className="btn-primary flex-1">
              {loading ? (
                <><span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" /> Saving...</>
              ) : editItem ? 'Update Transaction' : 'Create Transaction'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default TransactionModal
