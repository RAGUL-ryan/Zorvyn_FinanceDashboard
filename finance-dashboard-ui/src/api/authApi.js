import api from './axiosInstance'

export const authApi = {
  login: (email, password) =>
    api.post('/api/auth/login', { email, password }),

  logout: () =>
    api.post('/api/auth/logout'),

  refreshToken: (refreshToken) =>
    api.post('/api/auth/refresh-token', { refreshToken }),

  me: () =>
    api.get('/api/auth/me'),
}
