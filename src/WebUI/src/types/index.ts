// Sale Types
export interface SaleItem {
    id: string;
    sku: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    discountAmount: number;
    taxRate: number;
    taxAmount: number;
    lineTotal: number;
}

export interface Payment {
    id: string;
    paymentMethod: 'cash' | 'card' | 'ewallet' | 'qris';
    amount: number;
    reference?: string;
    processedAt: string;
}

export interface Sale {
    id: string;
    transactionNumber: string;
    storeId: string;
    terminalId: string;
    cashierId?: string;
    customerId?: string;
    status: 'draft' | 'pending' | 'completed' | 'cancelled';
    subTotal: number;
    taxAmount: number;
    discountAmount: number;
    totalAmount: number;
    paidAmount: number;
    changeAmount: number;
    currency: string;
    completedAt?: string;
    cancelledAt?: string;
    cancellationReason?: string;
    createdAt: string;
    updatedAt: string;
    items: SaleItem[];
    payments: Payment[];
}

// Inventory Types
export interface Inventory {
    id: string;
    sku: string;
    warehouseId: string;
    locationId?: string;
    quantityOnHand: number;
    quantityReserved: number;
    quantityAvailable: number;
    reorderPoint: number;
    reorderQuantity: number;
    lastStockCheck: string;
    isLowStock: boolean;
    isOutOfStock: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface InventoryMovement {
    id: string;
    sku: string;
    warehouseId: string;
    movementType: 'sale' | 'return' | 'adjustment' | 'transfer' | 'receipt';
    quantity: number;
    previousQuantity: number;
    newQuantity: number;
    referenceId: string;
    referenceType: string;
    reason?: string;
    processedBy?: string;
    createdAt: string;
}

// Stats Types
export interface DashboardStats {
    totalSales: number;
    totalRevenue: number;
    avgOrderValue: number;
    completedOrders: number;
    pendingOrders: number;
    cancelledOrders: number;
}

export interface InventoryStats {
    totalProducts: number;
    totalStock: number;
    lowStockItems: number;
    outOfStockItems: number;
}

// Pagination
export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

// Cart for POS
export interface CartItem {
    sku: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    taxRate: number;
}

export interface Product {
    id: string;
    sku: string;
    name: string;
    description?: string;
    category: string;
    unitPrice: number;
    taxRate: number;
    imageUrl?: string;
    stock: number;
}
