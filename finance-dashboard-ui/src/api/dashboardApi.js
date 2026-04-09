import api from './axiosInstance'

export const dashboardApi = {
  getSummary: (params = {}) =>
    api.get('/api/dashboard/summary', { params }),

  getTrends: (months = 6) =>
    api.get('/api/dashboard/trends', { params: { months } }),

  getCategories: (params = {}) =>
    api.get('/api/dashboard/categories', { params }),
}
