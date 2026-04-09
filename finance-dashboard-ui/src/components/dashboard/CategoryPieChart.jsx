import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend } from 'recharts'
import { formatCurrency } from '../../utils/formatters'

const COLORS = ['#3b82f6','#22c55e','#f59e0b','#ef4444','#8b5cf6','#06b6d4','#f97316','#ec4899','#14b8a6','#6366f1']

const CustomTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null
  const d = payload[0]
  return (
    <div className="bg-white border border-gray-100 rounded-xl shadow-lg p-3 text-sm">
      <p className="font-semibold text-gray-700">{d.name}</p>
      <p className="text-gray-500">{formatCurrency(d.value)}</p>
      <p className="text-primary-600 font-medium">{d.payload.percentage}%</p>
    </div>
  )
}

const CategoryPieChart = ({ data = [] }) => {
  if (!data.length) {
    return (
      <div className="flex items-center justify-center h-48 text-gray-400 text-sm">
        No category data available
      </div>
    )
  }

  const chartData = data.map((d) => ({
    name:       d.category,
    value:      Number(d.total),
    percentage: d.percentage,
  }))

  return (
    <div className="flex flex-col gap-4">
      <ResponsiveContainer width="100%" height={220}>
        <PieChart>
          <Pie
            data={chartData}
            cx="50%"
            cy="50%"
            innerRadius={60}
            outerRadius={90}
            paddingAngle={3}
            dataKey="value"
          >
            {chartData.map((_, i) => (
              <Cell key={i} fill={COLORS[i % COLORS.length]} stroke="none" />
            ))}
          </Pie>
          <Tooltip content={<CustomTooltip />} />
        </PieChart>
      </ResponsiveContainer>

      {/* Legend list */}
      <div className="space-y-2 max-h-40 overflow-y-auto pr-1">
        {chartData.map((item, i) => (
          <div key={item.name} className="flex items-center justify-between text-sm">
            <div className="flex items-center gap-2">
              <span className="w-2.5 h-2.5 rounded-full flex-shrink-0" style={{ background: COLORS[i % COLORS.length] }} />
              <span className="text-gray-600 truncate max-w-[120px]">{item.name}</span>
            </div>
            <div className="flex items-center gap-2 text-right">
              <span className="text-gray-400 text-xs">{item.percentage}%</span>
              <span className="font-medium text-gray-700">{formatCurrency(item.value)}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default CategoryPieChart
