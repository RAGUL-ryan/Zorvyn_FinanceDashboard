import { useState, useEffect } from 'react'
import { Plus, Edit2, Trash2, ToggleLeft, ToggleRight, Shield } from 'lucide-react'
import { usersApi } from '../api/usersApi'
import LoadingSpinner from '../components/common/LoadingSpinner'
import ErrorMessage from '../components/common/ErrorMessage'
import ConfirmModal from '../components/common/ConfirmModal'
import UserModal from '../components/users/UserModal'
import { getRoleBadgeClass, formatDate } from '../utils/formatters'
import toast from 'react-hot-toast'

const UsersPage = () => {
  const [users,      setUsers]      = useState([])
  const [loading,    setLoading]    = useState(true)
  const [error,      setError]      = useState(null)
  const [showModal,  setShowModal]  = useState(false)
  const [editUser,   setEditUser]   = useState(null)
  const [deleteId,   setDeleteId]   = useState(null)
  const [deleting,   setDeleting]   = useState(false)
  const [toggling,   setToggling]   = useState(null)

  const fetchUsers = async () => {
    setLoading(true)
    setError(null)
    try {
      const res = await usersApi.getAll()
      setUsers(res.data.data)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load users')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchUsers() }, [])

  const handleSave = async (data) => {
    try {
      if (editUser) {
        await usersApi.update(editUser.id, data)
        toast.success('User updated successfully')
      } else {
        await usersApi.create(data)
        toast.success('User created successfully')
      }
      setShowModal(false)
      setEditUser(null)
      fetchUsers()
    } catch (err) {
      toast.error(err.response?.data?.message || 'Operation failed')
      throw err
    }
  }

  const handleDelete = async () => {
    setDeleting(true)
    try {
      await usersApi.delete(deleteId)
      toast.success('User deleted successfully')
      setDeleteId(null)
      fetchUsers()
    } catch (err) {
      toast.error(err.response?.data?.message || 'Delete failed')
    } finally {
      setDeleting(false)
    }
  }

  const handleToggleStatus = async (user) => {
    setToggling(user.id)
    try {
      await usersApi.setStatus(user.id, !user.isActive)
      toast.success(`User ${user.isActive ? 'deactivated' : 'activated'} successfully`)
      fetchUsers()
    } catch (err) {
      toast.error(err.response?.data?.message || 'Status update failed')
    } finally {
      setToggling(null)
    }
  }

  if (loading) return <LoadingSpinner />
  if (error)   return <ErrorMessage message={error} onRetry={fetchUsers} />

  const activeCount   = users.filter(u => u.isActive).length
  const inactiveCount = users.length - activeCount

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-gray-800">User Management</h2>
          <p className="text-sm text-gray-400 mt-0.5">
            {users.length} total · <span className="text-success-600">{activeCount} active</span>
            {inactiveCount > 0 && <> · <span className="text-danger-500">{inactiveCount} inactive</span></>}
          </p>
        </div>
        <button onClick={() => { setEditUser(null); setShowModal(true) }} className="btn-primary">
          <Plus size={16} /> Add User
        </button>
      </div>

      {/* Stats row */}
      <div className="grid grid-cols-3 gap-4">
        {['Admin','Analyst','Viewer'].map(role => {
          const count = users.filter(u => u.role === role).length
          const colorMap = { Admin: 'text-purple-700 bg-purple-50', Analyst: 'text-blue-700 bg-blue-50', Viewer: 'text-gray-700 bg-gray-100' }
          return (
            <div key={role} className="card p-4 flex items-center gap-3">
              <div className={`w-9 h-9 rounded-lg flex items-center justify-center ${colorMap[role]}`}>
                <Shield size={16} />
              </div>
              <div>
                <p className="text-xl font-bold text-gray-800">{count}</p>
                <p className="text-xs text-gray-400">{role}s</p>
              </div>
            </div>
          )
        })}
      </div>

      {/* Table */}
      <div className="card overflow-hidden">
        {users.length === 0 ? (
          <div className="text-center py-16 text-gray-400">
            <p className="text-lg mb-1">No users found</p>
            <p className="text-sm">Add the first user to get started</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 border-b border-gray-100">
                <tr>
                  {['User','Email','Role','Status','Last Login','Actions'].map(h => (
                    <th key={h} className="text-left text-xs font-medium text-gray-400 uppercase tracking-wide px-4 py-3">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {users.map((u) => (
                  <tr key={u.id} className="hover:bg-gray-50/50 transition-colors">
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-xs font-bold flex-shrink-0">
                          {u.firstName?.[0]}{u.lastName?.[0]}
                        </div>
                        <div>
                          <p className="font-medium text-gray-800">{u.fullName}</p>
                          <p className="text-xs text-gray-400">ID: {u.id}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-gray-500">{u.email}</td>
                    <td className="px-4 py-3">
                      <span className={getRoleBadgeClass(u.role)}>{u.role}</span>
                    </td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium ${u.isActive ? 'bg-success-50 text-success-700' : 'bg-gray-100 text-gray-500'}`}>
                        <span className={`w-1.5 h-1.5 rounded-full ${u.isActive ? 'bg-success-500' : 'bg-gray-400'}`} />
                        {u.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-400">
                      {u.lastLoginAt ? formatDate(u.lastLoginAt) : 'Never'}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-1">
                        {/* Toggle active */}
                        <button
                          onClick={() => handleToggleStatus(u)}
                          disabled={toggling === u.id}
                          className={`w-8 h-8 rounded-lg flex items-center justify-center transition-colors
                            ${u.isActive
                              ? 'hover:bg-danger-50 text-gray-400 hover:text-danger-600'
                              : 'hover:bg-success-50 text-gray-400 hover:text-success-600'}`}
                          title={u.isActive ? 'Deactivate' : 'Activate'}
                        >
                          {toggling === u.id
                            ? <span className="w-3 h-3 border border-gray-300 border-t-gray-600 rounded-full animate-spin" />
                            : u.isActive ? <ToggleRight size={16} /> : <ToggleLeft size={16} />
                          }
                        </button>
                        <button
                          onClick={() => { setEditUser(u); setShowModal(true) }}
                          className="w-8 h-8 rounded-lg hover:bg-primary-50 text-gray-400 hover:text-primary-600 flex items-center justify-center transition-colors"
                        >
                          <Edit2 size={14} />
                        </button>
                        <button
                          onClick={() => setDeleteId(u.id)}
                          className="w-8 h-8 rounded-lg hover:bg-danger-50 text-gray-400 hover:text-danger-600 flex items-center justify-center transition-colors"
                        >
                          <Trash2 size={14} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <UserModal
        isOpen={showModal}
        editUser={editUser}
        onSave={handleSave}
        onClose={() => { setShowModal(false); setEditUser(null) }}
      />
      <ConfirmModal
        isOpen={!!deleteId}
        title="Delete User"
        message="This user will be soft-deleted and lose access to the system. You can reactivate them later if needed."
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
        loading={deleting}
      />
    </div>
  )
}

export default UsersPage
