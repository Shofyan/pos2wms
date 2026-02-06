import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api/v1';

export const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor
api.interceptors.request.use(
    (config) => {
        // Add any auth headers here if needed
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response interceptor
api.interceptors.response.use(
    (response) => response,
    (error) => {
        // Handle common errors
        if (error.response?.status === 401) {
            // Handle unauthorized
            console.error('Unauthorized access');
        }
        return Promise.reject(error);
    }
);

// Sales API
export const salesApi = {
    create: async (data: {
        storeId: string;
        terminalId: string;
        cashierId?: string;
        customerId?: string;
        items: Array<{
            sku: string;
            productName: string;
            quantity: number;
            unitPrice: number;
            taxRate: number;
        }>;
    }) => {
        const response = await api.post('/sales', data);
        return response.data;
    },

    getById: async (id: string) => {
        const response = await api.get(`/sales/${id}`);
        return response.data;
    },

    getByStore: async (
        storeId: string,
        params?: {
            from?: string;
            to?: string;
            pageNumber?: number;
            pageSize?: number;
        }
    ) => {
        const response = await api.get(`/sales/store/${storeId}`, { params });
        return response.data;
    },

    complete: async (id: string, payments: Array<{ paymentMethod: string; amount: number }>) => {
        const response = await api.post(`/sales/${id}/complete`, { payments });
        return response.data;
    },

    cancel: async (id: string, reason: string, authorizedBy?: string) => {
        const response = await api.post(`/sales/${id}/cancel`, { reason, authorizedBy });
        return response.data;
    },
};

// Inventory API (WMS)
export const inventoryApi = {
    getAll: async (warehouseId: string) => {
        const response = await api.get(`/inventory/warehouse/${warehouseId}`);
        return response.data;
    },

    getBySku: async (sku: string) => {
        const response = await api.get(`/inventory/sku/${sku}`);
        return response.data;
    },

    getMovements: async (
        warehouseId: string,
        params?: {
            from?: string;
            to?: string;
            pageNumber?: number;
            pageSize?: number;
        }
    ) => {
        const response = await api.get(`/inventory/warehouse/${warehouseId}/movements`, { params });
        return response.data;
    },

    adjust: async (data: {
        sku: string;
        warehouseId: string;
        newQuantity: number;
        reason: string;
        adjustedBy: string;
    }) => {
        const response = await api.post('/inventory/adjust', data);
        return response.data;
    },
};

export default api;
