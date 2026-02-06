import { Bell, Search, User, Menu } from 'lucide-react';
import './Header.css';

interface HeaderProps {
    onMenuClick: () => void;
}

export default function Header({ onMenuClick }: HeaderProps) {
    return (
        <header className="header">
            <div className="header-left">
                <button className="menu-btn btn btn-ghost btn-icon" onClick={onMenuClick}>
                    <Menu size={20} />
                </button>
                <div className="search-container">
                    <Search size={18} className="search-icon" />
                    <input
                        type="text"
                        placeholder="Search transactions, products..."
                        className="search-input"
                    />
                    <kbd className="search-kbd">⌘K</kbd>
                </div>
            </div>

            <div className="header-right">
                <button className="btn btn-ghost btn-icon notification-btn">
                    <Bell size={20} />
                    <span className="notification-badge">3</span>
                </button>

                <div className="header-divider"></div>

                <div className="user-menu">
                    <div className="user-avatar">
                        <User size={18} />
                    </div>
                    <div className="user-info">
                        <span className="user-name">Admin User</span>
                        <span className="user-role">Store Manager</span>
                    </div>
                </div>
            </div>
        </header>
    );
}
