import {
    Package,
    Warehouse,
    AlertTriangle,
    TrendingDown,
    ArrowUpRight,
    ArrowDownRight,
    MoreHorizontal,
    RefreshCw,
} from 'lucide-react';
import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    PieChart,
    Pie,
    Cell,
} from 'recharts';
import StatCard from '../../components/StatCard';
import './WMSDashboard.css';

// Mock data
const stockMovementData = [
    { name: 'Mon', inbound: 450, outbound: 320 },
    { name: 'Tue', inbound: 380, outbound: 420 },
    { name: 'Wed', inbound: 520, outbound: 390 },
    { name: 'Thu', inbound: 490, outbound: 510 },
    { name: 'Fri', inbound: 600, outbound: 480 },
    { name: 'Sat', inbound: 350, outbound: 280 },
    { name: 'Sun', inbound: 220, outbound: 180 },
];

const categoryData = [
    { name: 'Beverages', value: 2450 },
    { name: 'Bakery', value: 1890 },
    { name: 'Snacks', value: 1560 },
    { name: 'Food', value: 980 },
    { name: 'Desserts', value: 720 },
];

const COLORS = ['#6366f1', '#22d3ee', '#34d399', '#fbbf24', '#f87171'];

const lowStockItems = [
    { sku: 'BKR-023', name: 'Butter Croissant', warehouse: 'WH-001', current: 12, reorderPoint: 50, status: 'critical' },
    { sku: 'SND-001', name: 'Club Sandwich', warehouse: 'WH-001', current: 18, reorderPoint: 30, status: 'critical' },
    { sku: 'ICE-001', name: 'Vanilla Ice Cream', warehouse: 'WH-002', current: 35, reorderPoint: 50, status: 'low' },
    { sku: 'BKR-067', name: 'Danish Pastry', warehouse: 'WH-001', current: 28, reorderPoint: 40, status: 'low' },
    { sku: 'SMT-001', name: 'Berry Smoothie', warehouse: 'WH-002', current: 42, reorderPoint: 50, status: 'low' },
];

const recentMovements = [
    { id: '1', sku: 'COF-001', type: 'outbound', quantity: 24, reference: 'TRX-2026-001234', time: '5 mins ago' },
    { id: '2', sku: 'TEA-012', type: 'outbound', quantity: 12, reference: 'TRX-2026-001233', time: '8 mins ago' },
    { id: '3', sku: 'BKR-045', type: 'inbound', quantity: 100, reference: 'PO-2026-0045', time: '15 mins ago' },
    { id: '4', sku: 'SNK-101', type: 'outbound', quantity: 8, reference: 'TRX-2026-001230', time: '22 mins ago' },
    { id: '5', sku: 'COF-010', type: 'inbound', quantity: 200, reference: 'PO-2026-0044', time: '1 hour ago' },
];

export default function WMSDashboard() {
    const formatNumber = (num: number) => {
        return new Intl.NumberFormat('id-ID').format(num);
    };

    return (
        <div className="wms-dashboard animate-fade-in">
            {/* Page Header */}
            <div className="dashboard-header">
                <div className="header-content">
                    <h1 className="page-title">WMS Dashboard</h1>
                    <p className="page-subtitle">
                        Real-time inventory and warehouse operations overview
                    </p>
                </div>
                <button className="btn btn-secondary">
                    <RefreshCw size={16} />
                    Sync Inventory
                </button>
            </div>

            {/* Stats Grid */}
            <div className="stats-grid">
                <StatCard
                    icon={Package}
                    value={formatNumber(15847)}
                    label="Total SKUs"
                    trend={{ value: 2.8, isPositive: true }}
                    color="primary"
                />
                <StatCard
                    icon={Warehouse}
                    value={formatNumber(245680)}
                    label="Items in Stock"
                    trend={{ value: 5.4, isPositive: true }}
                    color="cyan"
                />
                <StatCard
                    icon={AlertTriangle}
                    value={formatNumber(23)}
                    label="Low Stock Alerts"
                    trend={{ value: 12, isPositive: false }}
                    color="warning"
                />
                <StatCard
                    icon={TrendingDown}
                    value={formatNumber(5)}
                    label="Out of Stock"
                    trend={{ value: 2, isPositive: true }}
                    color="error"
                />
            </div>

            {/* Charts Row */}
            <div className="charts-row">
                <div className="chart-card movements-chart">
                    <div className="chart-header">
                        <div>
                            <h3 className="chart-title">Stock Movements</h3>
                            <p className="chart-subtitle">Weekly inbound vs outbound</p>
                        </div>
                        <button className="btn btn-ghost btn-sm">
                            View Details <ArrowUpRight size={14} />
                        </button>
                    </div>
                    <div className="chart-container">
                        <ResponsiveContainer width="100%" height={280}>
                            <AreaChart data={stockMovementData}>
                                <defs>
                                    <linearGradient id="colorInbound" x1="0" y1="0" x2="0" y2="1">
                                        <stop offset="5%" stopColor="#22c55e" stopOpacity={0.3} />
                                        <stop offset="95%" stopColor="#22c55e" stopOpacity={0} />
                                    </linearGradient>
                                    <linearGradient id="colorOutbound" x1="0" y1="0" x2="0" y2="1">
                                        <stop offset="5%" stopColor="#6366f1" stopOpacity={0.3} />
                                        <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                                    </linearGradient>
                                </defs>
                                <CartesianGrid strokeDasharray="3 3" stroke="#2a2a38" />
                                <XAxis
                                    dataKey="name"
                                    stroke="#64748b"
                                    fontSize={12}
                                    tickLine={false}
                                />
                                <YAxis
                                    stroke="#64748b"
                                    fontSize={12}
                                    tickLine={false}
                                />
                                <Tooltip
                                    contentStyle={{
                                        background: '#1a1a24',
                                        border: '1px solid rgba(255,255,255,0.08)',
                                        borderRadius: '8px',
                                        color: '#f8fafc',
                                    }}
                                />
                                <Area
                                    type="monotone"
                                    dataKey="inbound"
                                    stroke="#22c55e"
                                    strokeWidth={2}
                                    fillOpacity={1}
                                    fill="url(#colorInbound)"
                                    name="Inbound"
                                />
                                <Area
                                    type="monotone"
                                    dataKey="outbound"
                                    stroke="#6366f1"
                                    strokeWidth={2}
                                    fillOpacity={1}
                                    fill="url(#colorOutbound)"
                                    name="Outbound"
                                />
                            </AreaChart>
                        </ResponsiveContainer>
                    </div>
                    <div className="chart-legend">
                        <div className="legend-item">
                            <span className="legend-dot" style={{ background: '#22c55e' }}></span>
                            Inbound
                        </div>
                        <div className="legend-item">
                            <span className="legend-dot" style={{ background: '#6366f1' }}></span>
                            Outbound
                        </div>
                    </div>
                </div>

                <div className="chart-card category-chart">
                    <div className="chart-header">
                        <div>
                            <h3 className="chart-title">Stock by Category</h3>
                            <p className="chart-subtitle">Distribution of inventory</p>
                        </div>
                        <button className="btn btn-ghost btn-icon btn-sm">
                            <MoreHorizontal size={16} />
                        </button>
                    </div>
                    <div className="chart-container pie-container">
                        <ResponsiveContainer width="100%" height={220}>
                            <PieChart>
                                <Pie
                                    data={categoryData}
                                    cx="50%"
                                    cy="50%"
                                    innerRadius={60}
                                    outerRadius={90}
                                    paddingAngle={2}
                                    dataKey="value"
                                >
                                    {categoryData.map((_, index) => (
                                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                    ))}
                                </Pie>
                                <Tooltip
                                    contentStyle={{
                                        background: '#1a1a24',
                                        border: '1px solid rgba(255,255,255,0.08)',
                                        borderRadius: '8px',
                                        color: '#f8fafc',
                                    }}
                                />
                            </PieChart>
                        </ResponsiveContainer>
                        <div className="pie-legend">
                            {categoryData.map((item, index) => (
                                <div key={item.name} className="pie-legend-item">
                                    <span className="legend-dot" style={{ background: COLORS[index] }}></span>
                                    <span className="legend-label">{item.name}</span>
                                    <span className="legend-value">{formatNumber(item.value)}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>

            {/* Tables Row */}
            <div className="tables-row">
                {/* Low Stock Alerts */}
                <div className="table-card low-stock">
                    <div className="table-header">
                        <div>
                            <h3 className="table-title">Low Stock Alerts</h3>
                            <p className="table-subtitle">Items requiring attention</p>
                        </div>
                        <button className="btn btn-secondary btn-sm">View All</button>
                    </div>
                    <div className="table-wrapper">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Warehouse</th>
                                    <th>Current</th>
                                    <th>Reorder At</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                {lowStockItems.map((item) => (
                                    <tr key={item.sku}>
                                        <td>
                                            <div className="product-cell">
                                                <span className="product-name">{item.name}</span>
                                                <span className="product-sku">{item.sku}</span>
                                            </div>
                                        </td>
                                        <td>{item.warehouse}</td>
                                        <td className="font-semibold">{item.current}</td>
                                        <td className="text-muted">{item.reorderPoint}</td>
                                        <td>
                                            <span className={`badge ${item.status === 'critical' ? 'badge-error' : 'badge-warning'}`}>
                                                {item.status}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* Recent Movements */}
                <div className="table-card recent-movements">
                    <div className="table-header">
                        <div>
                            <h3 className="table-title">Recent Movements</h3>
                            <p className="table-subtitle">Latest stock activity</p>
                        </div>
                    </div>
                    <div className="movements-list">
                        {recentMovements.map((movement) => (
                            <div key={movement.id} className="movement-item">
                                <div className={`movement-icon ${movement.type}`}>
                                    {movement.type === 'inbound' ? (
                                        <ArrowDownRight size={16} />
                                    ) : (
                                        <ArrowUpRight size={16} />
                                    )}
                                </div>
                                <div className="movement-info">
                                    <span className="movement-sku">{movement.sku}</span>
                                    <span className="movement-ref">{movement.reference}</span>
                                </div>
                                <div className="movement-details">
                                    <span className={`movement-qty ${movement.type}`}>
                                        {movement.type === 'inbound' ? '+' : '-'}{movement.quantity}
                                    </span>
                                    <span className="movement-time">{movement.time}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}
