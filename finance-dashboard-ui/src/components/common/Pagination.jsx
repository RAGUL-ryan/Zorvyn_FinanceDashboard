import { ChevronLeft, ChevronRight } from 'lucide-react'

const Pagination = ({ page, totalPages, totalCount, pageSize, onPageChange }) => {
  if (totalPages <= 1) return null

  const from = (page - 1) * pageSize + 1
  const to   = Math.min(page * pageSize, totalCount)

  const pages = []
  const delta = 2
  for (let i = Math.max(1, page - delta); i <= Math.min(totalPages, page + delta); i++) {
    pages.push(i)
  }

  return (
    <div className="flex items-center justify-between px-2 py-3">
      <p className="text-sm text-gray-500">
        Showing <span className="font-medium text-gray-700">{from}–{to}</span> of{' '}
        <span className="font-medium text-gray-700">{totalCount}</span> records
      </p>

      <div className="flex items-center gap-1">
        <button
          onClick={() => onPageChange(page - 1)}
          disabled={page === 1}
          className="w-8 h-8 flex items-center justify-center rounded-lg border border-gray-200 text-gray-500 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        >
          <ChevronLeft size={16} />
        </button>

        {pages[0] > 1 && (
          <>
            <button onClick={() => onPageChange(1)} className="w-8 h-8 flex items-center justify-center rounded-lg text-sm border border-gray-200 hover:bg-gray-50 transition-colors">1</button>
            {pages[0] > 2 && <span className="px-1 text-gray-400 text-sm">…</span>}
          </>
        )}

        {pages.map((p) => (
          <button
            key={p}
            onClick={() => onPageChange(p)}
            className={`w-8 h-8 flex items-center justify-center rounded-lg text-sm border transition-colors
              ${p === page
                ? 'bg-primary-600 border-primary-600 text-white font-medium'
                : 'border-gray-200 text-gray-600 hover:bg-gray-50'}`}
          >
            {p}
          </button>
        ))}

        {pages[pages.length - 1] < totalPages && (
          <>
            {pages[pages.length - 1] < totalPages - 1 && <span className="px-1 text-gray-400 text-sm">…</span>}
            <button onClick={() => onPageChange(totalPages)} className="w-8 h-8 flex items-center justify-center rounded-lg text-sm border border-gray-200 hover:bg-gray-50 transition-colors">{totalPages}</button>
          </>
        )}

        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page === totalPages}
          className="w-8 h-8 flex items-center justify-center rounded-lg border border-gray-200 text-gray-500 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        >
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  )
}

export default Pagination
