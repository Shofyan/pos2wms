import { NavLink } from 'react-router-dom';
import {
    LayoutDashboard,
    ShoppingCart,
    Package,
    History,
    BarChart3,
    Settings,
    Warehouse,
    ArrowLeftRight,
    AlertTriangle,
    ChevronRight,
    Store,
} from 'lucide-react';
import './Sidebar.css';

interface SidebarProps {
    activeSystem: 'pos' | 'wms';
    onSystemChange: (system: 'pos' | 'wms') => void;
}

const posNavItems = [
    { path: '/pos', label: 'Dashboard', icon: LayoutDashboard, end: true },
    { path: '/pos/terminal', label: 'POS Terminal', icon: ShoppingCart },
    { path: '/pos/sales', label: 'Sales History', icon: History },
    { path: '/pos/reports', label: 'Reports', icon: BarChart3 },
];

const wmsNavItems = [
    { path: '/wms', label: 'Dashboard', icon: LayoutDashboard, end: true },
    { path: '/wms/inventory', label: 'Inventory', icon: Package },
    { path: '/wms/movements', label: 'Stock Movements', icon: ArrowLeftRight },
    { path: '/wms/alerts', label: 'Stock Alerts', icon: AlertTriangle },
];

export default function Sidebar({ activeSystem, onSystemChange }: SidebarProps) {
    const navItems = activeSystem === 'pos' ? posNavItems : wmsNavItems;

    return (
        <aside className="sidebar">
            {/* Logo */}
            <div className="sidebar-header">
                <div className="logo">
                    <div className="logo-icon">
                        {activeSystem === 'pos' ? <Store size={24} /> : <Warehouse size={24} />}
                    </div>
                    <div className="logo-text">
                        <span className="logo-title">{activeSystem === 'pos' ? 'POS' : 'WMS'}</span>
                        <span className="logo-subtitle">Management</span>
                    </div>
                </div>
            </div>

            {/* System Selector */}
            <div className="system-selector">
                <button
                    className={`system-btn ${activeSystem === 'pos' ? 'active' : ''}`}
                    onClick={() => onSystemChange('pos')}
                >
                    <Store size={16} />
                    <span>POS</span>
                </button>
                <button
                    className={`system-btn ${activeSystem === 'wms' ? 'active' : ''}`}
                    onClick={() => onSystemChange('wms')}
                >
                    <Warehouse size={16} />
                    <span>WMS</span>
                </button>
            </div>

            {/* Navigation */}
            <nav className="sidebar-nav">
                <ul className="nav-list">
                    {navItems.map((item) => (
                        <li key={item.path}>
                            <NavLink
                                to={item.path}
                                end={item.end}
                                className={({ isActive }) =>
                                    `nav-link ${isActive ? 'active' : ''}`
                                }
                            >
                                <item.icon size={20} />
                                <span>{item.label}</span>
                                <ChevronRight className="nav-arrow" size={16} />
                            </NavLink>
                        </li>
                    ))}
                </ul>
            </nav>

            {/* Footer */}
            <div className="sidebar-footer">
                <NavLink to="/settings" className="nav-link">
                    <Settings size={20} />
                    <span>Settings</span>
                </NavLink>
                <div className="sidebar-info">
                    <span className="status-dot"></span>
                    <span className="status-text">System Online</span>
                </div>
            </div>
        </aside>
    );
}
