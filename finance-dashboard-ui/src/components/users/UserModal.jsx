import { useState, useEffect } from 'react'
import { X, Eye, EyeOff } from 'lucide-react'
import { ROLES } from '../../utils/formatters'

const EMPTY = { firstName: '', lastName: '', email: '', password: '', roleId: 3 }

const Field = ({ label, error, children }) => (
  <div>
    <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>
    {children}
    {error && <p className="text-xs text-danger-500 mt-1">{error}</p>}
  </div>
)

const UserModal = ({ isOpen, editUser, onSave, onClose }) => {
  const [form,     setForm]     = useState(EMPTY)
  const [errors,   setErrors]   = useState({})
  const [loading,  setLoading]  = useState(false)
  const [showPass, setShowPass] = useState(false)

  useEffect(() => {
    if (editUser) {
      setForm({
        firstName: editUser.firstName ?? '',
        lastName:  editUser.lastName  ?? '',
        email:     editUser.email     ?? '',
        password:  '',
        roleId:    editUser.roleId    ?? 3,
      })
    } else {
      setForm(EMPTY)
    }
    setErrors({})
    setShowPass(false)
  }, [editUser, isOpen])

  const validate = () => {
    const e = {}
    if (!form.firstName.trim()) e.firstName = 'First name is required'
    if (!form.lastName.trim())  e.lastName  = 'Last name is required'
    if (!form.email.trim())     e.email     = 'Email is required'
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = 'Invalid email format'
    if (!editUser) {
      if (!form.password)          e.password = 'Password is required'
      else if (form.password.length < 8) e.password = 'Minimum 8 characters'
      else if (!/[A-Z]/.test(form.password))     e.password = 'Must contain uppercase letter'
      else if (!/[0-9]/.test(form.password))     e.password = 'Must contain a digit'
      else if (!/[^a-zA-Z0-9]/.test(form.password)) e.password = 'Must contain a special character'
    }
    if (!form.roleId) e.roleId = 'Role is required'
    setErrors(e)
    return !Object.keys(e).length
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = { ...form, roleId: Number(form.roleId) }
      if (editUser && !payload.password) delete payload.password
      await onSave(payload)
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
      <div className="relative bg-white rounded-2xl shadow-xl w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-800">
            {editUser ? 'Edit User' : 'New User'}
          </h2>
          <button onClick={onClose} className="w-8 h-8 rounded-lg hover:bg-gray-100 flex items-center justify-center text-gray-400">
            <X size={18} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Field label="First Name" error={errors.firstName}>
              <input value={form.firstName} onChange={set('firstName')} placeholder="John"
                className={`input-field ${errors.firstName ? 'border-danger-500' : ''}`} />
            </Field>
            <Field label="Last Name" error={errors.lastName}>
              <input value={form.lastName} onChange={set('lastName')} placeholder="Doe"
                className={`input-field ${errors.lastName ? 'border-danger-500' : ''}`} />
            </Field>
          </div>

          <Field label="Email Address" error={errors.email}>
            <input type="email" value={form.email} onChange={set('email')} placeholder="john@company.com"
              className={`input-field ${errors.email ? 'border-danger-500' : ''}`} />
          </Field>

          <Field label={editUser ? 'New Password (leave blank to keep current)' : 'Password'} error={errors.password}>
            <div className="relative">
              <input
                type={showPass ? 'text' : 'password'}
                value={form.password}
                onChange={set('password')}
                placeholder={editUser ? 'Leave blank to keep current' : 'Min 8 chars, uppercase, digit, symbol'}
                className={`input-field pr-10 ${errors.password ? 'border-danger-500' : ''}`}
              />
              <button type="button" onClick={() => setShowPass(v => !v)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                {showPass ? <EyeOff size={15} /> : <Eye size={15} />}
              </button>
            </div>
          </Field>

          <Field label="Role" error={errors.roleId}>
            <select value={form.roleId} onChange={set('roleId')}
              className={`input-field ${errors.roleId ? 'border-danger-500' : ''}`}>
              {ROLES.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
            </select>
            <p className="text-xs text-gray-400 mt-1">
              Admin: full access · Analyst: read + write · Viewer: read only
            </p>
          </Field>

          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="btn-secondary flex-1">Cancel</button>
            <button type="submit" disabled={loading} className="btn-primary flex-1">
              {loading
                ? <><span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" /> Saving...</>
                : editUser ? 'Update User' : 'Create User'
              }
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default UserModal
