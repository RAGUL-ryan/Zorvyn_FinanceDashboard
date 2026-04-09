import { useState, useCallback } from 'react'
import { transactionsApi } from '../api/transactionsApi'
import toast from 'react-hot-toast'

export const useTransactions = () => {
  const [transactions, setTransactions] = useState([])
  const [pagination,   setPagination]   = useState({ page: 1, pageSize: 10, totalCount: 0, totalPages: 1 })
  const [loading,      setLoading]      = useState(false)
  const [filters,      setFilters]      = useState({ type: '', category: '', search: '', from: '', to: '' })

  const fetchTransactions = useCallback(async (params = {}) => {
    setLoading(true)
    try {
      const query = {
        page:     params.page     ?? pagination.page,
        pageSize: params.pageSize ?? pagination.pageSize,
        ...filters,
        ...params,
      }
      // Remove empty strings so API doesn't receive blank filters
      Object.keys(query).forEach((k) => { if (query[k] === '') delete query[k] })

      const res  = await transactionsApi.getAll(query)
      const data = res.data.data
      setTransactions(data.items)
      setPagination({ page: data.page, pageSize: data.pageSize, totalCount: data.totalCount, totalPages: data.totalPages })
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to load transactions')
    } finally {
      setLoading(false)
    }
  }, [filters, pagination.page, pagination.pageSize])

  const createTransaction = useCallback(async (data) => {
    const res = await transactionsApi.create(data)
    toast.success('Transaction created successfully')
    return res.data.data
  }, [])

  const updateTransaction = useCallback(async (id, data) => {
    const res = await transactionsApi.update(id, data)
    toast.success('Transaction updated successfully')
    return res.data.data
  }, [])

  const deleteTransaction = useCallback(async (id) => {
    await transactionsApi.delete(id)
    toast.success('Transaction deleted successfully')
  }, [])

  return {
    transactions, pagination, loading, filters,
    setFilters, fetchTransactions,
    createTransaction, updateTransaction, deleteTransaction,
  }
}
