import { useState } from 'react';
import {
    Search,
    Plus,
    Minus,
    Trash2,
    CreditCard,
    Wallet,
    Banknote,
    Smartphone,
    ShoppingCart,
    User,
    Tag,
    Receipt,
    X,
    Check,
} from 'lucide-react';
import type { Product, CartItem } from '../../types';
import './POSTerminal.css';

// Mock products
const mockProducts: Product[] = [
    { id: '1', sku: 'COF-001', name: 'Premium Coffee', category: 'Beverages', unitPrice: 25000, taxRate: 0.11, stock: 150 },
    { id: '2', sku: 'TEA-012', name: 'Green Tea Latte', category: 'Beverages', unitPrice: 28000, taxRate: 0.11, stock: 89 },
    { id: '3', sku: 'BKR-023', name: 'Butter Croissant', category: 'Bakery', unitPrice: 18000, taxRate: 0.11, stock: 45 },
    { id: '4', sku: 'BKR-045', name: 'Chocolate Muffin', category: 'Bakery', unitPrice: 22000, taxRate: 0.11, stock: 32 },
    { id: '5', sku: 'JUC-005', name: 'Fresh Orange Juice', category: 'Beverages', unitPrice: 20000, taxRate: 0.11, stock: 67 },
    { id: '6', sku: 'SNK-101', name: 'Almond Cookies', category: 'Snacks', unitPrice: 35000, taxRate: 0.11, stock: 78 },
    { id: '7', sku: 'BKR-067', name: 'Danish Pastry', category: 'Bakery', unitPrice: 24000, taxRate: 0.11, stock: 28 },
    { id: '8', sku: 'COF-010', name: 'Espresso Shot', category: 'Beverages', unitPrice: 15000, taxRate: 0.11, stock: 200 },
    { id: '9', sku: 'SMT-001', name: 'Berry Smoothie', category: 'Beverages', unitPrice: 32000, taxRate: 0.11, stock: 45 },
    { id: '10', sku: 'SND-001', name: 'Club Sandwich', category: 'Food', unitPrice: 45000, taxRate: 0.11, stock: 20 },
    { id: '11', sku: 'SND-002', name: 'Grilled Cheese', category: 'Food', unitPrice: 38000, taxRate: 0.11, stock: 25 },
    { id: '12', sku: 'ICE-001', name: 'Vanilla Ice Cream', category: 'Desserts', unitPrice: 28000, taxRate: 0.11, stock: 40 },
];

const categories = ['All', 'Beverages', 'Bakery', 'Food', 'Snacks', 'Desserts'];

type PaymentMethod = 'cash' | 'card' | 'ewallet' | 'qris';

export default function POSTerminal() {
    const [cart, setCart] = useState<CartItem[]>([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [activeCategory, setActiveCategory] = useState('All');
    const [showPaymentModal, setShowPaymentModal] = useState(false);
    const [selectedPayment, setSelectedPayment] = useState<PaymentMethod | null>(null);
    const [cashReceived, setCashReceived] = useState<string>('');

    const filteredProducts = mockProducts.filter((product) => {
        const matchesSearch = product.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            product.sku.toLowerCase().includes(searchQuery.toLowerCase());
        const matchesCategory = activeCategory === 'All' || product.category === activeCategory;
        return matchesSearch && matchesCategory;
    });

    const addToCart = (product: Product) => {
        setCart((prev) => {
            const existing = prev.find((item) => item.sku === product.sku);
            if (existing) {
                return prev.map((item) =>
                    item.sku === product.sku
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                );
            }
            return [
                ...prev,
                {
                    sku: product.sku,
                    productName: product.name,
                    quantity: 1,
                    unitPrice: product.unitPrice,
                    taxRate: product.taxRate,
                },
            ];
        });
    };

    const updateQuantity = (sku: string, delta: number) => {
        setCart((prev) =>
            prev
                .map((item) =>
                    item.sku === sku
                        ? { ...item, quantity: Math.max(0, item.quantity + delta) }
                        : item
                )
                .filter((item) => item.quantity > 0)
        );
    };

    const removeFromCart = (sku: string) => {
        setCart((prev) => prev.filter((item) => item.sku !== sku));
    };

    const clearCart = () => {
        setCart([]);
        setShowPaymentModal(false);
        setSelectedPayment(null);
        setCashReceived('');
    };

    const subtotal = cart.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0);
    const taxAmount = cart.reduce(
        (sum, item) => sum + item.unitPrice * item.quantity * item.taxRate,
        0
    );
    const total = subtotal + taxAmount;
    const cashChange = cashReceived ? parseInt(cashReceived) - total : 0;

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('id-ID', {
            style: 'currency',
            currency: 'IDR',
            minimumFractionDigits: 0,
        }).format(amount);
    };

    const handlePayment = () => {
        // In a real app, this would call the API
        console.log('Processing payment:', {
            items: cart,
            total,
            paymentMethod: selectedPayment,
        });
        clearCart();
    };

    const paymentMethods = [
        { id: 'cash', label: 'Cash', icon: Banknote, color: '#22c55e' },
        { id: 'card', label: 'Card', icon: CreditCard, color: '#6366f1' },
        { id: 'ewallet', label: 'E-Wallet', icon: Wallet, color: '#f59e0b' },
        { id: 'qris', label: 'QRIS', icon: Smartphone, color: '#22d3ee' },
    ];

    const quickAmounts = [50000, 100000, 150000, 200000];

    return (
        <div className="pos-terminal">
            {/* Products Grid */}
            <div className="products-section">
                {/* Search & Categories */}
                <div className="products-header">
                    <div className="search-box">
                        <Search size={18} className="search-icon" />
                        <input
                            type="text"
                            placeholder="Search products or scan barcode..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="search-input"
                        />
                    </div>
                </div>

                <div className="categories-bar">
                    {categories.map((category) => (
                        <button
                            key={category}
                            className={`category-btn ${activeCategory === category ? 'active' : ''}`}
                            onClick={() => setActiveCategory(category)}
                        >
                            {category}
                        </button>
                    ))}
                </div>

                {/* Products Grid */}
                <div className="products-grid">
                    {filteredProducts.map((product) => (
                        <div
                            key={product.id}
                            className="product-card"
                            onClick={() => addToCart(product)}
                        >
                            <div className="product-image">
                                <Tag size={24} />
                            </div>
                            <div className="product-details">
                                <span className="product-name">{product.name}</span>
                                <span className="product-sku">{product.sku}</span>
                                <span className="product-price">{formatCurrency(product.unitPrice)}</span>
                            </div>
                            <div className="product-stock">
                                {product.stock > 50 ? (
                                    <span className="stock-good">{product.stock} in stock</span>
                                ) : product.stock > 10 ? (
                                    <span className="stock-low">{product.stock} in stock</span>
                                ) : (
                                    <span className="stock-critical">{product.stock} left</span>
                                )}
                            </div>
                            <div className="product-add">
                                <Plus size={20} />
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            {/* Cart Section */}
            <div className="cart-section">
                <div className="cart-header">
                    <div className="cart-title">
                        <ShoppingCart size={20} />
                        <span>Current Order</span>
                    </div>
                    {cart.length > 0 && (
                        <button className="btn btn-ghost btn-sm" onClick={clearCart}>
                            <Trash2 size={16} />
                            Clear
                        </button>
                    )}
                </div>

                {/* Customer Info */}
                <div className="customer-info">
                    <button className="customer-btn">
                        <User size={18} />
                        <span>Add Customer</span>
                        <Plus size={16} />
                    </button>
                </div>

                {/* Cart Items */}
                <div className="cart-items">
                    {cart.length === 0 ? (
                        <div className="cart-empty">
                            <Receipt size={48} />
                            <p>No items in cart</p>
                            <span>Select products from the left to add them here</span>
                        </div>
                    ) : (
                        cart.map((item) => (
                            <div key={item.sku} className="cart-item">
                                <div className="item-info">
                                    <span className="item-name">{item.productName}</span>
                                    <span className="item-price">
                                        {formatCurrency(item.unitPrice)} × {item.quantity}
                                    </span>
                                </div>
                                <div className="item-controls">
                                    <button
                                        className="qty-btn"
                                        onClick={() => updateQuantity(item.sku, -1)}
                                    >
                                        <Minus size={14} />
                                    </button>
                                    <span className="qty-value">{item.quantity}</span>
                                    <button
                                        className="qty-btn"
                                        onClick={() => updateQuantity(item.sku, 1)}
                                    >
                                        <Plus size={14} />
                                    </button>
                                    <button
                                        className="remove-btn"
                                        onClick={() => removeFromCart(item.sku)}
                                    >
                                        <Trash2 size={14} />
                                    </button>
                                </div>
                                <div className="item-total">
                                    {formatCurrency(item.unitPrice * item.quantity)}
                                </div>
                            </div>
                        ))
                    )}
                </div>

                {/* Cart Summary */}
                <div className="cart-summary">
                    <div className="summary-row">
                        <span>Subtotal</span>
                        <span>{formatCurrency(subtotal)}</span>
                    </div>
                    <div className="summary-row">
                        <span>Tax (11%)</span>
                        <span>{formatCurrency(taxAmount)}</span>
                    </div>
                    <div className="summary-row total">
                        <span>Total</span>
                        <span>{formatCurrency(total)}</span>
                    </div>
                </div>

                {/* Payment Button */}
                <button
                    className="btn btn-primary btn-lg pay-btn"
                    disabled={cart.length === 0}
                    onClick={() => setShowPaymentModal(true)}
                >
                    <CreditCard size={20} />
                    Proceed to Payment
                </button>
            </div>

            {/* Payment Modal */}
            {showPaymentModal && (
                <div className="modal-overlay" onClick={() => setShowPaymentModal(false)}>
                    <div className="payment-modal" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2>Complete Payment</h2>
                            <button
                                className="btn btn-ghost btn-icon"
                                onClick={() => setShowPaymentModal(false)}
                            >
                                <X size={20} />
                            </button>
                        </div>

                        <div className="modal-body">
                            <div className="payment-total">
                                <span>Total Amount</span>
                                <span className="total-value">{formatCurrency(total)}</span>
                            </div>

                            <div className="payment-methods">
                                <h3>Select Payment Method</h3>
                                <div className="methods-grid">
                                    {paymentMethods.map((method) => (
                                        <button
                                            key={method.id}
                                            className={`method-btn ${selectedPayment === method.id ? 'active' : ''}`}
                                            onClick={() => setSelectedPayment(method.id as PaymentMethod)}
                                            style={{ '--method-color': method.color } as React.CSSProperties}
                                        >
                                            <method.icon size={24} />
                                            <span>{method.label}</span>
                                        </button>
                                    ))}
                                </div>
                            </div>

                            {selectedPayment === 'cash' && (
                                <div className="cash-section">
                                    <h3>Cash Received</h3>
                                    <input
                                        type="number"
                                        className="cash-input"
                                        placeholder="Enter amount received"
                                        value={cashReceived}
                                        onChange={(e) => setCashReceived(e.target.value)}
                                    />
                                    <div className="quick-amounts">
                                        {quickAmounts.map((amount) => (
                                            <button
                                                key={amount}
                                                className="quick-amt-btn"
                                                onClick={() => setCashReceived(amount.toString())}
                                            >
                                                {formatCurrency(amount)}
                                            </button>
                                        ))}
                                        <button
                                            className="quick-amt-btn exact"
                                            onClick={() => setCashReceived(total.toString())}
                                        >
                                            Exact
                                        </button>
                                    </div>
                                    {parseInt(cashReceived) >= total && (
                                        <div className="change-display">
                                            <span>Change</span>
                                            <span className="change-value">{formatCurrency(cashChange)}</span>
                                        </div>
                                    )}
                                </div>
                            )}
                        </div>

                        <div className="modal-footer">
                            <button
                                className="btn btn-secondary btn-lg"
                                onClick={() => setShowPaymentModal(false)}
                            >
                                Cancel
                            </button>
                            <button
                                className="btn btn-success btn-lg"
                                disabled={!selectedPayment || (selectedPayment === 'cash' && parseInt(cashReceived || '0') < total)}
                                onClick={handlePayment}
                            >
                                <Check size={20} />
                                Complete Payment
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
