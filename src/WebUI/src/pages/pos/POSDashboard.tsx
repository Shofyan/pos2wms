import { useState, useEffect } from 'react';
import {
    DollarSign,
    ShoppingCart,
    TrendingUp,
    CheckCircle,
    ArrowUpRight,
    MoreHorizontal,
} from 'lucide-react';
import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    BarChart,
    Bar,
} from 'recharts';
import StatCard from '../../components/StatCard';
import './POSDashboard.css';

// Mock data for demonstration
const revenueData = [
    { name: '00:00', revenue: 2400, transactions: 24 },
    { name: '04:00', revenue: 1398, transactions: 13 },
    { name: '08:00', revenue: 9800, transactions: 98 },
    { name: '12:00', revenue: 15390, transactions: 153 },
    { name: '16:00', revenue: 12400, transactions: 124 },
    { name: '20:00', revenue: 8780, transactions: 87 },
];

const productSalesData = [
    { name: 'Product A', sales: 4000 },
    { name: 'Product B', sales: 3000 },
    { name: 'Product C', sales: 2000 },
    { name: 'Product D', sales: 2780 },
    { name: 'Product E', sales: 1890 },
];

const recentSales = [
    {
        id: '1',
        transactionNumber: 'TRX-2026-001234',
        amount: 125000,
        items: 3,
        status: 'completed',
        time: '2 minutes ago',
        paymentMethod: 'QRIS',
    },
    {
        id: '2',
        transactionNumber: 'TRX-2026-001233',
        amount: 89500,
        items: 2,
        status: 'completed',
        time: '5 minutes ago',
        paymentMethod: 'Cash',
    },
    {
        id: '3',
        transactionNumber: 'TRX-2026-001232',
        amount: 215000,
        items: 5,
        status: 'pending',
        time: '8 minutes ago',
        paymentMethod: 'Card',
    },
    {
        id: '4',
        transactionNumber: 'TRX-2026-001231',
        amount: 45000,
        items: 1,
        status: 'cancelled',
        time: '12 minutes ago',
        paymentMethod: 'E-Wallet',
    },
    {
        id: '5',
        transactionNumber: 'TRX-2026-001230',
        amount: 178500,
        items: 4,
        status: 'completed',
        time: '15 minutes ago',
        paymentMethod: 'Cash',
    },
];

const topProducts = [
    { name: 'Premium Coffee Blend', sku: 'COF-001', sold: 245, revenue: 3675000 },
    { name: 'Organic Green Tea', sku: 'TEA-012', sold: 189, revenue: 1890000 },
    { name: 'Artisan Croissant', sku: 'BKR-023', sold: 156, revenue: 780000 },
    { name: 'Fresh Orange Juice', sku: 'JUC-005', sold: 134, revenue: 670000 },
    { name: 'Chocolate Muffin', sku: 'BKR-045', sold: 128, revenue: 384000 },
];

export default function POSDashboard() {
    const [currentTime, setCurrentTime] = useState(new Date());

    useEffect(() => {
        const timer = setInterval(() => setCurrentTime(new Date()), 1000);
        return () => clearInterval(timer);
    }, []);

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('id-ID', {
            style: 'currency',
            currency: 'IDR',
            minimumFractionDigits: 0,
        }).format(amount);
    };

    const formatNumber = (num: number) => {
        return new Intl.NumberFormat('id-ID').format(num);
    };

    const getStatusBadge = (status: string) => {
        const statusClasses: Record<string, string> = {
            completed: 'badge-success',
            pending: 'badge-warning',
            cancelled: 'badge-error',
        };
        return `badge ${statusClasses[status] || 'badge-default'}`;
    };

    return (
        <div className="pos-dashboard animate-fade-in">
            {/* Page Header */}
            <div className="dashboard-header">
                <div className="header-content">
                    <h1 className="page-title">POS Dashboard</h1>
                    <p className="page-subtitle">
                        Real-time overview of your sales performance
                    </p>
                </div>
                <div className="header-time">
                    <div className="time-display">
                        {currentTime.toLocaleTimeString('en-US', {
                            hour: '2-digit',
                            minute: '2-digit',
                            second: '2-digit',
                        })}
                    </div>
                    <div className="date-display">
                        {currentTime.toLocaleDateString('en-US', {
                            weekday: 'long',
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric',
                        })}
                    </div>
                </div>
            </div>

            {/* Stats Grid */}
            <div className="stats-grid">
                <StatCard
                    icon={DollarSign}
                    value={formatCurrency(48650000)}
                    label="Today's Revenue"
                    trend={{ value: 12.5, isPositive: true }}
                    color="primary"
                />
                <StatCard
                    icon={ShoppingCart}
                    value={formatNumber(524)}
                    label="Total Transactions"
                    trend={{ value: 8.2, isPositive: true }}
                    color="cyan"
                />
                <StatCard
                    icon={CheckCircle}
                    value={formatNumber(498)}
                    label="Completed Orders"
                    trend={{ value: 5.1, isPositive: true }}
                    color="success"
                />
                <StatCard
                    icon={TrendingUp}
                    value={formatCurrency(92843)}
                    label="Average Order Value"
                    trend={{ value: 3.2, isPositive: true }}
                    color="info"
                />
            </div>

            {/* Charts Row */}
            <div className="charts-row">
                <div className="chart-card revenue-chart">
                    <div className="chart-header">
                        <div>
                            <h3 className="chart-title">Revenue Overview</h3>
                            <p className="chart-subtitle">Today's hourly revenue</p>
                        </div>
                        <button className="btn btn-ghost btn-sm">
                            View Details <ArrowUpRight size={14} />
                        </button>
                    </div>
                    <div className="chart-container">
                        <ResponsiveContainer width="100%" height={280}>
                            <AreaChart data={revenueData}>
                                <defs>
                                    <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
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
                                    tickFormatter={(value) => `${value / 1000}k`}
                                />
                                <Tooltip
                                    contentStyle={{
                                        background: '#1a1a24',
                                        border: '1px solid rgba(255,255,255,0.08)',
                                        borderRadius: '8px',
                                        color: '#f8fafc',
                                    }}
                                    formatter={(value) => [formatCurrency(value as number), 'Revenue']}
                                />
                                <Area
                                    type="monotone"
                                    dataKey="revenue"
                                    stroke="#6366f1"
                                    strokeWidth={2}
                                    fillOpacity={1}
                                    fill="url(#colorRevenue)"
                                />
                            </AreaChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                <div className="chart-card products-chart">
                    <div className="chart-header">
                        <div>
                            <h3 className="chart-title">Top Products</h3>
                            <p className="chart-subtitle">Best selling items today</p>
                        </div>
                        <button className="btn btn-ghost btn-icon btn-sm">
                            <MoreHorizontal size={16} />
                        </button>
                    </div>
                    <div className="chart-container">
                        <ResponsiveContainer width="100%" height={280}>
                            <BarChart data={productSalesData} layout="vertical">
                                <CartesianGrid strokeDasharray="3 3" stroke="#2a2a38" />
                                <XAxis type="number" stroke="#64748b" fontSize={12} />
                                <YAxis
                                    dataKey="name"
                                    type="category"
                                    stroke="#64748b"
                                    fontSize={12}
                                    width={80}
                                />
                                <Tooltip
                                    contentStyle={{
                                        background: '#1a1a24',
                                        border: '1px solid rgba(255,255,255,0.08)',
                                        borderRadius: '8px',
                                        color: '#f8fafc',
                                    }}
                                />
                                <Bar dataKey="sales" fill="#22d3ee" radius={[0, 4, 4, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            </div>

            {/* Tables Row */}
            <div className="tables-row">
                {/* Recent Sales */}
                <div className="table-card recent-sales">
                    <div className="table-header">
                        <div>
                            <h3 className="table-title">Recent Transactions</h3>
                            <p className="table-subtitle">Latest sales activity</p>
                        </div>
                        <button className="btn btn-secondary btn-sm">View All</button>
                    </div>
                    <div className="table-wrapper">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Transaction</th>
                                    <th>Amount</th>
                                    <th>Payment</th>
                                    <th>Status</th>
                                    <th>Time</th>
                                </tr>
                            </thead>
                            <tbody>
                                {recentSales.map((sale) => (
                                    <tr key={sale.id}>
                                        <td>
                                            <div className="transaction-cell">
                                                <span className="transaction-number">
                                                    {sale.transactionNumber}
                                                </span>
                                                <span className="transaction-items">
                                                    {sale.items} items
                                                </span>
                                            </div>
                                        </td>
                                        <td className="font-semibold">
                                            {formatCurrency(sale.amount)}
                                        </td>
                                        <td>{sale.paymentMethod}</td>
                                        <td>
                                            <span className={getStatusBadge(sale.status)}>
                                                {sale.status}
                                            </span>
                                        </td>
                                        <td className="text-muted">{sale.time}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* Top Products */}
                <div className="table-card top-products">
                    <div className="table-header">
                        <div>
                            <h3 className="table-title">Top Selling Products</h3>
                            <p className="table-subtitle">Best performers today</p>
                        </div>
                    </div>
                    <div className="products-list">
                        {topProducts.map((product, index) => (
                            <div key={product.sku} className="product-item">
                                <div className="product-rank">{index + 1}</div>
                                <div className="product-info">
                                    <span className="product-name">{product.name}</span>
                                    <span className="product-sku">{product.sku}</span>
                                </div>
                                <div className="product-stats">
                                    <span className="product-sold">{product.sold} sold</span>
                                    <span className="product-revenue">
                                        {formatCurrency(product.revenue)}
                                    </span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}
