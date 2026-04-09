import api from './axiosInstance'

export const transactionsApi = {
  getAll: (params = {}) =>
    api.get('/api/transactions', { params }),

  getById: (id) =>
    api.get(`/api/transactions/${id}`),

  create: (data) =>
    api.post('/api/transactions', data),

  update: (id, data) =>
    api.put(`/api/transactions/${id}`, data),

  delete: (id) =>
    api.delete(`/api/transactions/${id}`),
}
