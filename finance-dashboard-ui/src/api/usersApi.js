import api from './axiosInstance'

export const usersApi = {
  getAll: () =>
    api.get('/api/users'),

  getById: (id) =>
    api.get(`/api/users/${id}`),

  create: (data) =>
    api.post('/api/users', data),

  update: (id, data) =>
    api.put(`/api/users/${id}`, data),

  delete: (id) =>
    api.delete(`/api/users/${id}`),

  setStatus: (id, isActive) =>
    api.patch(`/api/users/${id}/status`, null, { params: { isActive } }),

  changePassword: (data) =>
    api.post('/api/users/change-password', data),
}
