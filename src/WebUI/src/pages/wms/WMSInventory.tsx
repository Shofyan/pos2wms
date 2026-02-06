import { useState } from 'react';
import {
    Search,
    Filter,
    Download,
    RefreshCw,
    Package,
    ChevronLeft,
    ChevronRight,
    AlertTriangle,
    AlertCircle,
    CheckCircle,
    Edit2,
} from 'lucide-react';
import './WMSInventory.css';

interface InventoryItem {
    id: string;
    sku: string;
    name: string;
    category: string;
    warehouse: string;
    location: string;
    quantityOnHand: number;
    quantityReserved: number;
    quantityAvailable: number;
    reorderPoint: number;
    status: 'ok' | 'low' | 'critical' | 'out';
    lastUpdated: string;
}

// Mock data
const mockInventory: InventoryItem[] = [
    { id: '1', sku: 'COF-001', name: 'Premium Coffee Blend', category: 'Beverages', warehouse: 'WH-001', location: 'A-01-01', quantityOnHand: 150, quantityReserved: 12, quantityAvailable: 138, reorderPoint: 50, status: 'ok', lastUpdated: '2 hours ago' },
    { id: '2', sku: 'TEA-012', name: 'Organic Green Tea', category: 'Beverages', warehouse: 'WH-001', location: 'A-01-02', quantityOnHand: 89, quantityReserved: 5, quantityAvailable: 84, reorderPoint: 50, status: 'ok', lastUpdated: '3 hours ago' },
    { id: '3', sku: 'BKR-023', name: 'Butter Croissant', category: 'Bakery', warehouse: 'WH-001', location: 'B-02-01', quantityOnHand: 12, quantityReserved: 0, quantityAvailable: 12, reorderPoint: 50, status: 'critical', lastUpdated: '1 hour ago' },
    { id: '4', sku: 'BKR-045', name: 'Chocolate Muffin', category: 'Bakery', warehouse: 'WH-001', location: 'B-02-02', quantityOnHand: 32, quantityReserved: 2, quantityAvailable: 30, reorderPoint: 30, status: 'low', lastUpdated: '4 hours ago' },
    { id: '5', sku: 'JUC-005', name: 'Fresh Orange Juice', category: 'Beverages', warehouse: 'WH-002', location: 'C-01-01', quantityOnHand: 67, quantityReserved: 8, quantityAvailable: 59, reorderPoint: 40, status: 'ok', lastUpdated: '30 mins ago' },
    { id: '6', sku: 'SNK-101', name: 'Almond Cookies', category: 'Snacks', warehouse: 'WH-001', location: 'D-01-03', quantityOnHand: 78, quantityReserved: 0, quantityAvailable: 78, reorderPoint: 30, status: 'ok', lastUpdated: '5 hours ago' },
    { id: '7', sku: 'BKR-067', name: 'Danish Pastry', category: 'Bakery', warehouse: 'WH-001', location: 'B-03-01', quantityOnHand: 28, quantityReserved: 4, quantityAvailable: 24, reorderPoint: 40, status: 'low', lastUpdated: '2 hours ago' },
    { id: '8', sku: 'COF-010', name: 'Espresso Shot', category: 'Beverages', warehouse: 'WH-001', location: 'A-02-01', quantityOnHand: 200, quantityReserved: 15, quantityAvailable: 185, reorderPoint: 100, status: 'ok', lastUpdated: '1 hour ago' },
    { id: '9', sku: 'SMT-001', name: 'Berry Smoothie', category: 'Beverages', warehouse: 'WH-002', location: 'C-02-01', quantityOnHand: 0, quantityReserved: 0, quantityAvailable: 0, reorderPoint: 50, status: 'out', lastUpdated: '6 hours ago' },
    { id: '10', sku: 'SND-001', name: 'Club Sandwich', category: 'Food', warehouse: 'WH-001', location: 'E-01-01', quantityOnHand: 18, quantityReserved: 3, quantityAvailable: 15, reorderPoint: 30, status: 'critical', lastUpdated: '45 mins ago' },
];

const warehouses = ['All Warehouses', 'WH-001', 'WH-002'];
const categories = ['All Categories', 'Beverages', 'Bakery', 'Food', 'Snacks', 'Desserts'];
const statuses = ['All Status', 'In Stock', 'Low Stock', 'Critical', 'Out of Stock'];

export default function WMSInventory() {
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedWarehouse, setSelectedWarehouse] = useState('All Warehouses');
    const [selectedCategory, setSelectedCategory] = useState('All Categories');
    const [selectedStatus, setSelectedStatus] = useState('All Status');
    const [currentPage] = useState(1);

    const filteredInventory = mockInventory.filter((item) => {
        const matchesSearch =
            item.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            item.sku.toLowerCase().includes(searchQuery.toLowerCase());
        const matchesWarehouse = selectedWarehouse === 'All Warehouses' || item.warehouse === selectedWarehouse;
        const matchesCategory = selectedCategory === 'All Categories' || item.category === selectedCategory;
        const matchesStatus = selectedStatus === 'All Status' ||
            (selectedStatus === 'In Stock' && item.status === 'ok') ||
            (selectedStatus === 'Low Stock' && item.status === 'low') ||
            (selectedStatus === 'Critical' && item.status === 'critical') ||
            (selectedStatus === 'Out of Stock' && item.status === 'out');
        return matchesSearch && matchesWarehouse && matchesCategory && matchesStatus;
    });

    const getStatusBadge = (status: string) => {
        const statusConfig: Record<string, { class: string; label: string; icon: typeof CheckCircle }> = {
            ok: { class: 'badge-success', label: 'In Stock', icon: CheckCircle },
            low: { class: 'badge-warning', label: 'Low', icon: AlertTriangle },
            critical: { class: 'badge-error', label: 'Critical', icon: AlertCircle },
            out: { class: 'badge-error', label: 'Out of Stock', icon: AlertCircle },
        };
        const config = statusConfig[status];
        const Icon = config.icon;
        return (
            <span className={`badge ${config.class}`}>
                <Icon size={12} />
                {config.label}
            </span>
        );
    };

    const getStockBar = (available: number, reorderPoint: number) => {
        const percentage = Math.min((available / (reorderPoint * 2)) * 100, 100);
        let barClass = 'stock-bar-fill ok';
        if (available <= 0) barClass = 'stock-bar-fill out';
        else if (available <= reorderPoint * 0.5) barClass = 'stock-bar-fill critical';
        else if (available <= reorderPoint) barClass = 'stock-bar-fill low';

        return (
            <div className="stock-bar">
                <div className={barClass} style={{ width: `${percentage}%` }}></div>
            </div>
        );
    };

    return (
        <div className="wms-inventory animate-fade-in">
            {/* Page Header */}
            <div className="page-header">
                <div>
                    <h1 className="page-title">Inventory Management</h1>
                    <p className="page-subtitle">View and manage stock across all warehouses</p>
                </div>
                <div className="header-actions">
                    <button className="btn btn-secondary">
                        <Download size={16} />
                        Export
                    </button>
                    <button className="btn btn-primary">
                        <RefreshCw size={16} />
                        Sync Stock
                    </button>
                </div>
            </div>

            {/* Filters */}
            <div className="filters-bar">
                <div className="search-box">
                    <Search size={18} className="search-icon" />
                    <input
                        type="text"
                        placeholder="Search by SKU or product name..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="search-input"
                    />
                </div>

                <div className="filter-group">
                    <Filter size={16} />
                    <select
                        value={selectedWarehouse}
                        onChange={(e) => setSelectedWarehouse(e.target.value)}
                        className="filter-select"
                    >
                        {warehouses.map((wh) => (
                            <option key={wh} value={wh}>{wh}</option>
                        ))}
                    </select>

                    <select
                        value={selectedCategory}
                        onChange={(e) => setSelectedCategory(e.target.value)}
                        className="filter-select"
                    >
                        {categories.map((cat) => (
                            <option key={cat} value={cat}>{cat}</option>
                        ))}
                    </select>

                    <select
                        value={selectedStatus}
                        onChange={(e) => setSelectedStatus(e.target.value)}
                        className="filter-select"
                    >
                        {statuses.map((status) => (
                            <option key={status} value={status}>{status}</option>
                        ))}
                    </select>
                </div>
            </div>

            {/* Inventory Table */}
            <div className="inventory-table-card">
                <div className="table-wrapper">
                    <table className="table inventory-table">
                        <thead>
                            <tr>
                                <th>Product</th>
                                <th>Category</th>
                                <th>Location</th>
                                <th>On Hand</th>
                                <th>Reserved</th>
                                <th>Available</th>
                                <th>Stock Level</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredInventory.map((item) => (
                                <tr key={item.id}>
                                    <td>
                                        <div className="product-cell">
                                            <div className="product-icon">
                                                <Package size={18} />
                                            </div>
                                            <div className="product-info">
                                                <span className="product-name">{item.name}</span>
                                                <span className="product-sku">{item.sku}</span>
                                            </div>
                                        </div>
                                    </td>
                                    <td>{item.category}</td>
                                    <td>
                                        <div className="location-cell">
                                            <span className="warehouse">{item.warehouse}</span>
                                            <span className="location">{item.location}</span>
                                        </div>
                                    </td>
                                    <td className="font-semibold">{item.quantityOnHand}</td>
                                    <td className="text-muted">{item.quantityReserved}</td>
                                    <td className="font-semibold text-primary">{item.quantityAvailable}</td>
                                    <td>
                                        <div className="stock-level-cell">
                                            {getStockBar(item.quantityAvailable, item.reorderPoint)}
                                            <span className="stock-level-text">
                                                Reorder at {item.reorderPoint}
                                            </span>
                                        </div>
                                    </td>
                                    <td>{getStatusBadge(item.status)}</td>
                                    <td>
                                        <button className="btn btn-ghost btn-icon btn-sm">
                                            <Edit2 size={16} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                <div className="table-pagination">
                    <span className="pagination-info">
                        Showing {filteredInventory.length} of {mockInventory.length} items
                    </span>
                    <div className="pagination-controls">
                        <button className="btn btn-ghost btn-icon btn-sm" disabled>
                            <ChevronLeft size={16} />
                        </button>
                        <span className="page-number">Page {currentPage}</span>
                        <button className="btn btn-ghost btn-icon btn-sm">
                            <ChevronRight size={16} />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
