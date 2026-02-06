import { useState } from 'react';
import { Routes, Route, Navigate, useNavigate, useLocation } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Header from './components/Header';
import POSDashboard from './pages/pos/POSDashboard';
import POSTerminal from './pages/pos/POSTerminal';
import WMSDashboard from './pages/wms/WMSDashboard';
import WMSInventory from './pages/wms/WMSInventory';
import './App.css';

// Placeholder pages
const SalesHistory = () => (
  <div className="placeholder-page">
    <h1 className="page-title">Sales History</h1>
    <p className="page-subtitle">View and search all sales transactions</p>
    <div className="coming-soon">
      <span>Coming Soon</span>
    </div>
  </div>
);

const Reports = () => (
  <div className="placeholder-page">
    <h1 className="page-title">Reports</h1>
    <p className="page-subtitle">Generate and export business reports</p>
    <div className="coming-soon">
      <span>Coming Soon</span>
    </div>
  </div>
);

const StockMovements = () => (
  <div className="placeholder-page">
    <h1 className="page-title">Stock Movements</h1>
    <p className="page-subtitle">Track all inventory movements</p>
    <div className="coming-soon">
      <span>Coming Soon</span>
    </div>
  </div>
);

const StockAlerts = () => (
  <div className="placeholder-page">
    <h1 className="page-title">Stock Alerts</h1>
    <p className="page-subtitle">Manage low stock and reorder notifications</p>
    <div className="coming-soon">
      <span>Coming Soon</span>
    </div>
  </div>
);

const Settings = () => (
  <div className="placeholder-page">
    <h1 className="page-title">Settings</h1>
    <p className="page-subtitle">Configure system preferences</p>
    <div className="coming-soon">
      <span>Coming Soon</span>
    </div>
  </div>
);

function App() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  // Determine active system based on current path
  const activeSystem = location.pathname.startsWith('/wms') ? 'wms' : 'pos';

  const handleSystemChange = (system: 'pos' | 'wms') => {
    if (system === 'pos') {
      navigate('/pos');
    } else {
      navigate('/wms');
    }
  };

  return (
    <div className="app-layout">
      <Sidebar
        activeSystem={activeSystem}
        onSystemChange={handleSystemChange}
      />

      <main className="main-content">
        <Header onMenuClick={() => setSidebarOpen(!sidebarOpen)} />

        <div className="page-content">
          <Routes>
            {/* Default redirect */}
            <Route path="/" element={<Navigate to="/pos" replace />} />

            {/* POS Routes */}
            <Route path="/pos" element={<POSDashboard />} />
            <Route path="/pos/terminal" element={<POSTerminal />} />
            <Route path="/pos/sales" element={<SalesHistory />} />
            <Route path="/pos/reports" element={<Reports />} />

            {/* WMS Routes */}
            <Route path="/wms" element={<WMSDashboard />} />
            <Route path="/wms/inventory" element={<WMSInventory />} />
            <Route path="/wms/movements" element={<StockMovements />} />
            <Route path="/wms/alerts" element={<StockAlerts />} />

            {/* Settings */}
            <Route path="/settings" element={<Settings />} />

            {/* Catch all */}
            <Route path="*" element={<Navigate to="/pos" replace />} />
          </Routes>
        </div>
      </main>
    </div>
  );
}

export default App;
